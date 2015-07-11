using Edu.Wisc.Forest.Flel.Util;

namespace Landis.Harvest
{
    /// <summary>
    /// The parameters for harvest.
    /// </summary>
    public class Parameters
        : IParameters
    {
        private int timestep;
        private string managementAreaMap;
        private IManagementAreaDataset managementAreas;
        private string standMap;

        //---------------------------------------------------------------------

        public int Timestep
        {
            get {
                return timestep;
            }
        }

        //---------------------------------------------------------------------

        public string ManagementAreaMap
        {
            get {
                return managementAreaMap;
            }
        }

        //---------------------------------------------------------------------

        public IManagementAreaDataset ManagementAreas
        {
            get {
                return managementAreas;
            }
        }

        //---------------------------------------------------------------------

        public string StandMap
        {
            get {
                return standMap;
            }
        }

        //---------------------------------------------------------------------

        public Parameters(int                    timestep,
                          string                 managementAreaMap,
                          IManagementAreaDataset managementAreas,
                          string                 standMap)
        {
            this.timestep = timestep;
            this.managementAreaMap = managementAreaMap;
            this.managementAreas = managementAreas;
            this.standMap = standMap;
        }
    }
}
