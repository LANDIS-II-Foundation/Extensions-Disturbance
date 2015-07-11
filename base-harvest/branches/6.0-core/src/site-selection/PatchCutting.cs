using Edu.Wisc.Forest.Flel.Util;
using System.Collections;
using System.Collections.Generic;
using Wisc.Flel.GeospatialModeling.Landscapes;
using Wisc.Flel.GeospatialModeling.Grids;

namespace Landis.Harvest
{
    /// <summary>
    /// A site-selection method that selects small non-contiguous collections
    /// of sites within a stand.
    /// </summary>
    public class PatchCutting
        : StandSpreading, ISiteSelector, IEnumerable<ActiveSite>
    {

        //stand to work in
        private Stand stand;
        //percent of stand to harvest
        private double percent;
        //harvest patch sizes
        private int patch_size;
        //total area already selected
        private int areaSelected;

        //define 8 neighboring locations
        //up and down
        private RelativeLocation up;
        private RelativeLocation down;
        //left and right
        private RelativeLocation left;
        private RelativeLocation right;
        //up left & right
        private RelativeLocation up_left;
        private RelativeLocation up_right;
        //down left & right
        private RelativeLocation down_left;
        private RelativeLocation down_right;

        //collect all 8 relative neighbor locations in array
        private RelativeLocation[] all_neighbor_locations;

        public static void ValidatePercentage(InputValue<Percentage> percentage)
        {
            if (percentage.Actual < 0 || percentage.Actual > 1.0)
                throw new InputValueException(percentage.String,
                                              percentage.String + " is not between 0% and 100%");
        }

        //---------------------------------------------------------------------

        public static void ValidateSize(InputValue<int> size)
        {
            if (size.Actual < 0)
                throw new InputValueException(size.String,
                                              "Patch size cannot be negative");
        }

        //constructor
        public PatchCutting(Percentage percentage, int size) {
            this.percent = (double) percentage;
            this.patch_size = size;

            //define 8 neighboring locations
            //up and down
            up = new RelativeLocation(-1, 0);
            down = new RelativeLocation( 1, 0);
            //left and right
            left = new RelativeLocation( 0, -1);
            right = new RelativeLocation( 0, 1);
            //up left & right
            up_left = new RelativeLocation(-1, -1);
            up_right = new RelativeLocation(-1, 1);
            //down left & right
            down_left = new RelativeLocation(1, -1);
            down_right = new RelativeLocation(1, 1);

            //collect all 8 relative neighbor locations in array
            all_neighbor_locations = new RelativeLocation[]{up, down, left,
                    right, up_left, up_right, down_left, down_right};

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
            this.stand = stand;
            return this;
        }

        //---------------------------------------------------------------------

        IEnumerator<ActiveSite> IEnumerable<ActiveSite>.GetEnumerator() {
            //UI.WriteLine("\nPatchCutting on stand {0}", stand.MapCode);
            //mark this stand as harvested
            stand.MarkAsHarvested();
            //mark this stand's event id
            stand.EventId = PlugIn.EventId;
            //increment global event id number
            PlugIn.EventId++;

            //initialize areaSelected to 0
            areaSelected = 0;
            //initialize total_areaSelected to 0
            double total_areaSelected = 0;
            //get number of patches required: (+ 0.5 is to round up)
            //int num_patches = (int) (((double) stand.SiteCount / (double) patch_size) * percent + 0.5);
            //make sure at least 1 patch.  if the patch size is too big, the total_areaSelected counter will stop it before it goes over the desired percentage of cells.
            //num_patches = System.Math.Max(1, num_patches);
            //UI.WriteLine("\nSTAND {0}", stand.MapCode);
            //UI.WriteLine("ALLOWABLE AREA = {0}", stand.SiteCount * percent);

            //get list of this stand's sites
            List<ActiveSite> sites = stand.GetSites();
            //get a random site from the stand
            int random = (int) (Landis.Util.Random.GenerateUniform() * (stand.SiteCount - 1));
            //start with this site (if it's active)
            ActiveSite current_site = sites[random];

            //queue to hold sites to harvest
            Queue<ActiveSite> sitesToConsider = new Queue<ActiveSite>();
            //put initial pivot site on queue
            sitesToConsider.Enqueue(current_site);

            //queue to hold sites that were already harvested and taken off the queue
            Queue<ActiveSite> sitesToHarvest = new Queue<ActiveSite>();

            //loop through stand, harvesting patches of size patch_size at a time
            while (total_areaSelected < (stand.SiteCount * percent)) {
                while (areaSelected < patch_size && total_areaSelected < (stand.SiteCount * percent)) {

                    //loop through the site's neighbors enqueueing them too
                    foreach (RelativeLocation loc in all_neighbor_locations) {
                        //get a neighbor site (if it's non-null and active)
                        if (current_site.GetNeighbor(loc) != null && current_site.GetNeighbor(loc).IsActive) {
                            ActiveSite neighbor_site = (ActiveSite) current_site.GetNeighbor(loc);
                            //check if it's a valid neighbor:
                            // (if it's not null, in the same stand and management area, and not already in either of the queues)
                            if (SiteVars.Stand[neighbor_site] == SiteVars.Stand[current_site] &&
                                        SiteVars.ManagementArea[neighbor_site] == SiteVars.ManagementArea[current_site] &&
                                        !sitesToConsider.Contains(neighbor_site) && !sitesToHarvest.Contains(neighbor_site)) {
                                //then enqueue the neighbor
                                sitesToConsider.Enqueue(neighbor_site);
                            }
                        }
                    }
                    //check if there's anything left on the queue
                    if (sitesToConsider.Count > 1) {
                        //now after looping through all of the current site's neighbors
                        //dequeue the current site and put it on the sitesToHarvest queue (used later)
                        sitesToHarvest.Enqueue(sitesToConsider.Dequeue());
                        //increment area selected and total_areaSelected
                        areaSelected += (int) Model.Core.CellArea;
                        total_areaSelected += (int) Model.Core.CellArea;
                        //and set the new current_site to the head of the queue (by peeking)
                        current_site = sitesToConsider.Peek();
                    }
                    //get another site from the queue
                    else if (sitesToConsider.Count == 1) {
                        current_site = sitesToConsider.Peek();
                        //increment area selected and total_areaSelected
                        areaSelected += (int) Model.Core.CellArea;
                        total_areaSelected += (int) Model.Core.CellArea;
                        sitesToHarvest.Enqueue(sitesToConsider.Dequeue());
                    }
                    //else just break out- if it's not big enough it's not big enough
                    else {
                        //UI.WriteLine("patch isn't big enough ({0}), must BREAK.", areaSelected);
                        break;
                    }

                }
                //clear the sitesToConsider queue to get rid of old sites
                sitesToConsider.Clear();
                //get a new random site to start at (one that hasn't been put on the sitesToHarvest queue yet)
                //only allow a site-count # of tries
                bool found_site = false;
                int tries = 0;
                while (!found_site && tries < stand.SiteCount) {
                    //increment the number of tries
                    tries++;
                    random = (int) (Landis.Util.Random.GenerateUniform() * (stand.SiteCount - 1));
                    current_site = sites[random];
                    //if it's not on the sitesToHarvest queue already
                    if (!sitesToHarvest.Contains(current_site)) {
                        //now put this site on for consideration (which will later be put onto the sitesToHarvest queue).
                        sitesToConsider.Enqueue(current_site);
                        found_site = true;
                    }
                }
                //if the site isn't found (prev loop was broken because of too many tries) then break from this stand entirely
                if (!found_site) {
                    break;
                }
                //reset areaSelected = 0 to start over
                areaSelected = 0;
                //UI.WriteLine("total_areaSelected = {0}", total_areaSelected);
            }

            //finish off by returning all the sitesToHarvest
            while (sitesToHarvest.Count > 0) {
                yield return sitesToHarvest.Dequeue();
            }
        }

        //---------------------------------------------------------------------

        IEnumerator IEnumerable.GetEnumerator() {
            return ((IEnumerable<ActiveSite>) this).GetEnumerator();
        }

        //---------------------------------------------------------------------
    }
}
