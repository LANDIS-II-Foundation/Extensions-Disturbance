//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:  Robert M. Scheller, James B. Domingo

using Landis.Library.AgeOnlyCohorts;
using Landis.SpatialModeling;

namespace Landis.Extension.BaseHarvest
{
    /// <summary>
    /// A stand ranking method based on economic ranks
    /// </summary>
    public class FireRiskRank
        : StandRankingMethod
    {
        private FireRiskTable rankTable;

        //---------------------------------------------------------------------

        public FireRiskRank(FireRiskTable rankTable)
        {
            this.rankTable = rankTable;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Computes the rank for a stand.
        /// </summary>
        protected override double ComputeRank(Stand stand, int i)
        {

            SiteVars.ReInitialize();
            //if (SiteVars.CFSFuelType == null)
            //    throw new System.ApplicationException("Error: CFS Fuel Type NOT Initialized.  Fuel extension MUST be active.");

            double standFireRisk = 0.0;
            //PlugIn.ModelCore.UI.WriteLine("Base Harvest: EconomicRank.cs: ComputeRank:  there are {0} sites in this stand.", stand.SiteCount);
            foreach (ActiveSite site in stand) {

                //double siteFireRisk = 0.0;
                int fuelType = SiteVars.CFSFuelType[site];
                //PlugIn.ModelCore.UI.WriteLine("Base Harvest: ComputeRank:  FuelType = {0}.", fuelType);
                FireRiskParameters rankingParameters = rankTable[fuelType];
                standFireRisk = (double)rankingParameters.Rank;

                //foreach (ISpeciesCohorts speciesCohorts in SiteVars.Cohorts[site])
                //{
                //    FireRiskParameters rankingParameters = rankTable[speciesCohorts.Species];
                //    foreach (ICohort cohort in speciesCohorts) {
                //        if (rankingParameters.MinimumAge > 0 &&
                //            rankingParameters.MinimumAge <= cohort.Age)
                //            siteEconImportance += (double) rankingParameters.Rank / rankingParameters.MinimumAge * cohort.Age;
                //    }
                //}
                //standEconImportance += siteEconImportance;
            }
            standFireRisk /= stand.SiteCount;

            return standFireRisk;
        }
    }
}
