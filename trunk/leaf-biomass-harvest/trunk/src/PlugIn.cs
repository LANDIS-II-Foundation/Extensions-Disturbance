// Copyright 2008-2010 Green Code LLC, Portland State University
// Authors:  James B. Domingo, Robert M. Scheller, Srinivas S.

using Edu.Wisc.Forest.Flel.Util;
using Landis.Extension.BaseHarvest;
using Landis.Core;
using Landis.SpatialModeling;
using System.Collections.Generic;
using System.IO;
using System;

using BaseHarvest = Landis.Extension.BaseHarvest;

namespace Landis.Extension.LeafBiomassHarvest
{
    public class PlugIn
        : ExtensionMain
    {
        public static readonly ExtensionType type = new ExtensionType("disturbance:harvest");
        public static readonly string PlugInName = "Leaf Biomass Harvest";
        private IManagementAreaDataset managementAreas;
        private PrescriptionMaps prescriptionMaps;
        private string nameTemplate;
        private BiomassMaps biomassMaps;
        private StreamWriter log;
        private StreamWriter summaryLog;
        private bool running;
        int[] totalSites;
        int[] totalDamagedSites;
        int[,] totalSpeciesCohorts;
        int[] totalCohortsKilled;
        int[] totalCohortsDamaged;

        private static IInputParameters parameters;
        private static ICore modelCore;

        //---------------------------------------------------------------------

        public PlugIn()
            : base(PlugInName, type)
        {
        }

        //---------------------------------------------------------------------

        public static ICore ModelCore
        {
            get
            {
                return modelCore;
            }
        }

        //---------------------------------------------------------------------

        public override void LoadParameters(string dataFile,
                                            ICore mCore)
        {
            ExtensionMain baseHarvest = new BaseHarvest.PlugIn();
            try
            {
                baseHarvest.LoadParameters(null, mCore);
            }
            catch (System.ArgumentNullException)
            {
                // ignore
            }

            modelCore = mCore;

            // Add local event handler for cohorts death due to age-only
            // disturbances.
            Landis.Library.LeafBiomassCohorts.Cohort.AgeOnlyDeathEvent += CohortKilledByAgeOnlyDisturbance;

            InputParametersParser parser = new InputParametersParser(modelCore.Species);

            BaseHarvest.IInputParameters baseParameters = Landis.Data.Load<BaseHarvest.IInputParameters>(dataFile, parser);
            parameters = baseParameters as IInputParameters;
            if (parser.RoundedRepeatIntervals.Count > 0)
            {
                modelCore.UI.WriteLine("NOTE: The following repeat intervals were rounded up to");
                modelCore.UI.WriteLine("      ensure they were multiples of the harvest timestep:");
                modelCore.UI.WriteLine("      File: {0}", dataFile);
                foreach (RoundedInterval interval in parser.RoundedRepeatIntervals)
                    modelCore.UI.WriteLine("      At line {0}, the interval {1} rounded up to {2}",
                                 interval.LineNumber,
                                 interval.Original,
                                 interval.Adjusted);
            }
        }

        //---------------------------------------------------------------------

        public override void Initialize()
        {
            SiteVars.Initialize();
            PartialHarvestDisturbance.Initialize();

            Timestep = parameters.Timestep;
            managementAreas = parameters.ManagementAreas;
            
            //read management area map
            modelCore.UI.WriteLine("   Reading management-area map {0} ...", parameters.ManagementAreaMap);
            ManagementAreas.ReadMap(parameters.ManagementAreaMap, managementAreas);

            
            //  readMap reads the stand map and adds all the stands to a management area
            modelCore.UI.WriteLine("   Reading stand map {0} ...", parameters.StandMap);
            Stands.ReadMap(parameters.StandMap);
            
            //finish each managementArea's initialization
            foreach (ManagementArea mgmtArea in managementAreas)
                mgmtArea.FinishInitialization();

            prescriptionMaps = new PrescriptionMaps(parameters.PrescriptionMapNames);
            nameTemplate = parameters.PrescriptionMapNames;
            if (parameters.BiomassMapNames != null)
                biomassMaps = new BiomassMaps(parameters.BiomassMapNames);

            //open log file and write header
            modelCore.UI.WriteLine("   Opening harvest log file \"{0}\" ...", parameters.EventLog);
            try
            {
                log = Landis.Data.CreateTextFile(parameters.EventLog);
            }
            catch (Exception err)
            {
                string mesg = string.Format("{0}", err.Message);
                throw new System.ApplicationException(mesg);
            }
            log.AutoFlush = true;
            
            //include a column for each species in the species dictionary
            string species_header_names = "";
            int i = 0;
            for (i = 0; i < modelCore.Species.Count; i++) {
                species_header_names += "," + modelCore.Species[i].Name;
            }

            modelCore.UI.WriteLine("   Opening summary harvest log file \"{0}\" ...", parameters.SummaryLog);
            log.WriteLine("Time,ManagementArea,Prescription,StandMapCode,EventId,StandAge,StandRank,StandSiteCount,DamagedSites,MgBiomassRemoved,MgBioRemovedPerDamagedHa,CohortsDamaged,CohortsKilled{0}", species_header_names);

            try
            {
                summaryLog = Landis.Data.CreateTextFile(parameters.SummaryLog);
            }
            catch (Exception err)
            {
                string mesg = string.Format("{0}", err.Message);
                throw new System.ApplicationException(mesg);
            }
            summaryLog.AutoFlush = true;
            summaryLog.WriteLine("Time,ManagementArea,Prescription,TotalDamagedSites,TotalCohortsDamaged,TotalCohortsKilled{0}", species_header_names);

        }

        //---------------------------------------------------------------------

        public override void Run()
        {
            running = true;
            BaseHarvest.SiteVars.Prescription.ActiveSiteValues = null;
            SiteVars.BiomassRemoved.ActiveSiteValues = 0;
            SiteVars.CohortsPartiallyDamaged.ActiveSiteValues = 0;
            BaseHarvest.SiteVars.CohortsDamaged.ActiveSiteValues = 0;

            //harvest each management area in the list
            foreach (ManagementArea mgmtArea in managementAreas)
            {

                totalSites = new int[Prescription.Count];
                totalDamagedSites = new int[Prescription.Count];
                totalSpeciesCohorts = new int[Prescription.Count, modelCore.Species.Count];
                totalCohortsDamaged = new int[Prescription.Count];
                totalCohortsKilled = new int[Prescription.Count];


                mgmtArea.HarvestStands();
                //and record each stand that's been harvested

                foreach (Stand stand in mgmtArea)
                {
                    //modelCore.UI.WriteLine("   List of stands {0} ...", stand.MapCode);
                    if (stand.Harvested)
                        WriteLogEntry(mgmtArea, stand);

                }

                // Prevent establishment:
                foreach (Stand stand in mgmtArea)
                {

                    if (stand.Harvested && stand.LastPrescription.PreventEstablishment)
                    {

                        List<ActiveSite> sitesToDelete = new List<ActiveSite>();

                        foreach (ActiveSite site in stand)
                        {
                            if (SiteVars.CohortsPartiallyDamaged[site] > 0 || BaseHarvest.SiteVars.CohortsDamaged[site] > 0)
                            {
                                Landis.Library.Succession.Reproduction.PreventEstablishment(site);
                                sitesToDelete.Add(site);
                            }

                        }

                        foreach (ActiveSite site in sitesToDelete)
                        {
                            stand.DelistActiveSite(site);
                        }
                    }

                }

                // Write Summary Log File:
                foreach (AppliedPrescription aprescription in mgmtArea.Prescriptions)
                {
                    Prescription prescription = aprescription.Prescription;
                    string species_string = "";
                    foreach (ISpecies species in modelCore.Species)
                        species_string += ", " + totalSpeciesCohorts[prescription.Number, species.Index];

                    //summaryUI.WriteLine("Time,ManagementArea,Prescription,TotalDamagedSites,TotalCohortsDamaged,TotalCohortsKilled,{0}", species_header_names);
                    if (totalSites[prescription.Number] > 0)
                        summaryLog.WriteLine("{0},{1},{2},{3},{4},{5}{6}",
                            modelCore.CurrentTime,
                            mgmtArea.MapCode,
                            prescription.Name,
                            totalDamagedSites[prescription.Number],
                            totalCohortsDamaged[prescription.Number],
                            totalCohortsKilled[prescription.Number],
                            species_string);
                }
            }

            WritePrescriptionMap(modelCore.CurrentTime);
            if (biomassMaps != null)
                biomassMaps.WriteMap(modelCore.CurrentTime);

            running = false;
        }

        //---------------------------------------------------------------------

        // Event handler when a cohort is killed by an age-only disturbance.
        public void CohortKilledByAgeOnlyDisturbance(object                 sender,
                                                     Landis.Library.LeafBiomassCohorts.DeathEventArgs eventArgs)
        {
            // If this plug-in is not running, then some base disturbance
            // plug-in killed the cohort.
            if (! running)
                return;

            // If this plug-in is running, then the age-only disturbance must
            // be a cohort-selector from Base Harvest.
            SiteVars.BiomassRemoved[eventArgs.Site] += (int) (eventArgs.Cohort.LeafBiomass + eventArgs.Cohort.WoodBiomass);
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Writes an output map of prescriptions that harvested each active site.
        /// </summary>
        private void WritePrescriptionMap(int timestep)
        {
            string path = MapNames.ReplaceTemplateVars(nameTemplate, timestep);
            modelCore.UI.WriteLine("   Writing prescription map to {0} ...", path);
            using (IOutputRaster<ShortPixel> outputRaster = modelCore.CreateRaster<ShortPixel>(path, modelCore.Landscape.Dimensions))
            {
                ShortPixel pixel = outputRaster.BufferPixel;
                foreach (Site site in modelCore.Landscape.AllSites)
                {
                    if (site.IsActive)
                    {
                        Prescription prescription = BaseHarvest.SiteVars.Prescription[site];
                        if (prescription == null)
                            pixel.MapCode.Value = 1;
                        else
                            pixel.MapCode.Value = (short) (prescription.Number + 1);
                    }
                    else
                    {
                        //  Inactive site
                        pixel.MapCode.Value = 0;
                    }
                    outputRaster.WriteBufferPixel();
                }
            }
        }
        //---------------------------------------------------------------------

        public void WriteLogEntry(ManagementArea mgmtArea, Stand stand)
        {
            int damagedSites = 0;
            int cohortsDamaged = 0;
            int cohortsKilled = 0;
            int standPrescriptionNumber = 0;
            double biomassRemoved = 0.0;
            double biomassRemovedPerHa = 0.0;

            foreach (ActiveSite site in stand)
            {
                //set the prescription name for this site
                if (BaseHarvest.SiteVars.Prescription[site] != null)
                {
                    standPrescriptionNumber = BaseHarvest.SiteVars.Prescription[site].Number;
                    BaseHarvest.SiteVars.PrescriptionName[site] = BaseHarvest.SiteVars.Prescription[site].Name;
                    BaseHarvest.SiteVars.TimeOfLastEvent[site] = modelCore.CurrentTime;
                }

                cohortsDamaged += SiteVars.CohortsPartiallyDamaged[site];
                cohortsKilled += BaseHarvest.SiteVars.CohortsDamaged[site];


                if (SiteVars.CohortsPartiallyDamaged[site] > 0 || BaseHarvest.SiteVars.CohortsDamaged[site] > 0)
                {
                    damagedSites++;

                    //Conversion from [g m-2] to [Mg]
                    biomassRemoved += SiteVars.BiomassRemoved[site] / 100.0 * modelCore.CellArea;
                }
            }
            totalSites[standPrescriptionNumber] += stand.SiteCount;
            totalDamagedSites[standPrescriptionNumber] += damagedSites;
            totalCohortsDamaged[standPrescriptionNumber] += cohortsDamaged;
            totalCohortsKilled[standPrescriptionNumber] += cohortsKilled;


            //csv string for log file, contains species kill count
            string species_count = "";
            //if this is the right species match, add it's count to the csv string
            foreach (ISpecies species in modelCore.Species) {
                bool assigned = false;

                //loop through dictionary of species kill count
                foreach (KeyValuePair<string, int> kvp in stand.DamageTable) {
                    if (species.Name == kvp.Key) {
                        assigned = true;
                        species_count += ", " + kvp.Value;
                    }
                }
                if (!assigned) {
                    //put a 0 there if it's not assigned (because none were found in the dictionary)
                    species_count += ",0";
                }
            }

            //now that the damage table for this stand has been recorded, clear it!!
            stand.ClearDamageTable();

            //write to log file:
            biomassRemovedPerHa = biomassRemoved / (double)damagedSites / modelCore.CellArea;

            if (biomassRemoved <= 0.0)
                return;

            log.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9:0.000},{10:0.000},{11},{12}{13}",
                          modelCore.CurrentTime,
                          mgmtArea.MapCode,
                          stand.PrescriptionName,
                          stand.MapCode,
                          stand.EventId,
                          stand.Age,
                          stand.HarvestedRank,
                          stand.SiteCount,
                          damagedSites,
                          biomassRemoved,  // Mg
                          biomassRemovedPerHa, // Mg/ha
                          cohortsDamaged,
                          cohortsKilled,
                          species_count);
        }
    }
}
