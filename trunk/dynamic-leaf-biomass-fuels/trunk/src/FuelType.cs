//  Copyright 2007-2010 Portland State University, USFS Northern Research Station
//  Authors:  Robert M. Scheller, Brian R. Miranda

using Edu.Wisc.Forest.Flel.Util;

namespace Landis.Extension.Fuels.LeafBiomass
{

    //NOTE:  M2, M4, and O1b excluded for this list.  This is because these types are
    //dependent upon season (leaf on or off), which is derived in the new fire extension.
    public enum BaseFuelType {Conifer, ConiferPlantation, Deciduous, NoFuel, Open, Slash};

    /// <summary>
    /// A forest type.
    /// </summary>
    public class FuelType
        : IFuelType
    {
        private int index;
        private BaseFuelType baseFuel;
        private int minAge;
        private int maxAge;
        private int[] sppMultipliers;
        private bool[] ecoregions;

        //---------------------------------------------------------------------

        /// <summary>
        /// Fuel type index
        /// </summary>
        public int Index
        {
            get {
                return index;
            }
            set {
                if (value < 1 || value > 100)
                        throw new InputValueException(value.ToString(), "Value must be between 1 and 100.");
                index = value;
            }
        }
        //---------------------------------------------------------------------

        public BaseFuelType BaseFuel
        {
            get {
                return baseFuel;
            }
            set {
                baseFuel = value;
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
            set {
                if (value < 0)
                        throw new InputValueException(value.ToString(), "Value must be = or > 0.");
                minAge = value;
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
            set {
                if (value < 0)
                        throw new InputValueException(value.ToString(), "Value must be = or > 0.");
                maxAge = value;
            }
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// Optional list of associated ecoregions
        /// </summary>
        public bool[] Ecoregions
        {
            get {
                return ecoregions;
            }
            set {
                ecoregions = value;
            }
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// Multiplier for a species
        /// </summary>
        public int this[int speciesIndex]
        {
            get {
                return sppMultipliers[speciesIndex];
            }
            set {
                sppMultipliers[speciesIndex] = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Initialize a new instance.
        /// </summary>
        public FuelType(int speciesCount, int ecoCount)
        {
            ecoregions  = new bool[ecoCount];
            for (int i=0; i < ecoCount; i++)
                ecoregions[i] = true;

            sppMultipliers = new int[speciesCount];
        }
    }
}
