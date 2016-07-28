//  Copyright 2007-2008 Conservation Biology Institute, USFS Northern Research Station
//  Authors:
//      Robert M. Scheller
//      Brian R. Miranda
//  License:  Available at
//  http://www.landis-ii.org/developers/LANDIS-IISourceCodeLicenseAgreement.pdf

using System.Collections.Generic;
using Edu.Wisc.Forest.Flel.Util;


namespace Landis.Fuels
{
    //Disturbance fuel types
    public interface IDisturbanceType
    {
        int FuelIndex {get;set;}
        int MaxAge {get;set;}
        List<string> PrescriptionNames{get;set;}
    }

    /// <summary>
    /// A forest type.
    /// </summary>
    public class DisturbanceType
        : IDisturbanceType
    {
        private int fuelIndex;
        private int maxAge;
        private List<string> prescriptionNames;

        //---------------------------------------------------------------------

        /// <summary>
        /// Index
        /// </summary>
        public int FuelIndex
        {
            get {
                return fuelIndex;
            }
            set {
                if (value <= 1 || value > 100)
                        throw new InputValueException(value.ToString(), "Value must be > 1 and <= 100.");
                fuelIndex = value;
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
                if (value <= 0)
                        throw new InputValueException(value.ToString(), "Value must be > 0.");
                maxAge = value;
            }
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// A prescription name
        /// </summary>
        public List<string> PrescriptionNames
        {
            get {
                return prescriptionNames;
            }
            set {
                if (value != null)
                    prescriptionNames = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Initialize a new instance.
        /// </summary>
        public DisturbanceType()
        {
            prescriptionNames = new List<string>();
        }
    }
}
