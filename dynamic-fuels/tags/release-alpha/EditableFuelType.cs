//  Copyright 2005 University of Wisconsin-Madison
//  Authors:  Robert Scheller, Jimm Domingo
//  License:  Available at  
//  http://landis.forest.wisc.edu/developers/LANDIS-IISourceCodeLicenseAgreement.pdf

using Edu.Wisc.Forest.Flel.Util;

namespace Landis.Fuels
{
    /// <summary>
    /// Editable forest type.
    /// </summary>
    public class EditableFuelType
        : IEditableFuelType
    {
        //private InputValue<FuelTypeCode> name;
        private InputValue<int> fuelIndex;
        private InputValue<BaseFuelType> baseFuel;
        private InputValue<int> minAge;
        private InputValue<int> maxAge;
        private int[] multipliers;

        //---------------------------------------------------------------------

        /// <summary>
        /// Map name
        /// </summary>
        /*public InputValue<FuelTypeCode> Name
        {
            get {
                return name;
            }

            set {
                name = value;
            }
        }*/
        /// <summary>
        /// Map name
        /// </summary>
        public InputValue<int> FuelIndex
        {
            get {
                return fuelIndex;
            }

            set {
                if (value != null)
                    if (value.Actual < 1 || value.Actual > 100)
                        throw new InputValueException(value.String,
                                                      "Value must be between 1 and 100.");
                fuelIndex = value;
            }
        }
        //---------------------------------------------------------------------
        public InputValue<BaseFuelType> BaseFuel
        {
            get {
                return baseFuel;
            }

            set {
                baseFuel = value;
            }
        }
        //---------------------------------------------------------------------

        public InputValue<int> MinAge
        {
            get {
                return minAge;
            }

            set {
                if (value != null)
                    if (value.Actual < 0)
                        throw new InputValueException(value.String,
                                                      "Value must be = or > 0.");
                minAge = value;
            }
        }
        //---------------------------------------------------------------------

        public InputValue<int> MaxAge
        {
            get {
                return maxAge;
            }

            set {
                if (value != null)
                    if (value.Actual < 0)
                        throw new InputValueException(value.String,
                                                      "Value must be = or > 0.");
                maxAge = value;
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

            set {
                multipliers[speciesIndex] = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Initialize a new instance.
        /// </summary>
        public EditableFuelType(int speciesCount)
        {
            multipliers = new int[speciesCount];
        }

        //---------------------------------------------------------------------

        public bool IsComplete
        {
            get {
                foreach (object parameter in new object[]{ 
                    //name,
                    fuelIndex,
                    baseFuel,
                    minAge,
                    maxAge
                }) {
                    if (parameter == null)
                        return false;
                }
                return true;
            }
        }

        //---------------------------------------------------------------------

        public IFuelType GetComplete()
        {
            if (IsComplete)
                return new FuelType(//name.Actual, 
                            fuelIndex.Actual,
                            baseFuel.Actual,
                                    minAge.Actual,
                                    maxAge.Actual,
                                    multipliers);
            else
                return null;
        }
    }
}
