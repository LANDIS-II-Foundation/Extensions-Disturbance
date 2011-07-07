using Landis.SpatialModeling;
using Landis.Library.BiomassCohorts;

namespace Landis.Extension.StressMortality
{
    class SiteVars
    {
        private static ISiteVar<ushort> stressBioRemoved;
        private static ISiteVar<ushort> stressYears;
        private static ISiteVar<ISiteCohorts> biomassCohorts;

        //---------------------------------------------------------------------
        public static void Initialize()
        {
            biomassCohorts = PlugIn.ModelCore.GetSiteVar<ISiteCohorts>("Succession.BiomassCohorts");
            stressBioRemoved = PlugIn.ModelCore.Landscape.NewSiteVar<ushort>();
            stressYears = PlugIn.ModelCore.GetSiteVar<ushort>( "Stress.Years");
        }

        //---------------------------------------------------------------------
        public static ISiteVar<ushort> StressBioRemoved
        {
            get
            {
                return stressBioRemoved;
            }
        }

        //---------------------------------------------------------------------
        public static ISiteVar<ushort> StressYears
        {
            get
            {
                return stressYears;
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Biomass cohorts at each site.
        /// </summary>
        public static ISiteVar<ISiteCohorts> Cohorts
        {
            get
            {
                return biomassCohorts;
            }
            set
            {
                biomassCohorts = value;
            }
        }

        //---------------------------------------------------------------------

    }
}
