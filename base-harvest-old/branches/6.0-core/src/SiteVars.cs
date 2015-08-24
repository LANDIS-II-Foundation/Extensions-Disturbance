using Wisc.Flel.GeospatialModeling.Landscapes;

namespace Landis.Harvest
{
    public static class SiteVars
    {
        private static ISiteVar<int> timeOfLastEvent;
        private static ISiteVar<ManagementArea> mgmtAreas;
        private static ISiteVar<Stand> stand;
        private static ISiteVar<Prescription> prescriptions;
        private static ISiteVar<string> prescription_name;
        private static ISiteVar<int> cohortsKilled;

        //---------------------------------------------------------------------

        public static void Initialize()
        {
            timeOfLastEvent = Model.Core.Landscape.NewSiteVar<int>();
            mgmtAreas     = Model.Core.Landscape.NewSiteVar<ManagementArea>();
            stand         = Model.Core.Landscape.NewSiteVar<Stand>();
            prescriptions = Model.Core.Landscape.NewSiteVar<Prescription>();
            prescription_name = Model.Core.Landscape.NewSiteVar<string>();
            cohortsKilled = Model.Core.Landscape.NewSiteVar<int>();

            Model.Core.RegisterSiteVar(SiteVars.PrescriptionName, "Harvest.PrescriptionName");
            Model.Core.RegisterSiteVar(SiteVars.TimeOfLastEvent, "Harvest.TimeOfLastEvent");
            Model.Core.RegisterSiteVar(SiteVars.CohortsKilled, "Harvest.CohortsKilled");

            SiteVars.TimeOfLastEvent.ActiveSiteValues = 0;

        }

        //---------------------------------------------------------------------

        public static ISiteVar<ManagementArea> ManagementArea
        {
            get {
                return mgmtAreas;
            }
        }

        //---------------------------------------------------------------------

        public static ISiteVar<Stand> Stand
        {
            get {
                return stand;
            }
        }

        //---------------------------------------------------------------------

        public static ISiteVar<Prescription> Prescription

        {
            get {
                return prescriptions;
            }
        }

        //---------------------------------------------------------------------

        public static ISiteVar<string> PrescriptionName {
            get {
                return prescription_name;
            }
        }

        //---------------------------------------------------------------------

        public static ISiteVar<int> TimeOfLastEvent {
            get {
                return timeOfLastEvent;
            }
        }

        //---------------------------------------------------------------------

        public static ISiteVar<int> CohortsKilled
        {
            get {
                return cohortsKilled;
            }
        }
    }
}
