using Landis.Succession;
using System.Collections.Generic;

namespace Landis.Harvest
{
    /// <summary>
    /// A repeat harvest is a variation of a prescription that harvests stands
    /// and then sets them aside for additional harvests.
    /// </summary>
    public class RepeatHarvest
        : Prescription
    {
        private int interval;
        private StandSpreading spreadingSiteSelector;
        private List<Stand> harvestedStands;

        //---------------------------------------------------------------------

        /// <summary>
        /// The time interval between a stand's initial harvest and each of
        /// its additional harvests.
        /// </summary>
        /// <remarks>
        /// Units: years.
        /// </remarks>
        public int Interval
        {
            get {
                return interval;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The list of stands that were harvested during the most recent call
        /// to the Harvest method.
        /// </summary>
        public List<Stand> HarvestedStands
        {
            get {
                return harvestedStands;
            }
        }

        //---------------------------------------------------------------------

        public RepeatHarvest(string               name,
                             IStandRankingMethod  rankingMethod,
                             ISiteSelector        siteSelector,
                             ICohortSelector      cohortSelector,
                             Planting.SpeciesList speciesToPlant,
                             int                  interval)
            : base(name, rankingMethod, siteSelector, cohortSelector, speciesToPlant)
        {
            this.interval = interval;
            this.spreadingSiteSelector = siteSelector as StandSpreading;
            this.harvestedStands = new List<Stand>();
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Harvests a stand (and possibly its neighbors) according to the
        /// repeat-harvest's site-selection method.
        /// </summary>
        /// <returns>
        /// The area that was harvested (units: hectares).
        /// </returns>
        public override double Harvest(Stand stand)
        {
            double areaHarvested = base.Harvest(stand);

            harvestedStands.Clear();
            harvestedStands.Add(stand);
            if (spreadingSiteSelector != null)
                harvestedStands.AddRange(spreadingSiteSelector.HarvestedNeighbors);

            return areaHarvested;
        }
    }
}
