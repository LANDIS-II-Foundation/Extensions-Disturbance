namespace Landis.Extension.BaseHarvest
{
    /// <summary>
    /// A stand that's been set aside for a future harvest.
    /// </summary>
    public struct ReservedStand
    {
        public Stand Stand;
        public int NextTimeToHarvest;

        //---------------------------------------------------------------------

        public ReservedStand(Stand stand,
                             int   nextTimeToHarvest)
        {
            this.Stand = stand;
            this.NextTimeToHarvest = nextTimeToHarvest;
        }
    }
}
