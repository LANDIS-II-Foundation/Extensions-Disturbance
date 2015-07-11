using System.Collections.Generic;

namespace Landis.Harvest
{
    /// <summary>
    /// A stand ranking method based on economic ranks
    /// </summary>
    public class EconomicRank
        : IStandRankingMethod
    {
        public EconomicRank()
        {
        }

        //---------------------------------------------------------------------

        void IStandRankingMethod.RankStands(List<Stand>    stands,
                                            StandRanking[] rankings)
        {
            int i = 0;
            foreach (Stand stand in stands)
            {
                double standEconImportance = 0.0;
//                foreach (ActiveSite site in stand)
//                {
//                    double siteEconImportance = 0.0;
//                    foreach (AgeCohort cohort in site)
//                    {
//                        if (cohort.Age > 0 && cohort.Age > Prescription.EconomicTable[cohort.Species].MinimumAge)
//                            siteEconImportance += Prescription.EconomicTable[cohort.Species].Rank / Prescription.EconomicTable[cohort.Species].MinimumAge * cohort.Age;
//                    }
//                    standEconImportance += siteEconImportance;
//                }
//                standEconImportance /= stand.SiteCount;

                StandRanking ranking = new StandRanking();
                ranking.Stand = stand;
                ranking.Rank = standEconImportance;
                rankings[i] = ranking;
            }
        }
    }
}
