using Landis.AgeCohort;
using Wisc.Flel.GeospatialModeling.Landscapes.DualScale;

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
        protected override double ComputeRank(Stand stand, int i)
        {
            double standEconImportance = 0.0;
            //UI.WriteLine("there are {0} sites in this stand.", stand.SiteCount);
            foreach (ActiveSite site in stand) {

                double siteEconImportance = 0.0;
                foreach (ISpeciesCohorts speciesCohorts in Model.LandscapeCohorts[site]) {
                    EconomicRankParameters rankingParameters = rankTable[speciesCohorts.Species];
                    foreach (ICohort cohort in speciesCohorts) {
                        if (rankingParameters.MinimumAge > 0 &&
                            rankingParameters.MinimumAge <= cohort.Age)
                            siteEconImportance += (double) rankingParameters.Rank / rankingParameters.MinimumAge * cohort.Age;
                    }
                }
                standEconImportance += siteEconImportance;
            }
            standEconImportance /= stand.SiteCount;

            return standEconImportance;
        }
    }
}
