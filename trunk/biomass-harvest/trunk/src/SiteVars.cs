// Copyright 2008-2010 Green Code LLC, Portland State University
// Authors:  James B. Domingo, Robert M. Scheller


using Landis.SpatialModeling;
using Landis.Library.BiomassCohorts;
using Landis.Extension.BaseHarvest;

namespace Landis.Extension.BiomassHarvest
{
    public class SiteVars : Landis.Extension.BaseHarvest.SiteVars
    {
        private static ISiteVar<double> biomassRemoved;
        private static ISiteVar<int> cohortsPartiallyDamaged;
        private static ISiteVar<double> capacityReduction;

        private static ISiteVar<ISiteCohorts> cohorts;

        //---------------------------------------------------------------------

        public static new void Initialize()
        {

            cohorts = PlugIn.ModelCore.GetSiteVar<ISiteCohorts>("Succession.BiomassCohorts");

            biomassRemoved = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            cohortsPartiallyDamaged = PlugIn.ModelCore.Landscape.NewSiteVar<int>();
            capacityReduction = PlugIn.ModelCore.Landscape.NewSiteVar<double>();

            SiteVars.CapacityReduction.ActiveSiteValues = 0.0;

            PlugIn.ModelCore.RegisterSiteVar(SiteVars.CapacityReduction, "Harvest.CapacityReduction");

        }
        //---------------------------------------------------------------------
        public static new ushort GetMaxAge(ActiveSite site)
        {
            int maxAge = 0;
            foreach (ISpeciesCohorts sppCo in SiteVars.Cohorts[site])
                foreach (ICohort cohort in sppCo)
                    if (cohort.Age > maxAge)
                        maxAge = cohort.Age;

            return (ushort) maxAge;

        }

        //---------------------------------------------------------------------

        public static ISiteVar<double> BiomassRemoved
        {
            get {
                return biomassRemoved;
            }
        }
        //---------------------------------------------------------------------

        public static ISiteVar<int> CohortsPartiallyDamaged
        {
            get {
                return cohortsPartiallyDamaged;
            }
        }
        //---------------------------------------------------------------------

        public static ISiteVar<double> CapacityReduction
        {
            get {
                return capacityReduction;
            }
        }

        //---------------------------------------------------------------------
        public static new ISiteVar<ISiteCohorts> Cohorts
        {
            get
            {
                return cohorts;
            }
        }

    }
}
