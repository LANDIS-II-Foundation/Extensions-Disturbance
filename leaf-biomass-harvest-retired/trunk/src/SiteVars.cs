// Copyright 2008-2010 Green Code LLC, Portland State University
// Authors:  James B. Domingo, Robert M. Scheller, Srinivas S.

using Landis.SpatialModeling;
using Landis.Library.LeafBiomassCohorts;

namespace Landis.Extension.LeafBiomassHarvest
{
    public static class SiteVars
    {
        private static ISiteVar<int> biomassRemoved;
        private static ISiteVar<int> cohortsPartiallyDamaged;
        private static ISiteVar<double> capacityReduction;

        private static ISiteVar<ISiteCohorts> cohorts;

        //---------------------------------------------------------------------

        public static void Initialize()
        {
            cohorts = PlugIn.ModelCore.GetSiteVar<ISiteCohorts>("Succession.LeafBiomassCohorts");

            biomassRemoved = PlugIn.ModelCore.Landscape.NewSiteVar<int>();
            cohortsPartiallyDamaged = PlugIn.ModelCore.Landscape.NewSiteVar<int>();
            capacityReduction = PlugIn.ModelCore.Landscape.NewSiteVar<double>();

            SiteVars.CapacityReduction.ActiveSiteValues = 0.0;

            PlugIn.ModelCore.RegisterSiteVar(SiteVars.CapacityReduction, "Harvest.CapacityReduction");

            if (cohorts == null)
            {
                string mesg = string.Format("Cohorts are empty:  The requested cohort type (LeafBiomassCohorts) are not available.  Are you using the correct Harvest extension?");
                throw new System.ApplicationException(mesg);
            }

        }

        //---------------------------------------------------------------------

        public static ISiteVar<int> BiomassRemoved
        {
            get {
                return biomassRemoved;
            }
        }

        //---------------------------------------------------------------------

        public static ISiteVar<int> CohortsPartiallyDamaged
        {
            get
            {
                return cohortsPartiallyDamaged;
            }
        }
        //---------------------------------------------------------------------

        public static ISiteVar<double> CapacityReduction
        {
            get
            {
                return capacityReduction;
            }
        }
        //---------------------------------------------------------------------

        public static ISiteVar<ISiteCohorts> Cohorts
        {
            get
            {
                return cohorts;
            }
        }

        //---------------------------------------------------------------------
    }
}
