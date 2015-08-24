using Edu.Wisc.Forest.Flel.Util;
using Landis.Landscape;

using System.Collections;
using System.Collections.Generic;

namespace Landis.Harvest
{
    /// <summary>
    /// A site-selection method that harvests complete stands until a target
    /// size is reached.
    /// </summary>
    public class CompleteStandSpreading
        : StandSpreading, ISiteSelector, IEnumerable<ActiveSite>
    {
        private Stand initialStand;
        private double targetSize;
        private double areaSelected;

        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="targetSize">
        /// The target size (area) to harvest.  Units: hectares.
        /// </param>
        public CompleteStandSpreading(double targetSize)
        {
            this.targetSize = targetSize;
        }

        //---------------------------------------------------------------------

        double ISiteSelector.AreaSelected
        {
            get {
                return areaSelected;
            }
        }

        //---------------------------------------------------------------------

        IEnumerable<ActiveSite> ISiteSelector.SelectSites(Stand stand)
        {
            initialStand = stand;
            return this;
        }

        //---------------------------------------------------------------------

        IEnumerator<ActiveSite> IEnumerable<ActiveSite>.GetEnumerator()
        {
            areaSelected = initialStand.ActiveArea;
            initialStand.MarkAsHarvested();
            foreach (ActiveSite site in initialStand)
                yield return site;

            if (areaSelected >= targetSize)
                yield break;

            List<StandRanking> neighborRankings = new List<StandRanking>();
            AddUnharvestedNeighbors(initialStand, neighborRankings);

            while (neighborRankings.Count > 0) {
                Stand highestRankedNeighbor = neighborRankings[0].Stand;
                neighborRankings.RemoveAt(0);
                HarvestedNeighbors.Add(highestRankedNeighbor);

                areaSelected += highestRankedNeighbor.ActiveArea;
                highestRankedNeighbor.MarkAsHarvested();
                foreach (ActiveSite site in highestRankedNeighbor)
                    yield return site;

                if (areaSelected >= targetSize)
                    yield break;
                AddUnharvestedNeighbors(highestRankedNeighbor, neighborRankings);
            }
        }

        //---------------------------------------------------------------------

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<ActiveSite>) this).GetEnumerator();
        }
    }
}
