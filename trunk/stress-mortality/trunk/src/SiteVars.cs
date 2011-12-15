using Landis.SpatialModeling;
using Landis.Library.BiomassCohorts;
using Landis.Core;
using System.Collections.Generic;

namespace Landis.Extension.StressMortality
{
    class SiteVars
    {
        private static ISiteVar<int> stressBioRemoved;
        //private static ISiteVar<ushort> stressYears;
        private static ISiteVar<ISiteCohorts> biomassCohorts;
        public static ISiteVar<SpeciesAuxParm<Dictionary<int, Dictionary<int, int>>>> CumulativeMortality;

        //---------------------------------------------------------------------
        public static void Initialize()
        {
            biomassCohorts = PlugIn.ModelCore.GetSiteVar<ISiteCohorts>("Succession.BiomassCohorts");
            stressBioRemoved = PlugIn.ModelCore.Landscape.NewSiteVar<int>();
            //stressYears = PlugIn.ModelCore.GetSiteVar<ushort>( "Stress.Years");
            CumulativeMortality = PlugIn.ModelCore.Landscape.NewSiteVar<SpeciesAuxParm<Dictionary<int, Dictionary<int, int>>>>();

            foreach (ActiveSite site in PlugIn.ModelCore.Landscape)
            {
                SiteVars.CumulativeMortality[site] = new SpeciesAuxParm<Dictionary<int, Dictionary<int, int>>>(PlugIn.ModelCore.Species);
                foreach (ISpecies spp in PlugIn.ModelCore.Species)
                {
                    // Dictionary = time, age, reduction.
                    //Dictionary<int, int> cohortAgeReductions = new Dictionary<int, int>();
                    SiteVars.CumulativeMortality[site][spp] = new Dictionary<int, Dictionary<int, int>>();
                }
            }

            if (biomassCohorts == null)
            {
                string mesg = string.Format("Cohorts are empty.  Please double-check that this extension is compatible with your chosen succession extension.");
                throw new System.ApplicationException(mesg);
            }
        }

        //---------------------------------------------------------------------
        public static ISiteVar<int> StressBioRemoved
        {
            get
            {
                return stressBioRemoved;
            }
        }

        //---------------------------------------------------------------------
        /*public static ISiteVar<ushort> StressYears
        {
            get
            {
                return stressYears;
            }
        }*/

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
