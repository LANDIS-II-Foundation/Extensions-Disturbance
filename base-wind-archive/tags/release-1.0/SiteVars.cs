using Landis.Landscape;

namespace Landis.Wind
{
	public static class SiteVars
	{
		private static ISiteVar<Event> eventVar;
		private static ISiteVar<int> timeOfLastEvent;
		private static ISiteVar<byte> severity;

		//---------------------------------------------------------------------

		public static void Initialize()
		{
			eventVar        = Model.Landscape.NewSiteVar<Event>(InactiveSiteMode.DistinctValues);
			timeOfLastEvent = Model.Landscape.NewSiteVar<int>();
			severity        = Model.Landscape.NewSiteVar<byte>();
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
	}
}
