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
        private string prescriptionMapNamesTemplate;
        private string eventLog;

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

        /// <summary>
        /// Template for pathnames for prescription maps.
        /// </summary>
        public string PrescriptionMapNames
        {
            get {
                return prescriptionMapNamesTemplate;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Path for the log file with information about harvest events.
        /// </summary>
        public string EventLog
        {
            get {
                return eventLog;
            }
        }

        //---------------------------------------------------------------------

        public Parameters(int                    timestep,
                          string                 managementAreaMap,
                          IManagementAreaDataset managementAreas,
                          string                 standMap,
                          string                 prescriptionMapNamesTemplate,
                          string                 eventLog)
        {
            this.timestep = timestep;
            this.managementAreaMap = managementAreaMap;
            this.managementAreas = managementAreas;
            this.standMap = standMap;
            this.prescriptionMapNamesTemplate = prescriptionMapNamesTemplate;
            this.eventLog = eventLog;
        }
    }
}
