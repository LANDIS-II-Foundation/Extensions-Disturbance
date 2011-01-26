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

        // copied from BaseHarvest
        /*
        private static ISiteVar<int> timeOfLastEvent;
        private static ISiteVar<ManagementArea> mgmtAreas;
        private static ISiteVar<Stand> stand;
        private static ISiteVar<Prescription> prescription;
        private static ISiteVar<string> prescription_name;
        private static ISiteVar<int> cohortsDamaged;
        private static ISiteVar<int> timeOfLastFire;
        private static ISiteVar<int> timeOfLastWind;
        */
        //---------------------------------------------------------------------

        public static new void Initialize()
        {

            // copied from BaseHarvest ********************************
            /*
            timeOfLastEvent = PlugIn.ModelCore.Landscape.NewSiteVar<int>();
            mgmtAreas = PlugIn.ModelCore.Landscape.NewSiteVar<ManagementArea>();
            stand = PlugIn.ModelCore.Landscape.NewSiteVar<Stand>();
            prescription = PlugIn.ModelCore.Landscape.NewSiteVar<Prescription>();
            prescription_name = PlugIn.ModelCore.Landscape.NewSiteVar<string>();
            cohortsDamaged = PlugIn.ModelCore.Landscape.NewSiteVar<int>();
            timeOfLastFire = PlugIn.ModelCore.GetSiteVar<int>("Fire.TimeOfLastEvent");
            timeOfLastWind = PlugIn.ModelCore.GetSiteVar<int>("Wind.TimeOfLastEvent");
            
            PlugIn.ModelCore.RegisterSiteVar(SiteVars.PrescriptionName, "Harvest.PrescriptionName");
            PlugIn.ModelCore.RegisterSiteVar(SiteVars.TimeOfLastEvent, "Harvest.TimeOfLastEvent");
            PlugIn.ModelCore.RegisterSiteVar(SiteVars.CohortsDamaged, "Harvest.CohortsDamaged");

            SiteVars.TimeOfLastEvent.ActiveSiteValues = -100;
            SiteVars.Prescription.SiteValues = null;
            */
            // **********************************************************
            
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
        //---------------------------------------------------------------------
        /*
        public static ISiteVar<ManagementArea> ManagementArea
        {
            get
            {
                return mgmtAreas;
            }
        }

        //---------------------------------------------------------------------

        public static ISiteVar<Stand> Stand
        {
            get
            {
                return stand;
            }
        }

        //---------------------------------------------------------------------

        public static ISiteVar<Prescription> Prescription
        {
            get
            {
                return prescription;
            }
        }

        //---------------------------------------------------------------------

        public static ISiteVar<string> PrescriptionName
        {
            get
            {
                return prescription_name;
            }
        }

        //---------------------------------------------------------------------

        public static ISiteVar<int> TimeOfLastEvent
        {
            get
            {
                return timeOfLastEvent;
            }
        }

        //---------------------------------------------------------------------
        public static ISiteVar<int> TimeOfLastFire
        {
            get
            {
                return timeOfLastFire;
            }
        }

        //---------------------------------------------------------------------
        public static ISiteVar<int> TimeOfLastWind
        {
            get
            {
                return timeOfLastWind;
            }
        }
        //---------------------------------------------------------------------

        public static ISiteVar<int> CohortsDamaged
        {
            get
            {
                return cohortsDamaged;
            }
            set
            {
                cohortsDamaged = value;
            }
        }
        */
        //---------------------------------------------------------------------
        /*public static ushort GetMaxAge(ActiveSite site)
        {
            return Util.GetMaxAge(SiteVars.Cohorts[site]);
        }*/

        //---------------------------------------------------------------------
        /*public static int TimeSinceLastDamage(ActiveSite site)
        {

            int lastDamageTime = -100;

            if (SiteVars.TimeOfLastEvent[(Site)site] > lastDamageTime)
                lastDamageTime = SiteVars.TimeOfLastEvent[(Site)site];

            if (SiteVars.TimeOfLastFire != null)
                if (SiteVars.TimeOfLastFire[(Site)site] > lastDamageTime && SiteVars.TimeOfLastFire[(Site)site] > 0)
                    lastDamageTime = SiteVars.TimeOfLastFire[(Site)site];

            if (SiteVars.TimeOfLastWind != null)
                if (SiteVars.TimeOfLastWind[(Site)site] > lastDamageTime && SiteVars.TimeOfLastWind[(Site)site] > 0)
                    lastDamageTime = SiteVars.TimeOfLastWind[(Site)site];

            return PlugIn.ModelCore.CurrentTime - lastDamageTime;
        }*/

    }
}
