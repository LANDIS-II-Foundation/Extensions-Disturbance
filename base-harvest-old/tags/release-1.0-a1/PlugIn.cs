using Edu.Wisc.Forest.Flel.Util;
using Landis.PlugIns;
using System.Collections.Generic;

namespace Landis.Harvest
{
    public class PlugIn
        : Landis.PlugIns.PlugIn
    {
        public static readonly PlugInType Type = new PlugInType("disturbance:harvest");
        private IManagementAreaDataset managementAreas;

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
        }

        //---------------------------------------------------------------------

        public override void Run()
        {
            foreach (ManagementArea mgmtArea in managementAreas)
                mgmtArea.HarvestStands();
        }
    }
}
