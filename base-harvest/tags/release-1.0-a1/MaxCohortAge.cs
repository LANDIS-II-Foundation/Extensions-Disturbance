using System.Collections.Generic;

namespace Landis.Harvest
{
    /// <summary>
    /// A stand ranking method where the oldest stands are harvested first.
    /// </summary>
    public class MaxCohortAge
        : IStandRankingMethod
    {
        public MaxCohortAge()
        {
        }

        //---------------------------------------------------------------------

        void IStandRankingMethod.RankStands(List<Stand> stands,
                                            StandRanking[] rankings)
        {
            for (int i = 0; i < stands.Count; i++) {
                Stand stand = stands[i];
                rankings[i].Stand = stand;
                rankings[i].Rank = stand.Age;
            }
        }
    }
}
