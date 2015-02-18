// This file is part of the Base Harvest extension for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/trunk/base-harvest/trunk/

using Edu.Wisc.Forest.Flel.Util;
using Landis.Core;
using Landis.Library.HarvestManagement;
using Landis.Library.Succession;
using Landis.SpatialModeling;
using System.Collections.Generic;
using System.IO;
using System;

namespace Landis.Extension.BaseHarvest
{
    public class PlugIn
        : HarvestExtensionMain
    {
        public static readonly string ExtensionName = "Base Harvest";

        private IManagementAreaDataset managementAreas;
        private PrescriptionMaps prescriptionMaps;
        private StreamWriter log;
        private StreamWriter summaryLog;
        private static int event_id;
        private static double current_rank;     //need a global to keep track of the current stand's rank.  just for log file.

        int[] totalSites;
        int[] totalDamagedSites;
        int[,] totalSpeciesCohorts;

        private IInputParameters parameters;
        private static ICore modelCore;


        //---------------------------------------------------------------------

        public PlugIn()
            : base(ExtensionName)
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

        public override void LoadParameters(string dataFile, ICore mCore)
        {
            modelCore = mCore;
            Landis.Library.HarvestManagement.Main.InitializeLib(modelCore);
            InputParametersParser parser = new InputParametersParser(mCore.Species);
            parameters = Landis.Data.Load<IInputParameters>(dataFile, parser);
            if (parser.RoundedRepeatIntervals.Count > 0)
            {
                PlugIn.ModelCore.UI.WriteLine("NOTE: The following repeat intervals were rounded up to");
                PlugIn.ModelCore.UI.WriteLine("      ensure they were multiples of the harvest timestep:");
                PlugIn.ModelCore.UI.WriteLine("      File: {0}", dataFile);
                foreach (RoundedInterval interval in parser.RoundedRepeatIntervals)
                    PlugIn.ModelCore.UI.WriteLine("      At line {0}, the interval {1} rounded up to {2}",
                                 interval.LineNumber,
                                 interval.Original,
                                 interval.Adjusted);
            }
            if (parser.ParserNotes.Count > 0)
            {
                foreach (List<string> nList in parser.ParserNotes)
                {
                    foreach (string nLine in nList)
                    {
                        PlugIn.ModelCore.UI.WriteLine(nLine);
                    }
                }
            }
        }
        //---------------------------------------------------------------------

        public override void Initialize()
        {
            //initialize event id
            event_id = 1;
            Timestep = parameters.Timestep;
            managementAreas = parameters.ManagementAreas;
            PlugIn.ModelCore.UI.WriteLine("   Reading management-area map {0} ...", parameters.ManagementAreaMap);
            ManagementAreas.ReadMap(parameters.ManagementAreaMap, managementAreas);

            //readMap reads the stand map and adds all the stands to a management area
            PlugIn.ModelCore.UI.WriteLine("   Reading stand map {0} ...", parameters.StandMap);
            Stands.ReadMap(parameters.StandMap);

            //finish each managementArea's initialization
            //after reading the stand map, finish the initializations
            foreach (ManagementArea mgmtArea in managementAreas)
                mgmtArea.FinishInitialization();

            prescriptionMaps = new PrescriptionMaps(parameters.PrescriptionMapNames);

            //open log file and write header
            PlugIn.ModelCore.UI.WriteLine("   Opening harvest log file \"{0}\" ...", parameters.EventLog);

            try {
                log = Landis.Data.CreateTextFile(parameters.EventLog);
            }
            catch (Exception err) {
                string mesg = string.Format("{0}", err.Message);
                throw new System.ApplicationException(mesg);
            }
            log.AutoFlush = true;
            
            //include a column for each species in the species dictionary
            string species_header_names = "";
            int i = 0;
            for (i = 0; i < PlugIn.ModelCore.Species.Count; i++) {
                species_header_names += PlugIn.ModelCore.Species[i].Name + ",";
            }

            log.WriteLine("Time,ManagementArea,Prescription,Stand,EventId,StandAge,StandRank,NumberOfSites,HarvestedSites,CohortsKilled,{0}", species_header_names);

            PlugIn.ModelCore.UI.WriteLine("   Opening summary harvest log file \"{0}\" ...", parameters.SummaryLog);

            try {
                summaryLog = Landis.Data.CreateTextFile(parameters.SummaryLog);
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
            SiteVars.ReInitialize();
            SiteVars.Prescription.ActiveSiteValues = null;
            SiteVars.CohortsDamaged.ActiveSiteValues = 0;


            //harvest each management area in the list
            foreach (ManagementArea mgmtArea in managementAreas) {

                totalSites = new int[Prescription.Count];
                totalDamagedSites = new int[Prescription.Count];
                totalSpeciesCohorts = new int[Prescription.Count, PlugIn.ModelCore.Species.Count];

                mgmtArea.HarvestStands();
                //and record each stand that's been harvested

                foreach (Stand stand in mgmtArea) {
                    if (stand.Harvested)
                        WriteLogEntry(mgmtArea, stand);

                }

                // updating for preventing establishment
                foreach (Stand stand in mgmtArea) 
                {
                    if (stand.Harvested && stand.LastPrescription.PreventEstablishment) 
                    {

                        List<ActiveSite> sitesToDelete = new List<ActiveSite>();

                        foreach (ActiveSite site in stand) {
                            if (SiteVars.CohortsDamaged[site] > 0)
                            {
                                Reproduction.PreventEstablishment(site);
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
                    foreach (ISpecies species in PlugIn.ModelCore.Species)
                         species_string += ", " + totalSpeciesCohorts[prescription.Number, species.Index];

                    if(totalSites[prescription.Number] > 0)
                        summaryLog.WriteLine("{0},{1},{2},{3}{4}",
                            PlugIn.ModelCore.CurrentTime,
                            mgmtArea.MapCode,
                            prescription.Name,
                            totalDamagedSites[prescription.Number],
                            species_string);


                }

            }
            prescriptionMaps.WriteMap(PlugIn.ModelCore.CurrentTime);


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
                    SiteVars.TimeOfLastEvent[site] = PlugIn.ModelCore.CurrentTime;
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

            foreach (ISpecies species in PlugIn.ModelCore.Species)
            {
                int cohortCount = stand.DamageTable[species];
                species_count += string.Format("{0},", cohortCount);
                totalSpeciesCohorts[standPrescriptionNumber, species.Index] += cohortCount;
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
                          PlugIn.ModelCore.CurrentTime, mgmtArea.MapCode, stand.PrescriptionName, stand.MapCode, stand.EventId,
                          stand.Age, stand.HarvestedRank, stand.SiteCount, damagedSites, cohortsDamaged, species_count);


        }
        //---------------------------------------------------------------------
        //public void Mark(ManagementArea mgmtArea, Stand stand) {
        //} //

    }
}
