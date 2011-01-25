//  Copyright 2007-2010 Portland State University, USFS Northern Research Station
//  Authors:  Robert M. Scheller, Brian R. Miranda

namespace Landis.Extension.Fuels.LeafBiomass
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
