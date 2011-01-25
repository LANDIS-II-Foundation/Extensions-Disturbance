using Landis.AgeCohort;
using System;

namespace Landis.Harvest
{
    internal static class Model
    {
        private static PlugIns.ICore core;
        private static ILandscapeCohorts cohorts;
        public static double blockArea;

        //---------------------------------------------------------------------

        internal static PlugIns.ICore Core
        {
            get {
                return core;
            }

            set {
                core = value;
                if (value != null) {
                    cohorts = core.SuccessionCohorts as ILandscapeCohorts;
                    if (cohorts == null)
                        throw new ApplicationException("Cohorts don't support age-cohort interface");

                    blockArea = core.Landscape.SitesPerBlock * core.CellArea;
                }
            }
        }

        //---------------------------------------------------------------------

        internal static ILandscapeCohorts LandscapeCohorts
        {
            get {
                return cohorts;
            }
        }

        //---------------------------------------------------------------------

        internal static double BlockArea
        {
            get {
                return blockArea;
            }
        }
    }
}
