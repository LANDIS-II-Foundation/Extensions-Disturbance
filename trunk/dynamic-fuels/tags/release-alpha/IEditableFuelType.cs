//  Copyright 2006 University of Wisconsin-Madison
//  Authors:  Robert Scheller, Jimm Domingo
//  License:  Available at  
//  http://landis.forest.wisc.edu/developers/LANDIS-IISourceCodeLicenseAgreement.pdf

using Edu.Wisc.Forest.Flel.Util;

namespace Landis.Fuels
{
    /// <summary>
    /// Editable forest type.
    /// </summary>
    public interface IEditableFuelType
        : IEditable<IFuelType>
    {
        /// <summary>
        /// Map name
        /// </summary>
        //InputValue<FuelTypeCode> Name
        //{
        //    get;
        //    set;
        //}
        
        InputValue<int> FuelIndex {get; set;}
        InputValue<BaseFuelType> BaseFuel {get; set;}
        InputValue<int> MinAge {get; set;}
        InputValue<int> MaxAge {get; set;}

        //---------------------------------------------------------------------

        /// <summary>
        /// Multiplier for a species
        /// </summary>
        int this[int speciesIndex]
        {
            get;
            set;
        }
    }
}
