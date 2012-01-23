namespace Landis.Extension.BaseHarvest
{
    /// <summary>
    /// The parameters for computing the economic rank for a species.
    /// </summary>
    public struct FireRiskParameters
    {
        private byte fireRisk;
        //private ushort minAge;

        //---------------------------------------------------------------------

        /// <summary>
        /// The species' economic rank.
        /// </summary>
        public byte Rank
        {
            get {
                return fireRisk;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The minimum age at which the species has economic value.
        /// </summary>
        //public ushort MinimumAge
        //{
        //    get {
        //        return minAge;
        //    }
        //}

        //---------------------------------------------------------------------

        public FireRiskParameters(byte   rank)
        {
            this.fireRisk = rank;
        }
    }
}
