using Edu.Wisc.Forest.Flel.Util;

using Landis.Landscape;
using Landis.PlugIns;

using System.Collections.Generic;
using System.IO;

namespace Landis.Harvest
{
    public class PlugIn
        : Landis.PlugIns.PlugIn, Landis.PlugIns.ICleanUp
    {
        public static readonly PlugInType Type = new PlugInType("disturbance:harvest");
        private IManagementAreaDataset managementAreas;
        private PrescriptionMaps prescriptionMaps;
        private StreamWriter log;

        //---------------------------------------------------------------------

        public PlugIn()
            : base("Harvest", Type)
        {
        }

        //---------------------------------------------------------------------

        public override void Initialize(string        dataFile,
                                        PlugIns.ICore modelCore)
        {
            Model.Core = modelCore;
            SiteVars.Initialize();

            ParametersParser parser = new ParametersParser(Model.Core.StartTime,
                                                           Model.Core.EndTime);
            IParameters parameters = Landis.Data.Load<IParameters>(dataFile, parser);

            Timestep = parameters.Timestep;

            managementAreas = parameters.ManagementAreas;
            UI.WriteLine("Reading management-area map {0} ...",
                         parameters.ManagementAreaMap);
            ManagementAreas.ReadMap(parameters.ManagementAreaMap,
                                    managementAreas);

            UI.WriteLine("Reading stand map {0} ...", parameters.StandMap);
            Stands.ReadMap(parameters.StandMap);

            foreach (ManagementArea mgmtArea in managementAreas)
                mgmtArea.FinishInitialization();

            prescriptionMaps = new PrescriptionMaps(parameters.PrescriptionMapNames);

            UI.WriteLine("Opening harvest log file \"{0}\" ...", parameters.EventLog);
            log = Data.CreateTextFile(parameters.EventLog);
            log.AutoFlush = true;
            log.WriteLine("Time,Stand,Total Sites,Damaged Sites,Cohorts Killed");
        }

        //---------------------------------------------------------------------

        public override void Run()
        {
            SiteVars.Prescription.ActiveSiteValues = null;
            SiteVars.CohortsKilled.ActiveSiteValues = 0;

            foreach (ManagementArea mgmtArea in managementAreas) {
                mgmtArea.HarvestStands();
                
                foreach (Stand stand in mgmtArea) {
                    if (stand.Harvested)
                        WriteLogEntry(stand);
                }
            }

            prescriptionMaps.WriteMap(Model.Core.CurrentTime);
        }

        //---------------------------------------------------------------------

        public void WriteLogEntry(Stand stand)
        {
            int totalSites = 0;
            int damagedSites = 0;
            int cohortsKilled = 0;
            foreach (ActiveSite site in stand) {
                totalSites++;
                int cohortsKilledAtSite = SiteVars.CohortsKilled[site];
                cohortsKilled += cohortsKilledAtSite;
                if (cohortsKilledAtSite > 0)
                    damagedSites++;
            }

            log.WriteLine("{0},{1},{2},{3},{4}",
                          Model.Core.CurrentTime, stand.MapCode,
                          totalSites, damagedSites, cohortsKilled);
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
