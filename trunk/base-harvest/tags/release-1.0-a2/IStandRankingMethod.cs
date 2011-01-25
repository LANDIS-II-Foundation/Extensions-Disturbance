using System.Collections.Generic;

namespace Landis.Harvest
{
    /// <summary>
    /// A method for ranking stands.
    /// </summary>
    public interface IStandRankingMethod
    {
        /// <summary>
        /// Rank the stands in a management area.
        /// </summary>
        void RankStands(List<Stand>    stands,
                        StandRanking[] rankings);
    }
}
