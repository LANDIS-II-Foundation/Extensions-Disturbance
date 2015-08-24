//  Copyright 2009 Portland State University, University of Wisconsin
//  Authors:  Robert M. Scheller and others
//  License:  Available at
//  http://www.landis-ii.org/developers/LANDIS-IISourceCodeLicenseAgreement.pdf

using Edu.Wisc.Forest.Flel.Util;

using Landis.Landscape;
using Landis.PlugIns;
using Landis.Species;
using System.Collections.Generic;
using System.IO;
using System;

namespace Landis.Harvest
{
    public class PlugIn
        : Landis.PlugIns.PlugIn, Landis.PlugIns.ICleanUp
    {
        public static readonly PlugInType Type = new PlugInType("disturbance:harvest");
        private IManagementAreaDataset managementAreas;
        private PrescriptionMaps prescriptionMaps;
        private StreamWriter log;
        private StreamWriter summaryLog;
        private static int event_id;
        private static double current_rank;     //need a global to keep track of the current stand's rank.  just for log file.

        int[] totalSites;
        int[] totalDamagedSites;
        int[,] totalSpeciesCohorts;

        //---------------------------------------------------------------------

        public PlugIn()
            : base("Base Harvest", Type)
        {
        }

        //---------------------------------------------------------------------

        public override void Initialize(string        dataFile,
                                        PlugIns.ICore modelCore)
        {
            //initialize event id
            event_id = 1;

            Model.Core = modelCore;
            SiteVars.Initialize();
            ParametersParser parser = new ParametersParser(Model.Core.Species,
                                                           Model.Core.StartTime,
                                                           Model.Core.EndTime);

            IParameters parameters = Landis.Data.Load<IParameters>(dataFile, parser);
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
            //after reading the stand map, finish the initializations
            foreach (ManagementArea mgmtArea in managementAreas)
                mgmtArea.FinishInitialization();

            prescriptionMaps = new PrescriptionMaps(parameters.PrescriptionMapNames);

            //open log file and write header
            UI.WriteLine("Opening harvest log file \"{0}\" ...", parameters.EventLog);

            try {
                log = Data.CreateTextFile(parameters.EventLog);
            }
            catch (Exception err) {
                string mesg = string.Format("{0}", err.Message);
                throw new System.ApplicationException(mesg);
            }

            //log = Data.CreateTextFile(parameters.EventLog);
            log.AutoFlush = true;
            //include a column for each species in the species dictionary
            string species_header_names = "";
            int i = 0;
            for (i = 0; i < Model.Core.Species.Count; i++) {
                species_header_names += Model.Core.Species[i].Name + ",";
            }

            log.WriteLine("Time,ManagementArea,Prescription,Stand,Event Id,Stand Age,Stand Rank,NumberOfSites,HarvestedSites,CohortsDamaged,{0}", species_header_names);

            UI.WriteLine("Opening summary harvest log file \"{0}\" ...", parameters.SummaryLog);

            try {
                summaryLog = Data.CreateTextFile(parameters.SummaryLog);
            }
            catch (Exception err) {
                string mesg = string.Format("{0}", err.Message);
                throw new System.ApplicationException(mesg);
            }
            summaryLog.AutoFlush = true;

            summaryLog.WriteLine("Time,ManagementArea,Prescription,HarvestedSites,{0}", species_header_names);


        }

        //---------------------------------------------------------------------

        public override void Run()
        {
            SiteVars.Prescription.ActiveSiteValues = null;
            SiteVars.CohortsDamaged.ActiveSiteValues = 0;


            //harvest each management area in the list
            foreach (ManagementArea mgmtArea in managementAreas) {

                totalSites = new int[Prescription.Count];
                totalDamagedSites = new int[Prescription.Count];
                totalSpeciesCohorts = new int[Prescription.Count, Model.Core.Species.Count];

                mgmtArea.HarvestStands();
                //and record each stand that's been harvested

                foreach (Stand stand in mgmtArea) {
                    if (stand.Harvested)
                        WriteLogEntry(mgmtArea, stand);

                }

                // updating for preventing establishment
                foreach (Stand stand in mgmtArea) {
                    if (stand.Harvested && stand.LastPrescription.PreventEstablishment) {

                        List<ActiveSite> sitesToDelete = new List<ActiveSite>();

                        foreach (ActiveSite site in stand) {
                            if (SiteVars.CohortsDamaged[site] > 0)
                            {
                                Succession.Reproduction.PreventEstablishment(site);
                                sitesToDelete.Add(site);
                            }

                        }

                        foreach (ActiveSite site in sitesToDelete) {
                            stand.DelistActiveSite(site);
                        }
                    }

                } // foreach (Stand stand in mgmtArea)

                foreach (AppliedPrescription aprescription in mgmtArea.Prescriptions)
                {
                    Prescription prescription = aprescription.Prescription;
                    string species_string = "";
                    foreach (ISpecies species in Model.Core.Species)
                         species_string += ", " + totalSpeciesCohorts[prescription.Number, species.Index];

                    if(totalSites[prescription.Number] > 0)
                        summaryLog.WriteLine("{0},{1},{2},{3}{4}",
                            Model.Core.CurrentTime,
                            mgmtArea.MapCode,
                            prescription.Name,
                            totalDamagedSites[prescription.Number],
                            species_string);


                }

            }
            prescriptionMaps.WriteMap(Model.Core.CurrentTime);


        }

        //---------------------------------------------------------------------

        public static int EventId {
            get {
                return event_id;
            }

            set {
                event_id = value;
            }
        }

        //---------------------------------------------------------------------

        public static double CurrentRank {
            get {
                return current_rank;
            }

            set {
                current_rank = value;
            }
        }

        //---------------------------------------------------------------------
        public void WriteLogEntry(ManagementArea mgmtArea, Stand stand)
        {
            int damagedSites = 0;
            int cohortsDamaged = 0;
            int standPrescriptionNumber = 0;

            foreach (ActiveSite site in stand) {
                //set the prescription name for this site
                if (SiteVars.Prescription[site] != null)
                {
                    standPrescriptionNumber = SiteVars.Prescription[site].Number;
                    SiteVars.PrescriptionName[site] = SiteVars.Prescription[site].Name;
                    SiteVars.TimeOfLastEvent[site] = Model.Core.CurrentTime;
                }
                int cohortsDamagedAtSite = SiteVars.CohortsDamaged[site];
                cohortsDamaged += cohortsDamagedAtSite;
                if (cohortsDamagedAtSite > 0) {
                    damagedSites++;
                }
            }


            totalSites[standPrescriptionNumber] += stand.SiteCount;
            totalDamagedSites[standPrescriptionNumber] += damagedSites;

            //csv string for log file, contains species kill count
            string species_count = "";

            //if this is the right species match, add it's count to the csv string
            foreach (ISpecies species in Model.Core.Species)
            {
                bool assigned = false;

                //loop through dictionary of species kill count
                foreach (KeyValuePair<string, int> kvp in stand.DamageTable) {
                    if (species.Name == kvp.Key) {
                        assigned = true;
                        species_count += kvp.Value + ",";
                        totalSpeciesCohorts[standPrescriptionNumber, species.Index] += kvp.Value;
                    }
                }
                if (!assigned) {
                    //put a 0 there if it's not assigned (because none were found in the dictionary)
                    species_count += "0,";
                }
            }

            //now that the damage table for this stand has been recorded, clear it!!
            stand.ClearDamageTable();

            //write to log file:
                //current time
                //management area's map code
                //the prescription that caused this harvest
                //stand's map code
                //stand's age
                //stand's current rank
                //total sites in the stand
                //damaged sites from this stand
                //cohorts killed in this stand, by this harvest
            //and only record stands where a site has been damaged
            log.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",
                          Model.Core.CurrentTime, mgmtArea.MapCode, stand.PrescriptionName, stand.MapCode, stand.EventId,
                          stand.Age, stand.HarvestedRank, stand.SiteCount, damagedSites, cohortsDamaged, species_count);


        }
        //---------------------------------------------------------------------
        public void Mark(ManagementArea mgmtArea, Stand stand) {
        } //

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
