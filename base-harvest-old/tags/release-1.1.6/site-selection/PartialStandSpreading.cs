using Edu.Wisc.Forest.Flel.Grids;
using Edu.Wisc.Forest.Flel.Util;
using Landis.Landscape;

using System.Collections;
using System.Collections.Generic;

namespace Landis.Harvest
{
    /// <summary>
    /// A site-selection method that harvests site-by-site until a target
    /// size is reached.
    /// </summary>
    public class PartialStandSpreading
        : StandSpreading, ISiteSelector, IEnumerable<ActiveSite>
    {
        private Stand initialStand;
        private double targetSize;
        private int areaSelected;
        
        //collect all 8 relative neighbor locations in array
        public static RelativeLocation[] all_neighbor_locations = new RelativeLocation[]
        {
            //define 8 neighboring locations
            new RelativeLocation(-1, 0),
            new RelativeLocation( 1, 0),
            new RelativeLocation( 0, -1),
            new RelativeLocation( 0, 1),
            new RelativeLocation(-1, -1),
            new RelativeLocation(-1, 1),
            new RelativeLocation(1, -1),
            new RelativeLocation(1, 1)
        };

        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="targetSize">
        /// The target size (area) to harvest.  Units: hectares.
        /// </param>
        public PartialStandSpreading(double targetSize)
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

        IEnumerator<ActiveSite> IEnumerable<ActiveSite>.GetEnumerator() {
            //make and maintain a list of unharvested neighbors:
                //each time a stand is added to the list, put its neighbors on the unharvestedNeighbor list
                //each time a stand is harvested, take it off of the unharvestedNeighbor list
            
            //queue to hold sites to harvest
            Queue<ActiveSite> sitesToConsider = new Queue<ActiveSite>();
            
            //queue to hold sites that were already harvested and taken off the queue
            Queue<ActiveSite> sitesToHarvest = new Queue<ActiveSite>();

            //get area of initial stand
            areaSelected = (int) initialStand.ActiveArea;
            
            //mark the initial stand as harvested because we will at least harvest parts of this stand.
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

            //maintain a list of harvested stands, including initial
            List<Stand> HarvestedStands = new List<Stand>();
            HarvestedStands.Add(initialStand);
                
            //get a list of this stand's neighbors' rankings
            List<StandRanking> neighborRankings = new List<StandRanking>();
            
            //and add those rankings to this stand's unharvested-neighbor list
            AddUnharvestedNeighbors(initialStand, neighborRankings);

            if (neighborRankings.Count == 0 || neighborRankings[0].Rank <= 0)
                yield break;
            
            //UI.WriteLine("this stand has {0} ranked neighbors left.", neighborRankings.Count);

            //get list of this stand's sites
            // List<ActiveSite> sites = initialStand.GetSites();
            //get a random site from the list that is also a neighbor:
            // int random = (int) (Landis.Util.Random.GenerateUniform() * (initialStand.SiteCount - 1));
            // ActiveSite current_site = sites[random];

            Stand highestRankedNeighbor = neighborRankings[0].Stand;

            //then remove that neighbor from the list
            neighborRankings.RemoveAt(0);
            
            //get a neighboring (shared edge) site from this neighboring stand
            ActiveSite current_site = GetNeighboringSite(HarvestedStands, highestRankedNeighbor); 

            //put initial pivot site on queue
            if(current_site != null && !highestRankedNeighbor.IsSetAside)
            {
                sitesToConsider.Enqueue(current_site);
                                    
                //and add this neighbor to harvested list
                HarvestedNeighbors.Add(highestRankedNeighbor);
                HarvestedStands.Add(highestRankedNeighbor);
                                    
                //finally mark that neighbor as harvested
                highestRankedNeighbor.MarkAsHarvested();
                highestRankedNeighbor.EventId = PlugIn.EventId - 1;
                highestRankedNeighbor.PrescriptionName = prescription_name;
                
            }

                
            
            //loop until target size is reached or stand is completely harvested
            while (areaSelected < targetSize) {
            
                if (current_site != null)
                {
                    //loop through the site's neighbors enqueueing them too
                    foreach (RelativeLocation loc in all_neighbor_locations) {
                        //get a neighbor site (if it's active and non-null)
                        if (current_site.GetNeighbor(loc) != null 
                        && current_site.GetNeighbor(loc).IsActive) 
                        {
                            ActiveSite neighbor_site = (ActiveSite) current_site.GetNeighbor(loc);
                        
                            // check if in the same stand and management area, and not already
                            // in either of the queues)
                            if (SiteVars.Stand[neighbor_site] == SiteVars.Stand[current_site] 
                            && SiteVars.ManagementArea[neighbor_site] == SiteVars.ManagementArea[current_site]
                            && !sitesToConsider.Contains(neighbor_site) 
                            && !sitesToHarvest.Contains(neighbor_site)) 
                            {
                                    
                            //then enqueue the neighbor
                                sitesToConsider.Enqueue(neighbor_site);
                            //UI.WriteLine("enqueueing site at location {0}", neighbor_site.Location);
                            }
                        }
                    }
                }
                //check if there's anything left on the queue
                if (sitesToConsider.Count > 1) 
                {
                    
                    //now after looping through all of the current site's neighbors
                    //dequeue the current site and put it on the sitesToHarvest queue (used later)
                    sitesToHarvest.Enqueue(sitesToConsider.Dequeue());
                    
                    //increment area selected
                    areaSelected += (int) Model.Core.CellArea;
                    
                    //and set the new current_site to the head of the queue (by peeking)
                    current_site = sitesToConsider.Peek();
                }
                //if there's no more sites in consideration, go to highest ranked neighbor
                else 
                {
                    //loop until neighborRankings is empty or highest-ranked neighbor is ranked 0.
                    if (neighborRankings.Count > 0 && neighborRankings[0].Rank > 0) {
                        
                        //get NEXT highest ranked neighhbor from ranking list
                        highestRankedNeighbor = neighborRankings[0].Stand;
                        //UI.WriteLine("getting neighbor {0}", neighborRankings[0].Stand.MapCode);
                        
                        double saveRank = neighborRankings[0].Rank;
                        
                        //then remove that neighbor from the list
                        neighborRankings.RemoveAt(0);
                        
                        //get a neighboring (shared edge) site from this neighboring stand
                        current_site = GetNeighboringSite(HarvestedStands, highestRankedNeighbor); 

                        //put initial pivot site on queue
                        if(current_site != null && !highestRankedNeighbor.IsSetAside)
                        {
                            sitesToConsider.Enqueue(current_site);

                            //add all of this stand's neighbors to the unharvestedNeighbor list
                            foreach (Stand n_stand in highestRankedNeighbor.Neighbors) {
                                if (!UnharvestedNeighbors.Contains(n_stand) && !n_stand.Harvested) {
                                    UnharvestedNeighbors.Add(n_stand);
                                }
                            }
                        
                            //take this stand off of the unharvestedNeighbor list
                            UnharvestedNeighbors.Remove(highestRankedNeighbor);

                            //add unharvested neighbors OF THE HIGHEST RANKED NEIGHBOR because 
                            //areaSelected is still not yet at the targeted size (secondary neighbor).
                            AddUnharvestedNeighbors(highestRankedNeighbor, neighborRankings);

                            //and add this neighbor to harvested list
                            HarvestedNeighbors.Add(highestRankedNeighbor);
                            HarvestedStands.Add(highestRankedNeighbor);
                                    
                            //finally mark that neighbor as harvested
                            highestRankedNeighbor.MarkAsHarvested();
                            highestRankedNeighbor.EventId = PlugIn.EventId - 1;
                            highestRankedNeighbor.PrescriptionName = prescription_name;
                            highestRankedNeighbor.HarvestedRank = saveRank;
                        }
                        
                        //now do site initializer stuff for this stand
                        //get list of this stand's sites
                        /*sites = highestRankedNeighbor.GetSites();
                        
                        //get an initial site from this neighboring stand
                        bool found_edge = false;
                        while (!found_edge) {
                            //get random site from list
                            random = (int) (Landis.Util.Random.GenerateUniform() * (sites.Count - 1));
                            current_site = sites[random];
                            //check if one of its neighbors is on the edge of this stand and initialStand
                            foreach (RelativeLocation loc2 in all_neighbor_locations) {
                                //if it's a valid site and is on the edge
                                if (current_site.GetNeighbor(loc2) != null && 
                                        SiteVars.Stand[current_site.GetNeighbor(loc2)] == initialStand) {
                                    //set flag = true to break loop
                                    //UI.WriteLine("FOUND EDGE SITE");
                                    found_edge = true;
                                }
                            }
                        }*/

                        //now we're ready to keep going in the loop
                    }
                    else {
                        //if there are no neighbors left to keep harvesting
                        //then break out of the loop and give up
                        // ("if it's not big enough, it's not big enough")
                        //UI.WriteLine("NOT BIG ENOUGH");
                        break;
                    }
                }
            }
            
            //UI.WriteLine("FINAL AREA SELECTED = {0}\n\n", areaSelected);
            
            //finish off by returning all the sitesToHarvest
            while (sitesToHarvest.Count > 0) {
                yield return sitesToHarvest.Dequeue();
            }
        }
        
        private static ActiveSite GetNeighboringSite(List<Stand> harvestedNeighbors, Stand neighborStand)
        {
            //get an shared-edge site from any one of the previously harvested neighboring stands
            
            ActiveSite returnSite = null;
            
            while (returnSite == null) 
            {
                foreach(Site current_site in neighborStand)
                {
                    //check if one of its neighbors is on the edge of the initialStand
                    foreach (RelativeLocation relloc in all_neighbor_locations) 
                    {
                    
                        //if it's a valid site and is on the edge
                        if (current_site.GetNeighbor(relloc) != null && current_site.GetNeighbor(relloc).IsActive)
                        {
                            foreach(Stand stand in harvestedNeighbors)
                                if (SiteVars.Stand[current_site.GetNeighbor(relloc)] == stand)
                                    returnSite = (ActiveSite) current_site;
                        }
                    }
                    
                    if(returnSite != null)
                        break;
                }
             }
             
             return returnSite;
        }
        
        //---------------------------------------------------------------------

        IEnumerator IEnumerable.GetEnumerator() {
            return ((IEnumerable<ActiveSite>) this).GetEnumerator();
        }
    }
}