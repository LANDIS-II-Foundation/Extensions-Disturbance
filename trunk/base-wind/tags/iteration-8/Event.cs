using Landis.Landscape;
using Landis.Succession;
using System.Collections.Generic;

namespace Landis.Wind
{
	public class Event
	{
		private static EventParameters[] windEventParms;
		private static Landis.Util.Random.Uniform<float> randomUniform;

		private ActiveSite initiationSite;
		private float sizeHectares;
		private int size;	// in # of sites
		private int sitesDamaged;
		private int totalCohortsKilled;
		private int[] cohortsKilled;  // per ecoregion
		private byte severity;

		//---------------------------------------------------------------------

		public Location StartLocation
		{
			get {
				return initiationSite.Location;
			}
		}

		//---------------------------------------------------------------------

		public float SizeHectares
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

		public int CohortsKilled()
		{
			return totalCohortsKilled;
		}

		//---------------------------------------------------------------------

		public int CohortsKilled(int ecoregionIndex)
		{
			return cohortsKilled[ecoregionIndex];
		}

		//---------------------------------------------------------------------

		public byte Severity
		{
			get {
				return severity;
			}
		}

		//---------------------------------------------------------------------

		public static void Initialize(EventParameters[] eventParameters)
		{
			windEventParms = eventParameters;

			randomUniform = new Landis.Util.Random.Uniform<float>(0.0f, 1.0f,
			                                      Model.RandomNumGenerator);
		}

		//---------------------------------------------------------------------

		public static Event Initiate(ActiveSite site)
		{
			Ecoregion ecoregion = Model.SiteVars.Ecoregion[site];
			EventParameters eventParms = windEventParms[ecoregion.Index];
			if (eventParms.Probability > randomUniform.NextValue) {
				Event windEvent = new Event(site,
						                	ComputeSizeHectares(eventParms));
				windEvent.Spread();
				return windEvent;
			}
			return null;
		}

		//---------------------------------------------------------------------

		public static float ComputeSizeHectares(EventParameters eventParms)
		{
			return 0;
		}

		//---------------------------------------------------------------------

		private Event(ActiveSite initiationSite,
		              float      sizeInHectares)
		{
			this.initiationSite = initiationSite;
			this.sizeHectares = sizeInHectares;
			this.size = (int)(sizeInHectares/Model.CellArea);  //CellArea in m^2FIXME
//			                         Units.Convert(Framework.CellSize,
//				                                   "m^2", "hectares"));
			this.cohortsKilled = new int[Model.Ecoregions.Length];
		}

		//---------------------------------------------------------------------

		private void Spread()
		{
			int sitesInEvent = 0;
			Queue<Site> sitesToConsider = new Queue<Site>();
			sitesToConsider.Enqueue(initiationSite);
			while (sitesToConsider.Count > 0 && sitesInEvent < size) {
				Site site = sitesToConsider.Dequeue();
				Damage(site);
				sitesInEvent++;
				if (sitesInEvent < size) {
					//	Add site's neighbors in random order to the list of
					//	sites to consider.  The neighbors cannot be part of
					//	any other wind event in the current timestep, and
					//	cannot already be on the list.
					Site[] neighbors = GetNeighbors(site);
					foreach (Site neighbor in neighbors) {
						if (SiteVars.Event[neighbor] != null)
							continue;
						if (sitesToConsider.Contains(neighbor))
							continue;
						sitesToConsider.Enqueue(neighbor);
					}
				}
			}
		}

		//---------------------------------------------------------------------

		private Site[] GetNeighbors(Site site)
		{
			return null;
		}
		
		//---------------------------------------------------------------------

		private void Damage(Site site)
		{
			ICohorts cohorts = Succession.SiteVars.Cohorts[site];
			cohorts.Remove(new Cohort.FilterMethod(this.DamageCohort));
//			timeLastWind[site] = timestep;
		}

		//---------------------------------------------------------------------

		private bool DamageCohort(ISpecies species,
		                          int      cohortAge)
		{
			return false;
		}
/*
		private int Damage(Site site, double windStrength)
		//This will cause damage from the windthrow at the site.  It will return the
		//actual number of cohorts killed.
		
		{
			//windClass=0;  //global variable for this module
			//int numCohorts=0;
			//site.timeSinceLastWind = currentTimestep;
			
		
			foreach(Cohort cohort in site.cohortList)
			{
				//int i;
				//int longevity = species.longevity;
				//for (i=4; i>=0; i--)
				{
					//if (lifespan[i] < (cohort.age/longevity) ) break;
					//if (i>=0 && windStrength < suscept[i])
					{
						//site.killCohort(cohort);
						//numCohorts++;
			 			//windClass += (5-i);
				}
			}
			//final windClass equal to the average:
			//windClass =  (int) ((float) windClass / (float) numCohorts);
		
			//return numCohorts;
		}

 */

		//---------------------------------------------------------------------

		public void WriteLogEntry(object log)
		{
			//  TODO: implement
		}
	}
}
