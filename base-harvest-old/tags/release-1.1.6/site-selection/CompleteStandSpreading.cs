using Edu.Wisc.Forest.Flel.Util;
using Landis.Landscape;

using System.Collections;
using System.Collections.Generic;

namespace Landis.Harvest {
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
        public CompleteStandSpreading(double targetSize) {
            this.targetSize = targetSize;
        }

        //---------------------------------------------------------------------

        double ISiteSelector.AreaSelected {
            get {
                return areaSelected;
            }
        }

        //---------------------------------------------------------------------

        IEnumerable<ActiveSite> ISiteSelector.SelectSites(Stand stand) {
            initialStand = stand;
            return this;
        }

        //---------------------------------------------------------------------

        IEnumerator<ActiveSite> IEnumerable<ActiveSite>.GetEnumerator() {
            //get area of initial stand
            areaSelected = initialStand.ActiveArea;
            
            //mark the initial area as harvested
            initialStand.MarkAsHarvested();
            
            //clear unharvestedNeighbors list
            UnharvestedNeighbors.Clear();
            
            //add all of this stand's neighbors to the unharvestedNeighbor list
            foreach (Stand n_stand in initialStand.Neighbors) {
                if (!UnharvestedNeighbors.Contains(n_stand) && !n_stand.Harvested) {
                    UnharvestedNeighbors.Add(n_stand);
                }
            }
            
            //mark this stand's event id
            initialStand.EventId = PlugIn.EventId;
            
            //save this stand's prescription name and give it to the stands that it spreads to
            string prescription_name = initialStand.PrescriptionName;
            
            //increment global event id number
            PlugIn.EventId++;
            
            foreach (ActiveSite site in initialStand)
                yield return site;
            
            //now check if that area is enough to meet the target size
            if (areaSelected >= targetSize)
                yield break;

            //because area selected isn't at the target size yet, add unharvested
            //neighbors (immediate neighbor)
            List<StandRanking> neighborRankings = new List<StandRanking>();
            
            AddUnharvestedNeighbors(initialStand, neighborRankings);
            
            //if(neighborRankings.Count > 0)
            //UI.WriteLine("  Number neighbors={0}.  FirstN Rank={1}", neighborRankings.Count, neighborRankings[0].Rank);

            //loop until neighbor-rankings list is empty or until the highest ranked = 0
            while (neighborRankings.Count > 0 && neighborRankings[0].Rank > 0) {
            
                //get highest ranked neighhbor from ranking list
                Stand highestRankedNeighbor = neighborRankings[0].Stand;
                
                double saveRank = neighborRankings[0].Rank;

                //then remove that neighbor from the list
                neighborRankings.RemoveAt(0);
                if (!highestRankedNeighbor.IsSetAside) {
                
                
                    HarvestedNeighbors.Add(highestRankedNeighbor);
                    
                    //add the area of the neighbor to the total areaSelected
                    areaSelected += highestRankedNeighbor.ActiveArea;
                    
                    //add all of this stand's neighbors to the unharvestedNeighbor list
                    foreach (Stand n_stand in highestRankedNeighbor.Neighbors) {
                        if (!UnharvestedNeighbors.Contains(n_stand) && !n_stand.Harvested) {
                            UnharvestedNeighbors.Add(n_stand);
                        }
                    }

                    //finally mark that neighbor as harvested
                    //mark this stand's event id (use -1 because it was incremented earlier)
                    highestRankedNeighbor.MarkAsHarvested();
                    highestRankedNeighbor.HarvestedRank = saveRank;
                    highestRankedNeighbor.EventId = PlugIn.EventId - 1;
                    highestRankedNeighbor.PrescriptionName = prescription_name;
                    
                    //take this stand off of the unharvestedNeighbor list
                    UnharvestedNeighbors.Remove(highestRankedNeighbor);
                    
                    foreach (ActiveSite site in highestRankedNeighbor)
                        yield return site;

                    if (areaSelected >= targetSize)
                        yield break;
                                        
                    //add unharvested neighbors OF THE HIGHEST RANKED NEIGHBOR because 
                    //areaSelected is still not yet at the targeted size (secondary neighbor).
                    AddUnharvestedNeighbors(highestRankedNeighbor, neighborRankings);
                }
            }
        }

        //---------------------------------------------------------------------

        IEnumerator IEnumerable.GetEnumerator() {
            return ((IEnumerable<ActiveSite>) this).GetEnumerator();
        }
    }
}
