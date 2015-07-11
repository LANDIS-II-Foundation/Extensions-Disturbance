using Landis.Landscape;

namespace Landis.Harvest
{
    public static class SiteVars
    {
        private static ISiteVar<ManagementArea> mgmtAreas;
        private static ISiteVar<Stand> stands;
        private static ISiteVar<Prescription> prescriptions;
        private static ISiteVar<int> cohortsKilled;

        //---------------------------------------------------------------------

        public static void Initialize()
        {
            mgmtAreas     = Model.Core.Landscape.NewSiteVar<ManagementArea>();
            stands        = Model.Core.Landscape.NewSiteVar<Stand>();
            prescriptions = Model.Core.Landscape.NewSiteVar<Prescription>();
            cohortsKilled = Model.Core.Landscape.NewSiteVar<int>();
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
                return stands;
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

        public static ISiteVar<int> CohortsKilled
        {
            get {
                return cohortsKilled;
            }
        }
    }
}
