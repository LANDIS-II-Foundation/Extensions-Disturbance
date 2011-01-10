using Landis.Cohorts;
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
		private static int timestep;
		private static ISuccession<AgeCohort.ICohort> successionPlugIn;
		private static ILandscapeCohorts<AgeCohort.ICohort> cohorts;

		private ActiveSite initiationSite;
		private double intensity;
		private double sizeHectares;
		private int size;	// in # of sites
		private int sitesDamaged;
		private int cohortsKilled;
		private double severity;

		private ActiveSite currentSite;	// current site where cohorts are being damaged
		private byte siteSeverity;		// used to compute maximum cohort severity at a site
		
		private static ILog logger;

		//---------------------------------------------------------------------

		static Event()
		{
			neighborhood = new RelativeLocation[] {
				new RelativeLocation(-1,  0),	// north
				new RelativeLocation(-1,  1),	// northeast
				new RelativeLocation( 0,  1),	// east
				new RelativeLocation( 1,  1),	// southeast
				new RelativeLocation( 1,  0),	// south
				new RelativeLocation( 1,  -1),	// southwest
				new RelativeLocation( 0, -1),	// west
				new RelativeLocation( -1, -1),	// northwest
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
		                              ISeverity[]        severities,
		                              int                timestep)
		{
			windEventParms = eventParameters;
			Event.severities = severities;
			Event.timestep = timestep;

			successionPlugIn = Model.GetSuccession<AgeCohort.ICohort>();
			cohorts = successionPlugIn.Cohorts;
		}

		//---------------------------------------------------------------------

		public static Event Initiate(ActiveSite site,
		                             int        currentTime)
		{
			IEcoregion ecoregion = Model.SiteVars.Ecoregion[site];
			IEventParameters eventParms = windEventParms[ecoregion.Index];
			double eventProbability = (timestep * Model.CellArea) /
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
			this.intensity = Random.GenerateUniform();  //intensity ~ wind speed
			this.sitesDamaged = 0;
			this.cohortsKilled = 0;

			logger.Debug(string.Format("New wind event at {0}, size = {1} ({2} ha)",
			                           initiationSite.Location, size, sizeHectares));
		}

		//---------------------------------------------------------------------

		private void Spread(int currentTime)
		{
			int windDirection = (int) (Util.Random.GenerateUniform() * 8);
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
						SiteVars.TimeOfLastEvent[activeSite] = currentTime;
					}
					SiteVars.Severity[activeSite] = siteSeverity;
				}

				if (sitesInEvent < size) {
					//	Add site's neighbors in random order to the list of
					//	sites to consider.  The neighbors cannot be part of
					//	any other wind event in the current timestep, and
					//	cannot already be on the list.
					List<Site> neighbors = GetNeighbors(site, windDirection);
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

		private List<Site> GetNeighbors(Site site, int windDirection)
		{
			if(windDirection > 7) windDirection = 7;
			double[] windProbs = 
			{
			(((4.0 - this.intensity)/8.0) * (1+this.intensity)), //Primary direction
			(((4.0 - this.intensity)/8.0) * (1+this.intensity)),
			(((4.0 - this.intensity)/8.0)),
			(((4.0 - this.intensity)/8.0) * (1-this.intensity)),
			(((4.0 - this.intensity)/8.0) * (1-this.intensity)), //Opposite of primary direction
			(((4.0 - this.intensity)/8.0) * (1-this.intensity)),
			(((4.0 - this.intensity)/8.0)),
			(((4.0 - this.intensity)/8.0) * (1+this.intensity)),
			};
			
			double windProb = 0.0;
			int index = 0;
			List<Site> neighbors = new List<Site>(9);
			foreach (RelativeLocation relativeLoc in neighborhood) 
			{
				Site neighbor = site.GetNeighbor(relativeLoc);
				if(index + windDirection > 7) 
					windProb = windProbs[index + windDirection - 8];
				else 
					windProb = windProbs[index + windDirection];
				if (neighbor != null && Random.GenerateUniform() < windProb)
					neighbors.Add(neighbor);
				index++;
			}
			
			//Next, add the 9th neighbor, a neighbor one cell beyond the 
			//8 nearest neighbors.
			//array index 0 = north; 1 = northeast, 2 = east,...,8 = northwest
			int[] vertical  ={2,2,0,-2,-2,-2,0,2};
			int[] horizontal={0,2,2,2,0,-2,-2,-2};

			RelativeLocation relativeLoc9 = 
				new RelativeLocation(vertical[windDirection], horizontal[windDirection]);
			Site neighbor9 = site.GetNeighbor(relativeLoc9);
			if (neighbor9 != null && Random.GenerateUniform() < this.intensity)
				neighbors.Add(neighbor9);
			return neighbors;
		}
		
		//---------------------------------------------------------------------

		private void Damage(ActiveSite site)
		{
			currentSite = site;
			siteSeverity = 0;
			cohorts[site].Remove(this.DamageCohort);
		}

		//---------------------------------------------------------------------

		private bool DamageCohort(AgeCohort.ICohort cohort)
		{
			float ageAsPercent = cohort.Age / (float) cohort.Species.Longevity;
			for (int i = 0; i < severities.Length; ++i) {
				ISeverity severity = severities[i];
				if (severity.AgeRange.Contains(ageAsPercent)) {
					if (intensity < severity.MortalityProbability) {
						cohortsKilled++;
						if (severity.Number > siteSeverity)
							siteSeverity = severity.Number;
						successionPlugIn.CheckForResprouting(cohort, currentSite);
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
