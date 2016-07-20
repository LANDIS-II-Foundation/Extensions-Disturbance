//  Copyright 2006-2010 Portland State University, USFS Northern Research Station, University of Wisconsin-Madison
//  Authors:  Robert Scheller, Brian Miranda, Jimm Domingo

namespace Landis.Extension.BiomassFuels
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
