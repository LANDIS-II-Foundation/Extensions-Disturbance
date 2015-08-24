using Edu.Wisc.Forest.Flel.Util;
using Landis.SpatialModeling;
using Landis.Library.LandUses;
using System.Collections.Generic;

namespace Landis.Extension.BaseHarvest
{
    /// <summary>
    /// A base class that represents a method for computing stand rankings.
    /// </summary>
    public abstract class StandRankingMethod
        : IStandRankingMethod
    {
        private List<IRequirement> requirements;

        //---------------------------------------------------------------------

        protected StandRankingMethod()
        {
			requirements = new List<IRequirement>();
        }
		
		//---------------------------------------------------------------------
		
		/// <summary>
		/// Show list of requirements
		/// </summary>
		
		public List<IRequirement> Requirements {
			get {
				return requirements;
			}
		}

        //---------------------------------------------------------------------

        /// <summary>
        /// Computes the rank for a stand.
        /// </summary>
        protected abstract double ComputeRank(Stand stand, int i);

        //---------------------------------------------------------------------

        protected virtual void InitializeForRanking(List<Stand> stands, int standCount)
        {
        }

        //---------------------------------------------------------------------

        void IStandRankingMethod.AddRequirement(IRequirement requirement)
        {
            requirements.Add(requirement);
        }

        //---------------------------------------------------------------------

        void IStandRankingMethod.RankStands(List<Stand> stands, StandRanking[] rankings) 
        {
            InitializeForRanking(stands, stands.Count);
            for (int i = 0; i < stands.Count; i++) {
                Stand stand = stands[i];
                double rank = 0;
                if (! stand.IsSetAside) {
                    //check if stand meets all the ranking requirements
                    bool meetsAllRequirements = true;
                    foreach (IRequirement requirement in requirements) {
                        if (! requirement.MetBy(stand)) {
                            meetsAllRequirements = false;
							//set stand rank to 0
							rankings[i].Rank = 0;
                            break;
                        }
                    }
					
                    //if the stand meets all the requirements and is not set-aside,, get its rank
                    if (meetsAllRequirements) {
                        rank = ComputeRank(stand, i);
                    }
                    //otherwise, rank it 0 (so it will not be harvested.)
                    else {
                        rank = 0;
                        //PlugIn.ModelCore.UI.WriteLine("   Stand {0} did not meet its requirements.", stand.MapCode);
                    }
                }
				else {
					rankings[i].Rank = 0;
				}

                // Hack for land-use: set a stand's rank to 0 if it has at least one site whose land use doesn't allow harvesting
                // Really intended for case where each stand has one site.
                foreach (ActiveSite site in stand)
                {
                    // TO DO: Land-use library should initialize the site variable to have a default land-use
                    //        that allows harvesting (e.g., "forest").
                    bool siteAllowsHarvest = (LandUse.SiteVar == null) || LandUse.SiteVar[site].AllowsHarvest;
                    if (!siteAllowsHarvest)
                    {
                        rank = 0;
                        break;
                    }
                }
                rankings[i].Stand = stand;
                rankings[i].Rank = rank;
                //assign rank to stand
				//PlugIn.ModelCore.UI.WriteLine("   Stand {0} rank = {1}.", rankings[i].Stand.MapCode, rankings[i].Rank);
            }
        }
    }
}