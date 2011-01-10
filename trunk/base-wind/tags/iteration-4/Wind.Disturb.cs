using Landis.Landscape;

namespace Landis.Disturbance
{
	///<summary>
	/// A sample disturbance plug-in that simulates wind disturbance.
	/// Copyright University of Wisconsin-Madison 2004
	/// </summary>
	public class Wind
		: Landis.Disturbance.IPlugIn
	{
		ILandscape landscape;
		private SiteVariable<int> timeSinceLastWind;
		private EcoregionVariable<double> windProbability;
		private EcoregionVariable<int> maxWindSize;  //In hectares
		private EcoregionVariable<int> minWindSize;  //In hectares
		private int timestep;
		private int nextTimestep;
		private int windClass;  //a number 1 - 5 indicating damage done by wind
		//This is clutsy: should be input with wind parameter file!
		static const float lifespan[5]={(float)0.0,(float)0.2,(float)0.5,(float)0.7,(float)0.85};
		static const float suscept[5]={(float)0.05,(float)0.1,(float)0.5,(float)0.85,(float)0.95};


		//---------------------------------------------------------------------

		///<summary>
		/// The contructor:  Create a new plug-in.
		///</summary>
		public Wind()
		{//Create the Site Variables:
			timeSinceLastWind = new SiteVariable<int>("timeSinceLastWind");
			windProbability = new EcoregionVariable<double> ("windProbability");
			maxWindSize = new EcoregionVariable<int> ("maxWindSize");  //In hectares
			minWindSize = new EcoregionVariable<int>("minWindSize");  //In hectares
		}

		//---------------------------------------------------------------------

		///<summary>
		/// Initialize the plug-in.  Read data from text file.
		///</summary>
		public void Initialize(string settingsURI)
		{
			Settings mySettings = Settings.Load(settingsURI);
			timestep = mySettings.Timestep;
			nextTimestep = timestep;
		}

		//---------------------------------------------------------------------

		///<summary>
		/// Add the site variables used by plug-in to landscape.
		///</summary>
		public void AddSiteVars(ILandscape landscape)//Design decision needed here.
		{
			landscape.Add(timeSinceLastWind);
			this.landscape = landscape;
		}

		//---------------------------------------------------------------------

		///<summary>
		/// Add ecoregion variables used by plug-in to landscape.
		///</summary>
		public void AddEcoregionVars(IEcoregion ecoregions)
		{
			//Ecoregion class derived from generic Regions class
			ecoregions.AddEcoregionVar(windProbability);
			ecoregions.AddEcoregionVar(maxWindSize);
			ecoregions.AddEcoregionVar(minWindSize);
			this.ecoregions = ecoregions;
		}

		//---------------------------------------------------------------------
		///<summary>
		/// The next model timestep that the plug-in should be run.
		///</summary>
		public int NextTimeToRun {
			get {
				return nextTimeToRun;
			}
		}

		//---------------------------------------------------------------------

		///<summary>
		/// Initialize the plug-in in preparation for running it at a
		/// particular timestep.
		///</summary>
		public void InitializeDisturbance(int timestep)
		{
			//Delete or insert default
		}

		//---------------------------------------------------------------------

		///<summary>
		/// Run the plug-in at a particular timestep.
		///</summary>
		public void Run(int currentTimestep)
		{
			nextTimestep += this.timestep;
			
			//create new landscape, windDisturbMap, with one int: windClass
			//initialize windDisturbMap:  non-active = 0, active = 1;
			Landscape windClassMap = new Landscape(this.landscape);
			SiteVariable<int> windClassVar = new SiteVariable<int>("windClass");
			windClassMap.Add(windClassVar);
			foreach (ActiveSite site in windClassMap)
				windClassVar[site] = 1;

			foreach (ActiveSite mySite in landscape) {
			
				Ecoregion ecoregion = Ecoregion[site];
				//getWindProbability(ecoregionIndex)					//calculate numberWindSites:  int(cellSize [m^2] / 10000 * actualWindSize)
				//if (windProbability > randomNumber) 
					int maxWindSz = maxWindSize[ecoregion];
					//getMinWindSize(ecoregionIndex)
					//calculate random actualWindSize from maxWindSize and minWindSize
					//calculate number of numberWindSites  
					int(cellarea / 10000 * actualWindSize);
					if(Spread(mySite, numberWindSites));
						{
						timeOfLastWind[site] = currentTimeStep;
						siteDisturbed(site);  //Jimm: define this somewhere.
						}
				
			}
			//Write final wind event log
			//Write windClassMap
		}
		
		private bool Spread(ActiveSite site, int numberWindSites)
		{
			//int numCohortsKilled = 0;
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
					//numCohortsKilled[ecoregionIndex] = Damage(nextSite, windStrength)
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
			
		}
		
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

	}
}
