using Landis.Ecoregions;
using Landis.Landscape;
using Landis.Species;
using Landis.Util;
using System.Collections.Generic;

using log4net;

namespace Landis.Wind
{
	public class Event
	{
		private static RelativeLocation[] neighborhood;
		private static IEventParameters[] windEventParms;
		private static ISeverity[] severities;

		private ActiveSite initiationSite;
		private double intensity;
		private double sizeHectares;
		private int size;	// in # of sites
		private int sitesDamaged;
		private int cohortsKilled;
		private double severity;
		private byte siteSeverity;  // used to compute maximum cohort severity
									// at a site

		private static ILog logger;

		//---------------------------------------------------------------------

		static Event()
		{
			neighborhood = new RelativeLocation[] {
				new RelativeLocation(-1, 0),	// north
				new RelativeLocation(1, 0),		// south
				new RelativeLocation(0, 1),		// east
				new RelativeLocation(-1, 0)		// west
			};

			logger = LogManager.GetLogger(typeof(Event));
		}

		//---------------------------------------------------------------------

		public Location StartLocation
		{
			get {
				return initiationSite.Location;
			}
		}

		//---------------------------------------------------------------------

		public double Intensity
		{
			get {
				return intensity;
			}
		}

		//---------------------------------------------------------------------

		public double SizeHectares
		{
			get {
				return sizeHectares;
			}
		}

		//---------------------------------------------------------------------

		public int Size
		{
			get {
				return size;
			}
		}

		//---------------------------------------------------------------------

		public int SitesDamaged
		{
			get {
				return sitesDamaged;
			}
		}

		//---------------------------------------------------------------------

		public int CohortsKilled
		{
			get {
				return cohortsKilled;
			}
		}

		//---------------------------------------------------------------------

		public double Severity
		{
			get {
				return severity;
			}
		}

		//---------------------------------------------------------------------

		public static void Initialize(IEventParameters[] eventParameters,
		                              ISeverity[]        severities)
		{
			windEventParms = eventParameters;
			Event.severities = severities;
		}

		//---------------------------------------------------------------------

		public static Event Initiate(ActiveSite site,
		                             int        currentTime)
		{
			IEcoregion ecoregion = Model.SiteVars.Ecoregion[site];
			IEventParameters eventParms = windEventParms[ecoregion.Index];
			double eventProbability = (currentTime * Model.CellArea) /
				                      (eventParms.RotationPeriod * eventParms.MeanSize);
			if (Random.GenerateUniform() <= eventProbability) {
				Event windEvent = new Event(site,
						                	ComputeSizeHectares(eventParms));
				windEvent.Spread(currentTime);
				return windEvent;
			}
			else
				return null;
		}

		//---------------------------------------------------------------------

		public static double ComputeSizeHectares(IEventParameters eventParms)
		{
			double sizeGenerated = Random.GenerateExponential(eventParms.MeanSize);
			if (sizeGenerated < eventParms.MinSize)
				return eventParms.MinSize;
			else if (sizeGenerated > eventParms.MaxSize)
				return eventParms.MaxSize;
			else
				return sizeGenerated;
		}

		//---------------------------------------------------------------------

		private Event(ActiveSite initiationSite,
		              double     sizeInHectares)
		{
			this.initiationSite = initiationSite;
			this.sizeHectares = sizeInHectares;
			this.size = (int)(sizeInHectares/Model.CellArea);
				// Round up?
			this.intensity = Random.GenerateUniform();
			this.sitesDamaged = 0;
			this.cohortsKilled = 0;

			logger.Debug(string.Format("New wind event at {0}, size = {1} ({2} ha)",
			                           initiationSite.Location, size, sizeHectares));
		}

		//---------------------------------------------------------------------

		private void Spread(int currentTime)
		{
			int sitesInEvent = 0;
			long totalSiteSeverities = 0;
			Queue<Site> sitesToConsider = new Queue<Site>();
			sitesToConsider.Enqueue(initiationSite);
			while (sitesToConsider.Count > 0 && sitesInEvent < size) {
				Site site = sitesToConsider.Dequeue();
				logger.Debug(string.Format("event spread to {0}", site.Location));
				SiteVars.Event[site] = this;
				sitesInEvent++;

				ActiveSite activeSite = site as ActiveSite;
				if (activeSite != null) {
					Damage(activeSite);
					if (siteSeverity > 0) {
						sitesDamaged++;
						totalSiteSeverities += siteSeverity;
						logger.Debug(string.Format("  site severity: {0}", siteSeverity));
						Model.SiteVars.Disturbed[activeSite] = true;
					}
					SiteVars.Severity[activeSite] = siteSeverity;
					SiteVars.TimeOfLastEvent[activeSite] = currentTime;
				}

				if (sitesInEvent < size) {
					//	Add site's neighbors in random order to the list of
					//	sites to consider.  The neighbors cannot be part of
					//	any other wind event in the current timestep, and
					//	cannot already be on the list.
					List<Site> neighbors = GetNeighbors(site);
					Random.Shuffle(neighbors);
					foreach (Site neighbor in neighbors) {
						if (SiteVars.Event[neighbor] != null)
							continue;
						if (sitesToConsider.Contains(neighbor))
							continue;
						sitesToConsider.Enqueue(neighbor);
					}
				}
			}

			if (sitesDamaged == 0)
				severity = 0;
			else
				severity = ((double) totalSiteSeverities) / sitesDamaged;
		}

		//---------------------------------------------------------------------

		private List<Site> GetNeighbors(Site site)
		{
			List<Site> neighbors = new List<Site>(4);
			foreach (RelativeLocation relativeLoc in neighborhood) {
				Site neighbor = site.GetNeighbor(relativeLoc);
				if (neighbor != null)
					neighbors.Add(neighbor);
			}
			return neighbors;
		}
		
		//---------------------------------------------------------------------

		private void Damage(ActiveSite site)
		{
			ISiteCohorts<AgeOnly.ICohort> cohorts = Model.GetSuccession<AgeOnly.ICohort>().Cohorts[site];
			siteSeverity = 0;
			cohorts.Remove(this.DamageCohort, site);
		}

		//---------------------------------------------------------------------

		private bool DamageCohort(AgeOnly.ICohort cohort)
		{
			float ageAsPercent = cohort.Age / (float) cohort.Species.Longevity;
			for (int i = 0; i < severities.Length; ++i) {
				ISeverity severity = severities[i];
				if (severity.AgeRange.Contains(ageAsPercent)) {
					if (intensity < severity.MortalityProbability) {
						cohortsKilled++;
						if (severity.Number > siteSeverity)
							siteSeverity = severity.Number;
						logger.Debug(string.Format("  cohort {0}:{1} killed, severity {2}", cohort.Species.Name, cohort.Age, severity.Number));
						return true;
					}
					break;  // No need to search further in the table
				}
			}
			return false;
		}
	}
}
