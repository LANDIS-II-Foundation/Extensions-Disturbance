//  Copyright 2006-2010 Portland State University, USFS Northern Research Station, University of Wisconsin-Madison
//  Authors:  Robert Scheller, Brian Miranda, Jimm Domingo

using System.Collections.Generic;


namespace Landis.Extension.BiomassFuels
{
    /// <summary>
    /// The parameters for the plug-in.
    /// </summary>
    public interface IInputParameters
    {
        /// <summary>
        /// Timestep (years)
        /// </summary>
        int Timestep
        {
            get;
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Hardwood Maximum (percent)
        /// </summary>
        int HardwoodMax
        {
            get;
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Dead fir maximum age (years)
        /// </summary>

        int DeadFirMaxAge {get;}
        //---------------------------------------------------------------------

        /// <summary>
        /// Fuel coefficients for species
        /// </summary>
        double[] FuelCoefficients
        {
            get;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The fuel types that are cells are being classified into.
        /// </summary>
        List<IFuelType> FuelTypes
        {
            get;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Disturbance types - can be used to force a conversion of fuel type
        /// </summary>
        List<IDisturbanceType> DisturbanceTypes
        {
            get;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Template for the filenames for Fuel maps.
        /// </summary>
        string MapFileNames
        {
            get;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Template for the filenames for percent conifer maps.
        /// </summary>
        string PctConiferFileName
        {
            get;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Template for the filenames for percent dead fir maps.
        /// </summary>
        string PctDeadFirFileName
        {
            get;
        }

    }
}
