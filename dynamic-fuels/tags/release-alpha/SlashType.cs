//  Copyright 2006 University of Wisconsin-Madison
//  Authors:  Robert Scheller, Jimm Domingo
//  License:  Available at  
//  http://landis.forest.wisc.edu/developers/LANDIS-IISourceCodeLicenseAgreement.pdf

using System.Collections.Generic;

namespace Landis.Fuels
{
    //This slash type is used for all disturbance fuel types
    public interface ISlashType
    {
        /// <summary>
        /// Name
        /// </summary>
        //FuelTypeCode Name {get;}
        int FuelIndex {get;}
        int MaxAge {get;}
        List<string> PrescriptionNames{get;}
    }

    /// <summary>
    /// A forest type.
    /// </summary>
    public class SlashType
        : ISlashType
    {
        //private FuelTypeCode name;
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
        /// A prescription name
        /// </summary>
        public List<string> PrescriptionNames
        {
            get {
                return prescriptionNames;
            }
        }

        //---------------------------------------------------------------------

        public SlashType(int fuelIndex,
                            int maxAge,
                            List<string>  prescriptionNames)
        {
            this.fuelIndex = fuelIndex;
            this.maxAge = maxAge;
            this.prescriptionNames = prescriptionNames;
        }
    }
}
