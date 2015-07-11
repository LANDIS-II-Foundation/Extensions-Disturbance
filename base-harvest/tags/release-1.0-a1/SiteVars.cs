using Landis.Landscape;

namespace Landis.Harvest
{
    public static class SiteVars
    {
        private static ISiteVar<ManagementArea> mgmtAreas;
        private static ISiteVar<Stand> stands;

        //---------------------------------------------------------------------

        public static void Initialize()
        {
            mgmtAreas = Model.Core.Landscape.NewSiteVar<ManagementArea>();
            stands    = Model.Core.Landscape.NewSiteVar<Stand>();
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
    }
}
