//  Copyright 2005 University of Wisconsin
//  Authors:  Jimm Domingo, Robert M. Scheller
//  License:  Available at  
//  http://landis.forest.wisc.edu/developers/LANDIS-IISourceCodeLicenseAgreement.pdf

using Landis.AgeCohort;
using Landis.Landscape;

namespace Landis.Wind
{
    public static class SiteVars
    {
        private static ISiteVar<Event> eventVar;
        private static ISiteVar<int> timeOfLastEvent;
        private static ISiteVar<byte> severity;
        private static ISiteVar<bool> disturbed;
        private static ISiteVar<byte> lastSeverity;

        //---------------------------------------------------------------------

        public static void Initialize()
        {
            eventVar        = Model.Core.Landscape.NewSiteVar<Event>(InactiveSiteMode.DistinctValues);
            timeOfLastEvent = Model.Core.Landscape.NewSiteVar<int>();
            severity        = Model.Core.Landscape.NewSiteVar<byte>();
            disturbed      = Model.Core.Landscape.NewSiteVar<bool>();
            lastSeverity        = Model.Core.Landscape.NewSiteVar<byte>();
            
            Model.Core.RegisterSiteVar(SiteVars.TimeOfLastEvent, "Wind.TimeOfLastEvent");
            Model.Core.RegisterSiteVar(SiteVars.LastSeverity, "Wind.LastSeverity");

        }

        //---------------------------------------------------------------------

        public static ISiteVar<Event> Event
        {
            get {
                return eventVar;
            }
        }

        //---------------------------------------------------------------------

        public static ISiteVar<int> TimeOfLastEvent
        {
            get {
                return timeOfLastEvent;
            }
        }

        //---------------------------------------------------------------------

        public static ISiteVar<byte> Severity
        {
            get {
                return severity;
            }
        }
        //---------------------------------------------------------------------

        public static ISiteVar<byte> LastSeverity
        {
            get {
                return lastSeverity;
            }
        }
        //---------------------------------------------------------------------

        public static ISiteVar<bool> Disturbed
        {
            get {
                return disturbed;
            }
        }
    }
}
