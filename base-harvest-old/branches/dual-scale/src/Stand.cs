using Edu.Wisc.Forest.Flel.Util;
using System.Collections;
using System.Collections.Generic;
using Wisc.Flel.GeospatialModeling.Landscapes.DualScale;

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
        private double blockArea;
        private ManagementArea mgmtArea;
        private bool harvested;
        private List<Stand> neighbors;
        private List<Stand> ma_neighbors;   //list for neighbors in different management areas
        private ushort age;
        private int yearAgeComputed;
        private int setAsideUntil;
        //the time this was last harvested
        private int timeLastHarvested;
        //prescription name for use in log
        private string prescriptionName;
        //event id, for use in log
        private int event_id;
        //rank, used in log
        private double rank;
        //harvested_rank, used in log
        private double harvested_rank;
        //dictionary for keeping number of damaged sites.  key = species, value = number of occurrences
        private Dictionary<string, int> damage_table;

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

        /// <summar>
        /// The list of locations in this stand at which there exist a site.
        /// </summary>
        public List<Location> SiteLocations {
            get {
                return siteLocations;
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

        public List<ActiveSite> GetSites() {
            List<ActiveSite> sites = new List<ActiveSite>();
            foreach (ActiveSite site in this) {
                sites.Add(site);
            }
            return sites;
        }

        //---------------------------------------------------------------------

        public double ActiveArea
        {
            get {
                return activeArea;
            }
        }

        //---------------------------------------------------------------------

        public double BlockArea
        {
            get {
                return blockArea;
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

                    //also when age is computed, immediately give it to timeLastHarvested
                    //if it equals -1 because that is the initialization value.
                    if (this.timeLastHarvested == -1) {
                        //given negative so it's initial harvested value is always positive
                        this.timeLastHarvested = -(age);
                    }
                    yearAgeComputed = Model.Core.CurrentTime;
                }
                return age;
            }
        }


        //---------------------------------------------------------------------

        /// <summary>
        /// Set's or returns this stand's rank.
        /// </summary>
        public double Rank {
            get {
                return this.rank;
            }

            set {
                this.rank = value;
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

        /// <summary>
        /// Sets or returns the rank at which this stand was last harvested
        /// </summary>
        public double HarvestedRank {
            get {
                return this.harvested_rank;
            }

            set {
                this.harvested_rank = value;
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Return the time this stand was last harvested.
        /// </summary>
        public int TimeLastHarvested {
            get {
                return timeLastHarvested;
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Return the time SINCE this stand was last harvested.
        /// </summary>
        public int TimeSinceLastHarvested {
            get {
                return Model.Core.CurrentTime - timeLastHarvested;
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

        public IEnumerable<Stand> MaNeighbors
        {
            get {
                return ma_neighbors;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Has the stand been set aside by a repeat harvest? (or a stand adjacency constraint)
        /// </summary>
        public bool IsSetAside
        {
            get {

                bool ret = (Model.Core.CurrentTime <= setAsideUntil);
                return ret;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Returns the name of the prescription which damaged this stand
        /// </summary>
        public string PrescriptionName {
            get {
                return prescriptionName;
            }

            set {
                prescriptionName = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Returns the event-id of this harvest
        /// </summary>
        public int EventId {
            get {
                return event_id;
            }

            set {
                event_id = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// A new Stand Object, given a map code
        /// </summary>
        public Stand(ushort mapCode, double blockArea)
        {
            this.mapCode = mapCode;
            this.blockArea = blockArea;
            this.siteLocations = new List<Location>();
            this.activeArea = 0; //Model.Core.CellArea;
            this.mgmtArea = null;
            this.neighbors = new List<Stand>();
            this.ma_neighbors = new List<Stand>();      //new list for neighbors not in this management area
            this.yearAgeComputed = Model.Core.StartTime;
            this.setAsideUntil = Model.Core.StartTime;
            //initialize to -1, will be reset (to -age) on 1st pass through
            //when ages are computed
            this.timeLastHarvested = -1;
            //initialize damage_table dictionary
            damage_table = new Dictionary<string, int>();
        }

        //---------------------------------------------------------------------

        private static RelativeLocation neighborAbove = new RelativeLocation(-1, 0);
        private static RelativeLocation neighborLeft  = new RelativeLocation( 0, -1);
        private static RelativeLocation[] neighborsAboveAndLeft = new RelativeLocation[]{ neighborAbove, neighborLeft };

        //---------------------------------------------------------------------
         public void Add(ActiveSite site) {
         
            siteArea = (site.SharesData ? Model.BlockArea : Model.Core.CellArea);
            if (siteArea != this.blockArea)
                throw new ApplicationException("Attempted to add site with block size conflicting with stand block size.");

            siteLocations.Add(site.Location);
            this.activeArea += Model.Core.CellArea;
            //set site var
            SiteVars.Stand[site] = this;

            //loop- really just 2 locations, relative (-1, 0) and relative (0, -1)
            foreach (RelativeLocation neighbor_loc in neighborsAboveAndLeft) {
                //check this site for neighbors that are different
                Site neighbor = site.GetNeighbor(neighbor_loc);
                if (neighbor != null && neighbor.IsActive) {
                    //declare a stand with this site as its index.
                    Stand neighbor_stand = SiteVars.Stand[neighbor];
                    //check for non-null stand
                    //also, only allow stands in same management area to be called neighbors
                    if (neighbor_stand != null && this.ManagementArea == neighbor_stand.ManagementArea) {
                        //if neighbor_stand is different than this stand, then it is a true
                        //neighbor.  add it as a neighbor and add 'this' as a neighbor of that.
                        if (this != neighbor_stand) {
                            //add neighbor_stand as a neighboring stand to this
                            AddNeighbor(neighbor_stand);
                            //add this as a neighboring stand to neighbor_stand
                            neighbor_stand.AddNeighbor(this);
                        }
                    }
                    //take into account other management areas just for stand-adjacency issue
                    else if (neighbor_stand != null && this.ManagementArea != neighbor_stand.ManagementArea) {
                        if (this != neighbor_stand) {
                            //add neighbor_stand as a ma-neighboring stand to this
                            AddMaNeighbor(neighbor_stand);
                            //add this as a ma-neighbor stand to neighbor_stand
                            neighbor_stand.AddMaNeighbor(this);
                        }
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

        protected void AddMaNeighbor(Stand neighbor) {
            Require.ArgumentNotNull(neighbor);
            if (! ma_neighbors.Contains(neighbor)) {
                ma_neighbors.Add(neighbor);
            }
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
        public void MarkAsHarvested()
        {
            //reset timeLastHarvested to current time
            this.timeLastHarvested = Model.Core.CurrentTime;
            //mark stand as harvested
            harvested = true;
        }


        //---------------------------------------------------------------------

        /// <summary>
        /// Sets the stand aside until a later time for a repeat harvest.
        /// </summary>
        /// <param name="year">
        /// The calendar year until which the stand should be stand aside.
        /// </param>
        public void SetAsideUntil(int year) {
            setAsideUntil = year;
        }

        /// <summary>
        /// Update the damage_table for this stand
        /// </summary>
        public void UpdateDamageTable(string species) {
            try {
                //add this species to the dictionary, with initial value = 1
                damage_table.Add(species, 1);
            }
            //if an ArguementException is caught, increment this key's value
            catch (System.ArgumentException) {
                damage_table[species]++;
            }
        }

        /// <summary>
        ///Clear the damage table of all data.
        /// </summary>
        public void ClearDamageTable() {
            damage_table.Clear();
        }

        public Dictionary<string, int> DamageTable {
            get {
                return damage_table;
            }
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
