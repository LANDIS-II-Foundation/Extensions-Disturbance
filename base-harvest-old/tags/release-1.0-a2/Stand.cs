using Edu.Wisc.Forest.Flel.Grids;
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
        private ushort age;
        private int yearAgeComputed;

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

            set {
                harvested = value;
            }
        }

        //---------------------------------------------------------------------

        public IEnumerable<Stand> Neighbors
        {
            get {
                // TODO
                throw new System.NotImplementedException();
            }
        }

        //---------------------------------------------------------------------

        public Stand(ushort mapCode)
        {
            this.mapCode = mapCode;
            this.siteLocations = new List<Location>();
            this.activeArea = 0;
            this.mgmtArea = null;
            this.yearAgeComputed = Model.Core.StartTime;
        }

        //---------------------------------------------------------------------

        public void Add(ActiveSite site)
        {
            siteLocations.Add(site.Location);
            activeArea = siteLocations.Count * Model.Core.CellArea;
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
