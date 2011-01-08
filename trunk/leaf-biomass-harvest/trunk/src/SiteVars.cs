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

        private static ISiteVar<SiteCohorts> cohorts;

        //---------------------------------------------------------------------

        public static void Initialize()
        {
            cohorts = PlugIn.ModelCore.Landscape.NewSiteVar<SiteCohorts>();

            PlugIn.ModelCore.RegisterSiteVar(SiteVars.Cohorts, "Succession.LeafBiomassCohorts");
            foreach (ActiveSite site in PlugIn.ModelCore.Landscape)
            {
                // Test to make sure the cohort type is correct for this extension
                if (site.Location.Row == 1 && site.Location.Column == 1 && !SiteVars.Cohorts[site].HasAge() && !SiteVars.Cohorts[site].HasLeafBiomass())
                {
                    throw new System.ApplicationException("Error in the Scenario file:  Incompatible extensions; Cohort age AND biomass data required for this extension to operate.");
                }
            }


            biomassRemoved = PlugIn.ModelCore.Landscape.NewSiteVar<int>();
            cohortsPartiallyDamaged = PlugIn.ModelCore.Landscape.NewSiteVar<int>();
            capacityReduction = PlugIn.ModelCore.Landscape.NewSiteVar<double>();

            SiteVars.CapacityReduction.ActiveSiteValues = 0.0;

            PlugIn.ModelCore.RegisterSiteVar(SiteVars.CapacityReduction, "Harvest.CapacityReduction");
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

        public static ISiteVar<SiteCohorts> Cohorts
        {
            get
            {
                return cohorts;
            }
        }

        //---------------------------------------------------------------------
    }
}
