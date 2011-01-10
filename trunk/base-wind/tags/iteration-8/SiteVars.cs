using Landis.Landscape;

namespace Landis.Wind
{
	public static class SiteVars
	{
		private static SiteVariable<Event> eventVar;
		private static SiteVariable<int> timeLastEventVar;
		private static SiteVariable<byte> severityVar;

		//---------------------------------------------------------------------

		public static void Initialize()
		{
			eventVar = new SiteVariable<Event>("wind.event");
			Model.Landscape.Add(eventVar);

			timeLastEventVar = new SiteVariable<int>("wind.timeLastEvent");
			Model.Landscape.Add(timeLastEventVar);

			severityVar = new SiteVariable<byte>("wind.severity");
			Model.Landscape.Add(severityVar);

#if REVISED_LANDSCAPE_INTERFACE
			eventVar         = Model.Landscape.NewSiteVar<Event>();
			timeLastEventVar = Model.Landscape.NewSiteVar<int>();
			severityVar      = Model.Landscape.NewSiteVar<byte>();
#endif
		}

		//---------------------------------------------------------------------

		public static SiteVariable<Event> Event
		{
			get {
				return eventVar;
			}
		}

		//---------------------------------------------------------------------

		public static SiteVariable<int> TimeLastEvent
		{
			get {
				return timeLastEventVar;
			}
		}

		//---------------------------------------------------------------------

		public static SiteVariable<byte> Severity
		{
			get {
				return severityVar;
			}
		}
	}
}
