using System.Collections.Generic;

namespace Landis.Harvest
{
    /// <summary>
    /// A method for ranking stands.
    /// </summary>
    public interface IStandRankingMethod
    {
        /// <summary>
        /// Adds a requirement which must be satified by a stand in order for
        /// the ranking method to compute its ranking.
        /// </summary>
        /// <remarks>
        /// If a stand does not meet the requirement, its rank is 0.
        /// </remarks>
        void AddRequirement(IRankingRequirement requirement);

        //---------------------------------------------------------------------

        /// <summary>
        /// Rank the stands in a management area.
        /// </summary>
        void RankStands(List<Stand>    stands,
                        StandRanking[] rankings);
    }
}
