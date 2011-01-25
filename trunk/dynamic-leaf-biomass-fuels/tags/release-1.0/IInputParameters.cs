//  Copyright 2007-2008 Conservation Biology Institute, USFS Northern Research Station
//  Authors:
//      Robert M. Scheller
//      Brian R. Miranda
//  License:  Available at
//  http://www.landis-ii.org/developers/LANDIS-IISourceCodeLicenseAgreement.pdf

using System.Collections.Generic;


namespace Landis.Fuels
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
            get;set;
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Hardwood Maximum (percent)
        /// </summary>
        int HardwoodMax
        {
            get;set;
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Dead fir maximum age (years)
        /// </summary>

        int DeadFirMaxAge {get;set;}
        //---------------------------------------------------------------------

        /// <summary>
        /// Fuel coefficients for species
        /// </summary>
        double[] FuelCoefficients
        {
            get;set;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The fuel types that are cells are being classified into.
        /// </summary>
        List<IFuelType> FuelTypes
        {
            get;set;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Disturbance types - can be used to force a conversion of fuel type
        /// </summary>
        List<IDisturbanceType> DisturbanceTypes
        {
            get;set;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Template for the filenames for Fuel maps.
        /// </summary>
        string MapFileNames
        {
            get;set;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Template for the filenames for percent conifer maps.
        /// </summary>
        string PctConiferFileName
        {
            get;set;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Template for the filenames for percent dead fir maps.
        /// </summary>
        string PctDeadFirFileName
        {
            get;set;
        }

    }
}
