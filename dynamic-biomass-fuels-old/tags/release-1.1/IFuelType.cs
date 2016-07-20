//  Copyright 2007-2008 Conservation Biology Institute, USFS Northern Research Station
//  Authors:
//      Robert M. Scheller
//      Brian R. Miranda
//  License:  Available at
//  http://www.landis-ii.org/developers/LANDIS-IISourceCodeLicenseAgreement.pdf

namespace Landis.Extension.Fuels
{
    /// <summary>
    /// A forest type.
    /// </summary>
    public interface IFuelType
    {
        int Index {get;set;}
        BaseFuelType BaseFuel {get;set;}
        int MinAge {get;set;}
        int MaxAge {get;set;}
        bool[] Ecoregions {get;set;}

        //---------------------------------------------------------------------

        /// <summary>
        /// Multiplier for a species
        /// </summary>
        int this[int speciesIndex]
        {
            get;set;
        }
    }
}
