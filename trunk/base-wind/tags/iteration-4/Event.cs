using Landis.Core;
using Landis.Landscape;

namespace Landis.Wind
{
	public class Event
	{
		private static EventParameters[] windEventParms;
		private static SiteVariable<Ecoregion> ecoregionSiteVar;
		private static Landis.Util.Random.Uniform<float> randomUniform;

		private Location initiationSite;
		private float sizeInHectares;
		private int sizeInSites;
		private byte windClass;

		//---------------------------------------------------------------------

		public static void Initialize(EventParameters[] eventParameters)
		{
			windEventParms = eventParameters;

			ecoregionSiteVar = new SiteVariable<Ecoregion>("ecoregion");
			Framework.Landscape.Add(ecoregionSiteVar);

			randomUniform = new Landis.Util.Random.Uniform<float>(0.0, 1.0,
			                                      Framework.RandomNumGenerator);
		}

		//---------------------------------------------------------------------

		public static Event Initiate(ActiveSite site)
		{
			Ecoregion ecoregion = ecoregionSiteVar[site];
			EventParameters eventParms;
			// code if local parameter array
			eventParms = windEventParms[ecoregion.Index];

			// code if: EcoregionVariable<EventParameters> windEventParms
			eventParms = windEventParms[ecoregion];
			
			// code if: EcoregionVariable<EventParameters> windEventParms
			//          and extensible Ecoregion class
			eventParms = ecoregion.XParm(windEventParms);

			if (eventParms.Probability > randomUniform.NextValue) {
				float size = f(eventParms, randomExponential);
				return new Event(site, size);
			}
			return null;
		}

		//---------------------------------------------------------------------

		private Event(ActiveSite initiationSite,
		              float      sizeInHectares)
		{
			this.initiationSite = initiationSite.Location;
			this.sizeInHectares = sizeInHectares;
			this.sizeInSites = (int)(sizeInHectares /
			                         Units.Convert(Framework.CellSize,
				                                   "m^2", "hectares"));
		}

		//---------------------------------------------------------------------

		public byte WindClass
		{
			get {
				return windClass;
			}
		}

		//---------------------------------------------------------------------

		public void Spread()
		{
/*
		private bool Spread(ActiveSite site, int numberWindSites)

			//int numCohortsKilled = 0;
			//int numCohortsKilledByEcoregion[ecoregion.numActiveEcoregions] = 0;
			//int numSitesDamaged = 0;
			//create stack (first on, first off) myStack
			//int numActualSites = 0;
			
			//Randomly determine windDirection and windStrength:
			//int windDirection; //1 - 4 [for now]
			//double windStrength = randomNumber(0-1)
			
			//Jimm: please (!) replace with recursive loop.
			//while (numActualSites < numberSitesDamaged && !myStack.empty())
			{
				//pull nextSite from top of myStack
				//get ecoregionIndex for nextSite
				//if (nextSite.active && timeOfLastWind != currentTimestep) 
					//numCohortsKilledByEcoregion[ecoregionIndex] = Damage(nextSite, windStrength)
					//if(numCohortsKilled[ecoregionIndex]>0) 
					{
						numSitesDamaged++;
						//windDisturbMap.modify(nextSite) = windClass + 1;
					}
				
				//calculate probabilities for N,S,E,W based on windDirection and windStrength
				//These sites do NOT need to be active.
				//Check neighbors, timeOfLastWind != currentTimestep
				//if(probabilityN > randomNumber) add site one-up to myStack
				//if(probabilityS > randomNumber) add site one-down to myStack
				//if(probabilityE > randomNumber) add site one-right to myStack
				//if(probabilityW > randomNumber) add site one-left to myStack
			} //endwhile
			
			//Log the wind event, both total for the event and by ecoregion
			
 */
		}
		
		//---------------------------------------------------------------------

		public void WriteLogEntry(object log)
		{
			//  TODO: implement
		}
	}
}
/*
		
		//---------------------------------------------------------------------

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
