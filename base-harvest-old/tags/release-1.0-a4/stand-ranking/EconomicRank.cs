using Landis.AgeCohort;
using Landis.Landscape;

namespace Landis.Harvest
{
    /// <summary>
    /// A stand ranking method based on economic ranks
    /// </summary>
    public class EconomicRank
        : StandRankingMethod
    {
        private EconomicRankTable rankTable;

        //---------------------------------------------------------------------

        public EconomicRank(EconomicRankTable rankTable)
        {
            this.rankTable = rankTable;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Computes the rank for a stand.
        /// </summary>
        protected override double ComputeRank(Stand stand)
        {
            double standEconImportance = 0.0;

            foreach (ActiveSite site in stand) {
                double siteEconImportance = 0.0;
                foreach (ICohort cohort in Model.LandscapeCohorts[site]) {
                    EconomicRankParameters rankingParameters = rankTable[cohort.Species];
                    if (rankingParameters.MinimumAge > 0 &&
                        rankingParameters.MinimumAge <= cohort.Age)
                        siteEconImportance += (double) rankingParameters.Rank / rankingParameters.MinimumAge * cohort.Age;
                }
                standEconImportance += siteEconImportance;
            }
            standEconImportance /= stand.SiteCount;

            return standEconImportance;
        }
    }
}
