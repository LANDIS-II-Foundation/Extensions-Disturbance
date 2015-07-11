//  Copyright 2006 University of Wisconsin-Madison
//  Authors:  Robert Scheller, Jimm Domingo
//  License:  Available at  
//  http://landis.forest.wisc.edu/developers/LANDIS-IISourceCodeLicenseAgreement.pdf

namespace Landis.Fuels
{
    /// <summary>
    /// A forest type.
    /// </summary>
    public interface IFuelType
    {
        /// <summary>
        /// Name
        /// </summary>
        //FuelTypeCode Name
        //{
        //    get;
        //}
        
        int FuelIndex {get;}
        BaseFuelType BaseFuel {get;}
        int MinAge {get;}
        int MaxAge {get;}

        //---------------------------------------------------------------------

        /// <summary>
        /// Multiplier for a species
        /// </summary>
        int this[int speciesIndex]
        {
            get;
        }
    }
}
