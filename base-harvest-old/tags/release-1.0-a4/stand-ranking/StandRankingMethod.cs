using System.Collections.Generic;

namespace Landis.Harvest
{
    /// <summary>
    /// A base class that represents a method for computing stand rankings.
    /// </summary>
    public abstract class StandRankingMethod
        : IStandRankingMethod
    {
        private List<IRankingRequirement> requirements;

        //---------------------------------------------------------------------

        protected StandRankingMethod()
        {
            requirements = new List<IRankingRequirement>();
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Computes the rank for a stand.
        /// </summary>
        protected abstract double ComputeRank(Stand stand);

        //---------------------------------------------------------------------

        void IStandRankingMethod.AddRequirement(IRankingRequirement requirement)
        {
            requirements.Add(requirement);
        }

        //---------------------------------------------------------------------

        void IStandRankingMethod.RankStands(List<Stand> stands,
                                            StandRanking[] rankings)
        {
            for (int i = 0; i < stands.Count; i++) {
                Stand stand = stands[i];
                double rank = 0;
                if (! stand.IsSetAside) {
                    bool meetsAllRequirements = true;
                    foreach (IRankingRequirement requirement in requirements) {
                        if (! requirement.MetBy(stand)) {
                            meetsAllRequirements = false;
                            break;
                        }
                    }
                    if (meetsAllRequirements)
                        rank = ComputeRank(stand);
                }
                rankings[i].Stand = stand;
                rankings[i].Rank = rank;
            }
        }
    }
}
