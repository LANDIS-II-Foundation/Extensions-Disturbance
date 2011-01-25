using Landis.Landscape;

namespace Landis.Harvest
{
    public static class SiteVars
    {
        private static ISiteVar<int> timeOfLastEvent;
        private static ISiteVar<ManagementArea> mgmtAreas;
        private static ISiteVar<Stand> stand;
        private static ISiteVar<Prescription> prescription;
        private static ISiteVar<string> prescription_name;
        private static ISiteVar<int> cohortsDamaged;
        private static ISiteVar<int> timeOfLastFire;
        private static ISiteVar<int> timeOfLastWind;


        //---------------------------------------------------------------------

        public static void Initialize()
        {
            timeOfLastEvent         = Model.Core.Landscape.NewSiteVar<int>();
            mgmtAreas               = Model.Core.Landscape.NewSiteVar<ManagementArea>();
            stand                   = Model.Core.Landscape.NewSiteVar<Stand>();
            prescription            = Model.Core.Landscape.NewSiteVar<Prescription>();
            prescription_name       = Model.Core.Landscape.NewSiteVar<string>();
            cohortsDamaged          = Model.Core.Landscape.NewSiteVar<int>();
            timeOfLastFire          = Model.Core.GetSiteVar<int>("Fire.TimeOfLastEvent");
            timeOfLastWind          = Model.Core.GetSiteVar<int>("Wind.TimeOfLastEvent");

            Model.Core.RegisterSiteVar(SiteVars.PrescriptionName, "Harvest.PrescriptionName");
            Model.Core.RegisterSiteVar(SiteVars.TimeOfLastEvent, "Harvest.TimeOfLastEvent");
            Model.Core.RegisterSiteVar(SiteVars.CohortsDamaged, "Harvest.CohortsDamaged");

            SiteVars.TimeOfLastEvent.ActiveSiteValues = -100;
            SiteVars.Prescription.SiteValues = null;
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
                return prescription;
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
            get {
                return cohortsDamaged;
            }
            set {
                cohortsDamaged = value;
            }
        }
        //---------------------------------------------------------------------
        public static int TimeSinceLastDamage(ActiveSite site)
        {

            int lastDamageTime = -100;

            if(SiteVars.TimeOfLastEvent[(Site) site] > lastDamageTime)
                lastDamageTime = SiteVars.TimeOfLastEvent[(Site)site];

            if (SiteVars.TimeOfLastFire != null)
                if (SiteVars.TimeOfLastFire[(Site)site] > lastDamageTime && SiteVars.TimeOfLastFire[(Site) site] > 0)
                    lastDamageTime = SiteVars.TimeOfLastFire[(Site)site];

            if (SiteVars.TimeOfLastWind != null)
                if (SiteVars.TimeOfLastWind[(Site)site] > lastDamageTime && SiteVars.TimeOfLastWind[(Site) site] > 0)
                    lastDamageTime = SiteVars.TimeOfLastWind[(Site)site];

            return Model.Core.CurrentTime - lastDamageTime;
        }

    }
}
