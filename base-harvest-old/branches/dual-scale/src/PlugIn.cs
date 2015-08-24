using Edu.Wisc.Forest.Flel.Util;
using Landis.PlugIns;
using Landis.Species;
using System.Collections.Generic;
using System.IO;
using Wisc.Flel.GeospatialModeling.Landscapes.DualScale;

namespace Landis.Harvest
{
    public class PlugIn
        : Landis.PlugIns.PlugIn, Landis.PlugIns.ICleanUp
    {
        public static readonly PlugInType Type = new PlugInType("disturbance:harvest");
        private IManagementAreaDataset managementAreas;
        private PrescriptionMaps prescriptionMaps;
        private StreamWriter log;
        private static int event_id;
        private static double current_rank;     //need a global to keep track of the current stand's rank.  just for log file.

        //---------------------------------------------------------------------

        public PlugIn()
            : base("Harvest", Type)
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
            foreach (ManagementArea mgmtArea in managementAreas)
                //after reading the stand map, finish the initializations
                mgmtArea.FinishInitialization();

            prescriptionMaps = new PrescriptionMaps(parameters.PrescriptionMapNames);

            //open log file and write header
            UI.WriteLine("Opening harvest log file \"{0}\" ...", parameters.EventLog);
            log = Data.CreateTextFile(parameters.EventLog);
            log.AutoFlush = true;
            //include a column for each species in the species dictionary
            string species_header_names = "";
            int i = 0;
            for (i = 0; i < Model.Core.Species.Count; i++) {
                species_header_names += Model.Core.Species[i].Name + ",";
            }

            log.WriteLine("Time,Management Area,Prescription,Stand,Event Id,Stand Age,Stand Rank,Total Sites,Damaged Sites,Cohorts Killed,{0}", species_header_names);

        }

        //---------------------------------------------------------------------

        public override void Run()
        {
            SiteVars.Prescription.ActiveSiteValues = null;
            SiteVars.CohortsKilled.ActiveSiteValues = 0;

            //harvest each management area in the list
            foreach (ManagementArea mgmtArea in managementAreas) 
            {
                UI.WriteLine("      Harvest running in management area {0}.", mgmtArea.MapCode);
                mgmtArea.HarvestStands();
                //and record each stand that's been harvested
                foreach (Stand stand in mgmtArea) {
                    if (stand.Harvested) {
                        WriteLogEntry(mgmtArea, stand);
                    }
                }
            }
            UI.WriteLine("      Finished harvesting stands.");
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
            int cohortsKilled = 0;
            foreach (ActiveSite site in stand) {
                //set the prescription name for this site
                if (SiteVars.Prescription[site] != null) {
                    //SiteVars.PrescriptionName[site] = SiteVars.Prescription[site].Name;
                    SiteVars.TimeOfLastEvent[site] = Model.Core.CurrentTime;
                }
                int cohortsKilledAtSite = SiteVars.CohortsKilled[site];
                cohortsKilled += cohortsKilledAtSite;
                if (cohortsKilledAtSite > 0) {
                    damagedSites++;
                }
            }

            //csv string for log file, contains species kill count
            string species_count = "";
            //if this is the right species match, add it's count to the csv string
            foreach (ISpecies species in Model.Core.Species) {
                bool assigned = false;
                //loop through dictionary of species kill count
                foreach (KeyValuePair<string, int> kvp in stand.DamageTable) {
                    if (species.Name == kvp.Key) {
                        assigned = true;
                        species_count += kvp.Value + ",";
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
            if (damagedSites > 0) {
                log.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",
                          Model.Core.CurrentTime, mgmtArea.MapCode, stand.PrescriptionName, stand.MapCode, stand.EventId,
                          stand.Age, stand.HarvestedRank, stand.SiteCount, damagedSites, cohortsKilled, species_count);
            }
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
