using Edu.Wisc.Forest.Flel.Util;
using Landis.Harvest;
using Landis.Landscape;
using Landis.PlugIns;
using Landis.Species;
using System.Collections.Generic;
using System.IO;

using BaseHarvest = Landis.Harvest;

namespace Landis.Extension.LeafBiomassHarvest
{
    public class PlugIn
        : Landis.PlugIns.PlugIn, Landis.PlugIns.ICleanUp
    {
        public static readonly PlugInType Type = new PlugInType("disturbance:harvest");
        private IManagementAreaDataset managementAreas;
        private PrescriptionMaps prescriptionMaps;        
        private BiomassMaps biomassMaps;        
        private StreamWriter log;
        private bool running;

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
            Biomass.Cohort.AgeOnlyDeathEvent += CohortKilledByAgeOnlyDisturbance;
            
            InputParametersParser parser = new InputParametersParser(Model.Core.Species,
                                                           Model.Core.StartTime,
                                                           Model.Core.EndTime);
            
            BaseHarvest.IParameters baseParameters = Landis.Data.Load<BaseHarvest.IParameters>(dataFile, parser);
            IInputParameters parameters = baseParameters as IInputParameters;
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
                species_header_names += Model.Core.Species[i].Name + ",";
            }
            
            log.WriteLine("Time,Management Area,Prescription,Stand,Event Id,Stand Age,Stand Rank,Total Sites,Damaged Sites,Cohorts Killed,{0}", species_header_names);
            
        }

        //---------------------------------------------------------------------

        public override void Run()
        {
            running = true;

            BaseHarvest.SiteVars.Prescription.ActiveSiteValues = null;
            //BaseHarvest.SiteVars.CohortsKilled.ActiveSiteValues = 0;
            SiteVars.BiomassRemoved.ActiveSiteValues = 0;

            //harvest each management area in the list
            foreach (ManagementArea mgmtArea in managementAreas) {
                mgmtArea.HarvestStands();
                //and record each stand that's been harvested
                foreach (Stand stand in mgmtArea) {
                    if (stand.Harvested) {
                        WriteLogEntry(mgmtArea, stand);
                    }
                }
            }
            prescriptionMaps.WriteMap(Model.Core.CurrentTime);
            if (biomassMaps != null)
                biomassMaps.WriteMap(Model.Core.CurrentTime);

            running = false;
        }

        //---------------------------------------------------------------------

        // Event handler when a cohort is killed by an age-only disturbance.
        public void CohortKilledByAgeOnlyDisturbance(object                 sender,
                                                     Biomass.DeathEventArgs eventArgs)
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

        public void WriteLogEntry(ManagementArea mgmtArea, Stand stand)
        {
            int damagedSites = 0;
            int cohortsKilled = 0;
            foreach (ActiveSite site in stand) {
                //set the prescription name for this site
                if (BaseHarvest.SiteVars.Prescription[site] != null) 
                {
                    BaseHarvest.SiteVars.PrescriptionName[site] = BaseHarvest.SiteVars.Prescription[site].Name;
                    BaseHarvest.SiteVars.TimeOfLastEvent[site] = Model.Core.CurrentTime;
                }
                int cohortsKilledAtSite = BaseHarvest.SiteVars.CohortsDamaged[site];
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
            //if (damagedSites > 0) {
                log.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",
                          Model.Core.CurrentTime, mgmtArea.MapCode, stand.PrescriptionName, stand.MapCode, stand.EventId,
                          stand.Age, stand.HarvestedRank, stand.SiteCount, damagedSites, cohortsKilled, species_count);
            //}
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
