using Edu.Wisc.Forest.Flel.Grids;
using Edu.Wisc.Forest.Flel.Util;
using Landis.Landscape;

using System.Collections;
using System.Collections.Generic;

namespace Landis.Harvest
{
    /// <summary>
    /// A stand is a collection of sites and represent typical or average
    /// forest management block sizes.
    /// </summary>
    public class Stand
        : IEnumerable<ActiveSite>
    {
        private ushort mapCode;
        private List<Location> siteLocations;
        private double activeArea;
        private ManagementArea mgmtArea;
        private bool harvested;
        private List<Stand> neighbors;
        private ushort age;
        private int yearAgeComputed;
        private int setAsideUntil;

        //---------------------------------------------------------------------

        /// <summary>
        /// The code that designates which sites are in the stand in the stand
        /// input map.
        /// </summary>
        public ushort MapCode
        {
            get {
                return mapCode;
            }
        }

        //---------------------------------------------------------------------

        public int SiteCount
        {
            get {
                return siteLocations.Count;
            }
        }

        //---------------------------------------------------------------------

        public double ActiveArea
        {
            get {
                return activeArea;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The management area that the stand belongs to.
        /// </summary>
        public ManagementArea ManagementArea
        {
            get {
                return mgmtArea;
            }

            internal set {
                mgmtArea = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The stand's age, which is the mean of the oldest cohort on each
        /// site within the stand.
        /// </summary>
        public ushort Age
        {
            get {
                if (yearAgeComputed != Model.Core.CurrentTime) {
                    age = ComputeAge();
                    yearAgeComputed = Model.Core.CurrentTime;
                }
                return age;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Has the stand been harvested during the current timestep?
        /// </summary>
        public bool Harvested
        {
            get {
                return harvested;
            }
        }

        //---------------------------------------------------------------------

        public IEnumerable<Stand> Neighbors
        {
            get {
                return neighbors;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Has the stand been set aside by a repeat harvest?
        /// </summary>
        public bool IsSetAside
        {
            get {
                return Model.Core.CurrentTime <= setAsideUntil;
            }
        }

        //---------------------------------------------------------------------

        public Stand(ushort mapCode)
        {
            this.mapCode = mapCode;
            this.siteLocations = new List<Location>();
            this.activeArea = 0;
            this.mgmtArea = null;
            this.neighbors = new List<Stand>();
            this.yearAgeComputed = Model.Core.StartTime;
            this.setAsideUntil = Model.Core.StartTime;
        }

        //---------------------------------------------------------------------

        private static RelativeLocation neighborAbove = new RelativeLocation(-1, 0);
        private static RelativeLocation neighborLeft  = new RelativeLocation( 0, -1);
        private static RelativeLocation[] neighborsAboveAndLeft = new RelativeLocation[]{ neighborAbove, neighborLeft };

        //---------------------------------------------------------------------

        public void Add(ActiveSite site)
        {
            siteLocations.Add(site.Location);
            activeArea = siteLocations.Count * Model.Core.CellArea;

            //  Add this stand as a neighbor to the stands to the left and
            //  above of the current site.  This assumes that the sites are
            //  being added to the stands by reading the landscape in row
            //  major order (upper left to lower right).
            foreach (RelativeLocation neighborLoc in neighborsAboveAndLeft) {
                Site neighbor = site.GetNeighbor(neighborLoc);
                if (neighbor != null && neighbor.IsActive) {
                    Stand neighborStand = SiteVars.Stand[neighbor];
                    if (this != neighborStand) {
                        AddNeighbor(neighborStand);
                        neighborStand.AddNeighbor(this);
                    }
                }
            }
        }

        //---------------------------------------------------------------------

        protected void AddNeighbor(Stand neighbor)
        {
            Require.ArgumentNotNull(neighbor);
            if (! neighbors.Contains(neighbor))
                neighbors.Add(neighbor);
        }

        //---------------------------------------------------------------------

        public ushort ComputeAge()
        {
            long total = 0;
            foreach (ActiveSite site in this) {
                total += AgeCohort.Util.GetMaxAge(Model.LandscapeCohorts[site]);
            }
            return (ushort) (total / siteLocations.Count);
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes the stand for the current timestep by resetting certain
        /// harvest-related properties.
        /// </summary>
        public void InitializeForHarvesting()
        {
            harvested = false;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Marks the stand as having been harvested.
        /// </summary>
        public void MarkAsHarvested()
        {
            harvested = true;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Sets the stand aside until a later time for a repeat harvest.
        /// </summary>
        /// <param name="year">
        /// The calendar year until which the stand should be stand aside.
        /// </param>
        public void SetAsideUntil(int year)
        {
            setAsideUntil = year;
        }

        //---------------------------------------------------------------------

        IEnumerator<ActiveSite> IEnumerable<ActiveSite>.GetEnumerator()
        {
            foreach (Location location in siteLocations)
               yield return Model.Core.Landscape[location];
        }

        //---------------------------------------------------------------------

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<ActiveSite>) this).GetEnumerator();
        }
    }
}
