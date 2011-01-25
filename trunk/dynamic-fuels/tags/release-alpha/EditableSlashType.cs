//  Copyright 2006 University of Wisconsin-Madison
//  Authors:  Robert Scheller, Jimm Domingo
//  License:  Available at  
//  http://landis.forest.wisc.edu/developers/LANDIS-IISourceCodeLicenseAgreement.pdf

using Edu.Wisc.Forest.Flel.Util;
using System.Collections.Generic;

namespace Landis.Fuels
{
    /// <summary>
    /// Editable forest type.
    /// </summary>
    public interface IEditableSlashType
        : IEditable<ISlashType>
    {
        //InputValue<FuelTypeCode> Name {get; set;}
        InputValue<int> FuelIndex {get; set;}
        InputValue<int> MaxAge {get; set;}
        List<string> PrescriptionNames {get; set;}
    }

    public class EditableSlashType
        : IEditableSlashType
    {
        //private InputValue<FuelTypeCode> name;
        private InputValue<int> fuelIndex;
        private InputValue<int> maxAge;
        private List<string> prescriptionNames;

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
        public InputValue<int> FuelIndex
        {
            get {
                return fuelIndex;
            }

            set {
                if (value != null)
                    if (value.Actual <= 1 || value.Actual > 100)
                        throw new InputValueException(value.String,
                                                      "Value must be > 1 and <= 100.");
                fuelIndex = value;
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
                    if (value.Actual <= 0)
                        throw new InputValueException(value.String,
                                                      "Value must be > 0.");
                maxAge = value;
            }
        }
        //---------------------------------------------------------------------

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
        public EditableSlashType()
        {
            prescriptionNames = new List<string>();
        }

        //---------------------------------------------------------------------

        public bool IsComplete
        {
            get {
                foreach (object parameter in new object[]{ 
                    //name,
                    fuelIndex,
                    maxAge,
                    prescriptionNames
                }) {
                    if (parameter == null)
                        return false;
                }
                return true;
            }
        }

        //---------------------------------------------------------------------

        public ISlashType GetComplete()
        {
            if (IsComplete)
                return new SlashType(//name.Actual, 
                            fuelIndex.Actual,
                                    maxAge.Actual,
                                    prescriptionNames);
            else
                return null;
        }
    }


}
