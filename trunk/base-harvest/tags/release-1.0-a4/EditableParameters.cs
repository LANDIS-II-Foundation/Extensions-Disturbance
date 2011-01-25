using Edu.Wisc.Forest.Flel.Util;
using System.Collections.Generic;

namespace Landis.Harvest
{
    /// <summary>
    /// Editable set of parameters for harvest.
    /// </summary>
    public class EditableParameters
        : IEditable<IParameters>
    {
        private InputValue<int> timestep;
        private InputValue<string> managementAreaMap;
        private InputValue<string> standMap;
        private List<Prescription> prescriptions;
        private ManagementAreaDataset managementAreas;
        private InputValue<string> prescriptionMapNamesTemplate;
        private InputValue<string> eventLog;

        //---------------------------------------------------------------------

        /// <summary>
        /// Timestep (years)
        /// </summary>
        public InputValue<int> Timestep
        {
            get {
                return timestep;
            }

            set {
                if (value != null) {
                    if (value.Actual < 0)
                        throw new InputValueException(value.String,
                                                      "Timestep must be > or = 0");
                }
                timestep = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Path to the map of management areas.
        /// </summary>
        public InputValue<string> ManagementAreaMap
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

        private void CheckPath(InputValue<string> path)
        {
            if (string.IsNullOrEmpty(path.Actual))
                throw new InputValueException(path.String,
                                              "Path is empty string");

            if (path.Actual.Trim(null).Length == 0)
                throw new InputValueException(path.String,
                                              "Path is just whitespace");
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Path to the map of stands.
        /// </summary>
        public InputValue<string> StandMap
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
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Template for pathnames for prescription maps.
        /// </summary>
        public InputValue<string> PrescriptionMapNames
        {
            get {
                return prescriptionMapNamesTemplate;
            }

            set {
                if (value != null) {
                    MapNames.CheckTemplateVars(value.Actual);
                }
                prescriptionMapNamesTemplate = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Path for the log file.
        /// </summary>
        public InputValue<string> EventLog
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

        public bool IsComplete
        {
            get {
                object[] parameters = new object[]{ timestep,
                                                    managementAreaMap,
                                                    standMap,
                                                    prescriptionMapNamesTemplate,
                                                    eventLog };
                foreach (object parameter in parameters)
                    if (parameter == null)
                        return false;
                return true;
            }
        }

        //---------------------------------------------------------------------

        public EditableParameters()
        {
            prescriptions = new List<Prescription>();
            managementAreas = new ManagementAreaDataset();
        }

        //---------------------------------------------------------------------

        public IParameters GetComplete()
        {
            if (this.IsComplete)
                return new Parameters(timestep.Actual,
                                      managementAreaMap.Actual,
                                      managementAreas,
                                      standMap.Actual,
                                      prescriptionMapNamesTemplate.Actual,
                                      eventLog.Actual);
            else
                return null;
        }
    }
}
