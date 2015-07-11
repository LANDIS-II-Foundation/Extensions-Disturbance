namespace Landis.Harvest
{
    /// <summary>
    /// The parameters for harvest.
    /// </summary>
    public interface IParameters
    {
        /// <summary>
        /// Timestep (years)
        /// </summary>
        int Timestep
        {
            get;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Path to the map of management areas.
        /// </summary>
        string ManagementAreaMap
        {
            get;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Management areas that prescriptions have been applied to.
        /// </summary>
        IManagementAreaDataset ManagementAreas
        {
            get;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Path to the map of stands.
        /// </summary>
        string StandMap
        {
            get;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Template for pathnames for prescription maps.
        /// </summary>
        string PrescriptionMapNames
        {
            get;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Path for the log file with information about harvest events.
        /// </summary>
        string EventLog
        {
            get;
        }
    }
}
