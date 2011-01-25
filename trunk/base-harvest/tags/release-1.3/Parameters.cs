using System.Collections.Generic;
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
        //private IManagementAreaDataset managementAreas;
        private string standMap;
        private string prescriptionMapNamesTemplate;
        private string eventLog;
        private string summaryLog;
        private List<Prescription> prescriptions;
        private ManagementAreaDataset managementAreas;

        //---------------------------------------------------------------------

        public int Timestep
        {
            get {
                return timestep;
            }
            set {
                //if (value != null) {
                    if (value < 0)
                        throw new InputValueException(value.ToString(),
                                                      "Timestep must be > or = 0");
                //}
                timestep = value;
            }
        }

        //---------------------------------------------------------------------

        public string ManagementAreaMap
        {
            get {
                return managementAreaMap;
            }
            set {
                if (value != null)
                    CheckPath(value);
                managementAreaMap = value;
            }
        }

        //---------------------------------------------------------------------

        /*public IManagementAreaDataset ManagementAreas
        {
            get {
                return managementAreas;
            }
        }*/

        //---------------------------------------------------------------------

        public string StandMap
        {
            get {
                return standMap;
            }
            set {
                if (value != null)
                    CheckPath(value);
                standMap = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The list of prescriptions in the order they were defined.
        /// </summary>
        public List<Prescription> Prescriptions
        {
            get {
                return prescriptions;
            }
            set {
                prescriptions = value;
            }
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// Management areas that prescriptions are applied to.
        /// </summary>
        public ManagementAreaDataset ManagementAreas
        {
            get {
                return managementAreas;
            }
            set {
                managementAreas = value;
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
            set {
                if (value != null) {
                    MapNames.CheckTemplateVars(value);
                }
                prescriptionMapNamesTemplate = value;
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
            set {
                if (value != null)
                    CheckPath(value);
                eventLog = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Path for the log file with information about harvest events.
        /// </summary>
        public string SummaryLog
        {
            get {
                return summaryLog;
            }
            set {
                if (value != null)
                    CheckPath(value);
                summaryLog = value;
            }
        }
        
        public Parameters()
        {
            prescriptions = new List<Prescription>();
            managementAreas = new ManagementAreaDataset();
        }
        
        //---------------------------------------------------------------------
/*
        public Parameters(int                    timestep,
                          string                 managementAreaMap,
                          IManagementAreaDataset managementAreas,
                          string                 standMap,
                          string                 prescriptionMapNamesTemplate,
                          string                 eventLog,
                          string                 summaryLog)
        {
            this.timestep = timestep;
            this.managementAreaMap = managementAreaMap;
            this.managementAreas = managementAreas;
            this.standMap = standMap;
            this.prescriptionMapNamesTemplate = prescriptionMapNamesTemplate;
            this.eventLog = eventLog;
            this.summaryLog = summaryLog;
            
        }*/
        //---------------------------------------------------------------------

        private void CheckPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new InputValueException(path,
                                              "Path is empty string");

            if (path.Trim(null).Length == 0)
                throw new InputValueException(path,
                                              "Path is just whitespace");
        }
    }
}
