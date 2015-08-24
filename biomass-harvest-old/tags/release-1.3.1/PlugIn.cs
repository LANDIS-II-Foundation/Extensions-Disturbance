using Edu.Wisc.Forest.Flel.Util;
using Landis.Harvest;
using Landis.Landscape;
using Landis.PlugIns;
using Landis.Species;
using Landis.Biomass;
using Landis.RasterIO;

using System.Collections.Generic;
using System.IO;
using System;

using BaseHarvest = Landis.Harvest;

namespace Landis.Extension.BiomassHarvest
{
    public class PlugIn
        : Landis.PlugIns.PlugIn, Landis.PlugIns.ICleanUp
    {
        public static readonly PlugInType Type = new PlugInType("disturbance:harvest");
        private IManagementAreaDataset managementAreas;
        private BiomassMaps biomassMaps;
        private string nameTemplate;
        private StreamWriter log;
        private StreamWriter summaryLog;
        private static bool running;

        int[] totalSites;
        int[] totalDamagedSites;
        int[,] totalSpeciesCohorts;
        int[] totalCohortsKilled;
        int[] totalCohortsDamaged;



        //---------------------------------------------------------------------

        public PlugIn()
            : base("Biomass Harvest", Type)
        {
        }

        //---------------------------------------------------------------------

        public override void Initialize(string        dataFile,
                                        PlugIns.ICore modelCore)
        {
            // Initialize the Base Harvest's Model.Core property.
            // HACK: Because that property is internal, we must
            // call the Initialize method on an instance of Base
            // Harvest's PlugIn class.  But don't want that
            // Initialize method parsing the data file.  So we
            // pass in a null string to force an exception to
            // be raised; hence aborting the initialization at
            // a point that's acceptable.
            PlugIns.PlugIn baseHarvest = new BaseHarvest.PlugIn();
            try {
                baseHarvest.Initialize(null, modelCore);
            }
            catch (System.ArgumentNullException) {
                // ignore
            }

            Model.Core = modelCore;
            PartialHarvestDisturbance.Initialize();
            SiteVars.Initialize();

            // Add local event handler for cohorts death due to age-only
            // disturbances.
            Cohort.AgeOnlyDeathEvent += CohortKilledByAgeOnlyDisturbance;

            ParametersParser parser = new ParametersParser(Model.Core.Species,
                                                           Model.Core.StartTime,
                                                           Model.Core.EndTime);

            BaseHarvest.IParameters baseParameters = Landis.Data.Load<BaseHarvest.IParameters>(dataFile, parser);
            IParameters parameters = baseParameters as IParameters;
            if (parser.RoundedRepeatIntervals.Count > 0) {
                UI.WriteLine("NOTE: The following repeat intervals were rounded up to");
                UI.WriteLine("      ensure they were multiples of the harvest timestep:");
                UI.WriteLine("      File: {0}", dataFile);
                foreach (RoundedInterval interval in parser.RoundedRepeatIntervals)
                    UI.WriteLine("      At line {0}, the interval {1} rounded up to {2}",
                                 interval.LineNumber,
                                 interval.Original,
                                 interval.Adjusted);
            }
            //set timestep
            Timestep = parameters.Timestep;
            //set management areas
            managementAreas = parameters.ManagementAreas;
            UI.WriteLine("Reading management-area map {0} ...", parameters.ManagementAreaMap);
            //read management area map
            ManagementAreas.ReadMap(parameters.ManagementAreaMap,
                                    managementAreas);

            UI.WriteLine("Reading stand map {0} ...", parameters.StandMap);
            //readMap reads the stand map and adds all the stands to a management area
            Stands.ReadMap(parameters.StandMap);
            //finish each managementArea's initialization
            foreach (ManagementArea mgmtArea in managementAreas)
                //after reading the stand map, finish the initializations
                mgmtArea.FinishInitialization();

            //prescriptionMaps = new PrescriptionMaps(parameters.PrescriptionMapNames);
            nameTemplate = parameters.PrescriptionMapNames;

            if (parameters.BiomassMapNames != null)
                biomassMaps = new BiomassMaps(parameters.BiomassMapNames);

            //open log file and write header
            UI.WriteLine("Opening harvest log file \"{0}\" ...", parameters.EventLog);
            log = Data.CreateTextFile(parameters.EventLog);
            log.AutoFlush = true;
            //include a column for each species in the species dictionary
            string species_header_names = "";
            int i = 0;
            for (i = 0; i < Model.Core.Species.Count; i++) {
                species_header_names += "," + Model.Core.Species[i].Name;
            }

            log.WriteLine("Time,ManagementArea,Prescription,StandMapCode,EventId,StandAge,StandRank,StandSiteCount,DamagedSites,MgBiomassRemoved,MgBioRemovedPerDamagedHa,CohortsDamaged,CohortsKilled{0}", species_header_names);

            UI.WriteLine("Opening summary harvest log file \"{0}\" ...", parameters.SummaryLog);

            summaryLog = Data.CreateTextFile(parameters.SummaryLog);
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
            foreach (ManagementArea mgmtArea in managementAreas) {

                totalSites          = new int[Prescription.Count];
                totalDamagedSites   = new int[Prescription.Count];
                totalSpeciesCohorts = new int[Prescription.Count, Model.Core.Species.Count];
                totalCohortsDamaged = new int[Prescription.Count];
                totalCohortsKilled  = new int[Prescription.Count];


                mgmtArea.HarvestStands();
                //and record each stand that's been harvested

                foreach (Stand stand in mgmtArea) {
                    if (stand.Harvested)
                        WriteLogEntry(mgmtArea, stand);

                }

                // Prevent establishment:
                foreach (Stand stand in mgmtArea) {
                    if (stand.Harvested && stand.LastPrescription.PreventEstablishment) {

                        List<ActiveSite> sitesToDelete = new List<ActiveSite>();

                        foreach (ActiveSite site in stand)
                        {
                            if (SiteVars.CohortsPartiallyDamaged[site] > 0 || BaseHarvest.SiteVars.CohortsDamaged[site] > 0)
                            {
                                Succession.Reproduction.PreventEstablishment(site);
                                sitesToDelete.Add(site);
                            }

                        }

                        foreach (ActiveSite site in sitesToDelete) {
                            stand.DelistActiveSite(site);
                        }
                    }

                }

                // Write Summary Log File:
                foreach (AppliedPrescription aprescription in mgmtArea.Prescriptions)
                {
                    Prescription prescription = aprescription.Prescription;
                    string species_string = "";
                    foreach (ISpecies species in Model.Core.Species)
                         species_string += ", " + totalSpeciesCohorts[prescription.Number, species.Index];

                    //summaryLog.WriteLine("Time,ManagementArea,Prescription,TotalDamagedSites,TotalCohortsDamaged,TotalCohortsKilled,{0}", species_header_names);
                    if(totalSites[prescription.Number] > 0)
                        summaryLog.WriteLine("{0},{1},{2},{3},{4},{5}{6}",
                            Model.Core.CurrentTime,
                            mgmtArea.MapCode,
                            prescription.Name,
                            totalDamagedSites[prescription.Number],
                            totalCohortsDamaged[prescription.Number],
                            totalCohortsKilled[prescription.Number],
                            species_string);


                }
            }

            WritePrescriptionMap(Model.Core.CurrentTime);
            if (biomassMaps != null)
                biomassMaps.WriteMap(Model.Core.CurrentTime);

            running = false;
        }

        //---------------------------------------------------------------------

        // Event handler when a cohort is killed by an age-only disturbance.
        public static void CohortKilledByAgeOnlyDisturbance(object                 sender,
                                                     Biomass.DeathEventArgs eventArgs)
        {

            // If this plug-in is not running, then some base disturbance
            // plug-in killed the cohort.
            if (! running)
                return;

            // If this plug-in is running, then the age-only disturbance must
            // be a cohort-selector from Base Harvest.

            int reduction = eventArgs.Cohort.Biomass;
            SiteVars.BiomassRemoved[eventArgs.Site] += reduction;

            //UI.WriteLine("Cohort Biomass removed={0:0.0}; Total Killed={1:0.0}.", reduction, SiteVars.BiomassRemoved[eventArgs.Site]);
            //SiteVars.CohortsPartiallyDamaged[eventArgs.Site]++;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Writes an output map of prescriptions that harvested each active site.
        /// </summary>
        private void WritePrescriptionMap(int timestep)
        {
            string path = MapNames.ReplaceTemplateVars(nameTemplate, timestep);
            using (IOutputRaster<PrescriptionPixel> map = CreateMap(path)) {
                PrescriptionPixel pixel = new PrescriptionPixel();
                foreach (Site site in Model.Core.Landscape.AllSites) {
                    if (site.IsActive) {
                        Prescription prescription = BaseHarvest.SiteVars.Prescription[site];
                        if (prescription == null)
                            pixel.Band0 = 1;
                        else
                            pixel.Band0 = (byte) (prescription.Number + 1);
                    }
                    else {
                        //  Inactive site
                        pixel.Band0 = 0;
                    }
                    map.WritePixel(pixel);
                }
            }
        }

        //---------------------------------------------------------------------

        private IOutputRaster<PrescriptionPixel> CreateMap(string path)
        {
            UI.WriteLine("Writing prescription map to {0} ...", path);
            return Model.Core.CreateRaster<PrescriptionPixel>(path,
                                                              Model.Core.Landscape.Dimensions,
                                                              Model.Core.LandscapeMapMetadata);
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

            foreach (ActiveSite site in stand) {
                //set the prescription name for this site
                if (BaseHarvest.SiteVars.Prescription[site] != null)
                {
                    standPrescriptionNumber = BaseHarvest.SiteVars.Prescription[site].Number;
                    BaseHarvest.SiteVars.PrescriptionName[site] = BaseHarvest.SiteVars.Prescription[site].Name;
                    BaseHarvest.SiteVars.TimeOfLastEvent[site] = Model.Core.CurrentTime;
                }

                cohortsDamaged += SiteVars.CohortsPartiallyDamaged[site];
                cohortsKilled += BaseHarvest.SiteVars.CohortsDamaged[site];


                if (SiteVars.CohortsPartiallyDamaged[site] > 0 ||  BaseHarvest.SiteVars.CohortsDamaged[site] > 0)
                {
                    damagedSites++;

                    //Conversion from [g m-2] to [Mg]
                    biomassRemoved += SiteVars.BiomassRemoved[site] / 100.0 * Model.Core.CellArea;
                }
            }

            totalSites[standPrescriptionNumber] += stand.SiteCount;
            totalDamagedSites[standPrescriptionNumber] += damagedSites;
            totalCohortsDamaged[standPrescriptionNumber] += cohortsDamaged;
            totalCohortsKilled[standPrescriptionNumber] += cohortsKilled;


            //csv string for log file, contains species kill count
            string species_count = "";
            //if this is the right species match, add it's count to the csv string
            foreach (ISpecies species in Model.Core.Species) {
                bool assigned = false;

                //loop through dictionary of species kill count
                foreach (KeyValuePair<string, int> kvp in stand.DamageTable) {
                    if (species.Name == kvp.Key) {
                        assigned = true;
                        species_count += "," + kvp.Value;
                        totalSpeciesCohorts[standPrescriptionNumber, species.Index] += kvp.Value;
                    }
                }
                if (!assigned) {
                    //put a 0 there if it's not assigned (because none were found in the dictionary)
                    species_count += ",0";
                    totalSpeciesCohorts[standPrescriptionNumber, species.Index] += 0;
                }
            }

            //now that the damage table for this stand has been recorded, clear it!!
            stand.ClearDamageTable();

            //write to log file:
            biomassRemovedPerHa = biomassRemoved / (double) damagedSites / Model.Core.CellArea;

            if(biomassRemoved <= 0.0)
                return;

            log.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9:0.000},{10:0.000},{11},{12}{13}",
                          Model.Core.CurrentTime,
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
        //---------------------------------------------------------------------

        void PlugIns.ICleanUp.CleanUp()
        {
            if (log != null) {
                log.Close();
                log = null;
            }
        }
    }
}
