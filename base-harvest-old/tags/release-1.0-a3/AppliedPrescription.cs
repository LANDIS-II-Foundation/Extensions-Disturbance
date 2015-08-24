using Edu.Wisc.Forest.Flel.Util;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

using log4net;

namespace Landis.Harvest
{
    /// <summary>
    /// The application of a prescription to a management area.
    /// </summary>
    public class AppliedPrescription
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly bool isDebugEnabled = log.IsDebugEnabled;

        private Prescription prescription;
        private StandSpreading standSpreadSiteSelector;
        private Percentage percentageToHarvest;  //TODO: change to Percentage0to100 once it's moved to Util lib
        private int beginTime;
        private int endTime;
        private double areaToHarvest;
        private double areaRemainingToHarvest;
        private double areaRemainingRatio;
        private double areaHarvested;

        private StandRanking[] rankings;

        //  Index of the highest-ranked stand that's not yet harvested.
        private int highestUnharvestedStand;

        //---------------------------------------------------------------------

        /// <summary>
        /// The prescription to apply.
        /// </summary>
        public Prescription Prescription
        {
            get {
                return prescription;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The percentage of the management area to be harvested.
        /// </summary>
        public Percentage PercentageToHarvest
        {
            get {
                return percentageToHarvest;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// When the prescription should begin being applied to the management
        /// area.
        /// </summary>
        /// <remarks>
        /// Units: calendar year
        /// </remarks>
        public int BeginTime
        {
            get {
                return beginTime;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// When the prescription should stop being applied to the management
        /// area.
        /// </summary>
        /// <remarks>
        /// Units: calendar year
        /// </remarks>
        public int EndTime
        {
            get {
                return endTime;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The area (hectares) to be harvested by this prescription during
        /// each harvest timestep.
        /// </summary>
        public double AreaToHarvest
        {
            get {
                return areaToHarvest;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The area (hectares) remaining to be harvested by this prescription
        /// during the current timestep.
        /// </summary>
        public double AreaRemainingToHarvest
        {
            get {
                return areaRemainingToHarvest;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The ratio of AreaRemainingToHarvest to AreaToHarvest.
        /// </summary>
        public double AreaRemainingRatio
        {
            get {
                return areaRemainingRatio;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The area (hectares) harvested by this prescription during the
        /// current timestep.
        /// </summary>
        public double AreaHarvested
        {
            get {
                return areaHarvested;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Any stands that haven't been harvested yet during the current
        /// timestep ranked > 0?
        /// </summary>
        public bool AnyUnharvestedStandsRankedAbove0
        {
            get {
                //  Search for the highest unharvested stand with rank > 0
                while (highestUnharvestedStand < rankings.Length) {
                    if (rankings[highestUnharvestedStand].Rank <= 0)
                        //  All the remaining stands are ranked 0 from this
                        //  point on in the rankings.
                        return false;
                    if (rankings[highestUnharvestedStand].Stand.Harvested)
                        highestUnharvestedStand++;
                    else
                        //  Found an unharvested stand whose rank > 0
                        return true;
                }
                //  End of rankings array so no more unharvested stands
                return false;
            }
        }

        //---------------------------------------------------------------------

        public AppliedPrescription(Prescription   prescription,
                                   ManagementArea managementArea,
                                   Percentage     percentageToHarvest,
                                   int            beginTime,
                                   int            endTime)
        {
            this.prescription = prescription;
            this.standSpreadSiteSelector = prescription.SiteSelectionMethod as StandSpreading;
            this.percentageToHarvest = percentageToHarvest;
            this.areaToHarvest = managementArea.Area * percentageToHarvest;
            this.beginTime = beginTime;
            this.endTime = endTime;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Finish initializing the applied prescription after reading the
        /// stand map.
        /// </summary>
        /// <param name="standCount">
        /// The number of stands in the management area that the prescription
        /// is applied to.
        /// </param>
        public void FinishInitialization(int standCount)
        {
            rankings = new StandRanking[standCount];
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes the prescription for harvesting, which includes
        /// computing stand rankings.
        /// </summary>
        /// <remarks>
        /// AreaRemainingToHarvest is set to AreaToHarvest, AreaRemainingRatio
        /// is set to 1.0, and AreaHarvested is set to 0.0.
        /// </remarks>
        public void InitializeForHarvest(List<Stand> stands)
        {
            areaRemainingToHarvest = areaToHarvest;
            areaRemainingRatio = 1.0;
            areaHarvested = 0.0;

            prescription.StandRankingMethod.RankStands(stands, rankings);
            // Sort rankings highest to lowest
            Array.Sort<StandRanking>(rankings, CompareRankings);

            highestUnharvestedStand = 0;

            if (isDebugEnabled) {
                log.DebugFormat("prescription {0}:", prescription.Name);
                log.DebugFormat("  _Ranking_  Stand");
                foreach (StandRanking ranking in rankings)
                    log.DebugFormat("  {0,9}  {1,5}", ranking.Rank, ranking.Stand.MapCode);
            }

            if (standSpreadSiteSelector != null)
                standSpreadSiteSelector.StandRankings = rankings;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Compares two stand rankings such that the higher ranking comes
        /// before the lower ranking.
        /// </summary>
        public static int CompareRankings(StandRanking x,
                                          StandRanking y)
        {
            if (x.Rank > y.Rank)
                return -1;
            else if (x.Rank < y.Rank)
                return 1;
            else
                return 0;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Harvests the highest-ranked stand which hasn't been harvested yet
        /// during the current timestep.
        /// </summary>
        public void HarvestHighestRankedStand()
        {
            Debug.Assert(AnyUnharvestedStandsRankedAbove0);

            Stand stand = rankings[highestUnharvestedStand].Stand;
            if (isDebugEnabled)
                log.DebugFormat("Harvesting stand {0} ...", stand.MapCode);
            double harvestedArea = prescription.Harvest(stand);
            areaHarvested += harvestedArea;
            if (harvestedArea >= areaRemainingToHarvest)
                areaRemainingToHarvest = 0;
            else
                areaRemainingToHarvest -= harvestedArea;
            areaRemainingRatio = areaRemainingToHarvest / areaToHarvest;
        }
    }
}
