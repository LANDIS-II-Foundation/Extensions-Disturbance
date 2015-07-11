//  Copyright 2006 University of Wisconsin-Madison
//  Authors:  Robert Scheller, Jimm Domingo
//  License:  Available at  
//  http://landis.forest.wisc.edu/developers/LANDIS-IISourceCodeLicenseAgreement.pdf

namespace Landis.Fuels
{

    //NOTE:  M2, M4, and O1b excluded for this list.  This is because these types are
    //dependent upon season (leaf on or off), which is derived in the new fire extension.
    //This enumerated list must EXACTLY match the FuelTypeCode list found in the fire 2006 extension.
    //public enum FuelTypeCode {C1, C2, C3, C4, C5, C6, C7, D1, S1, S2, S3, M1, M2, M3, M4, O1a, O1b, NoFuel};
    public enum BaseFuelType {Conifer, ConiferPlantation, Deciduous, NoFuel, Open, Slash};

    /// <summary>
    /// A forest type.
    /// </summary>
    public class FuelType
        : IFuelType
    {
        //private FuelTypeCode name;
        private int fuelIndex;
        private BaseFuelType baseFuel;
        private int minAge;
        private int maxAge;
        private int[] multipliers;

        //---------------------------------------------------------------------

        /// <summary>
        /// Name
        /// </summary>
        //public FuelTypeCode Name
        //{
        //    get {
        //        return name;
        //    }
        //}
        public int FuelIndex
        {
            get {
                return fuelIndex;
            }
        }
        //---------------------------------------------------------------------

        public BaseFuelType BaseFuel
        {
            get {
                return baseFuel;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Minimum cohort age.
        /// </summary>
        public int MinAge
        {
            get {
                return minAge;
            }
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// Maximum cohort age.
        /// </summary>
        public int MaxAge
        {
            get {
                return maxAge;
            }
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// Multiplier for a species
        /// </summary>
        public int this[int speciesIndex]
        {
            get {
                return multipliers[speciesIndex];
            }
        }

        //---------------------------------------------------------------------

        public FuelType(//FuelTypeCode name,
                            int fuelIndex,
                            BaseFuelType baseFuel,
                            int minAge,
                            int maxAge,
                          int[]  multipliers)
        {
            //this.name = name;
            this.fuelIndex = fuelIndex;
            this.baseFuel = baseFuel;
            this.minAge = minAge;
            this.maxAge = maxAge;
            this.multipliers = multipliers;
        }
    }
}
