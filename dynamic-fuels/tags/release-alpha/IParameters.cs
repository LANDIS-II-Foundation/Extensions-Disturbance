//  Copyright 2006 University of Wisconsin-Madison
//  Authors:  Robert Scheller, Jimm Domingo
//  License:  Available at  
//  http://landis.forest.wisc.edu/developers/LANDIS-IISourceCodeLicenseAgreement.pdf

using System.Collections.Generic;


namespace Landis.Fuels
{
    /// <summary>
    /// The parameters for the plug-in.
    /// </summary>
    public interface IParameters
    {
        /// <summary>
        /// Timestep (years)
        /// </summary>
        int Timestep
        {
            get;
        }

        /// <summary>
        /// Hardwood Maximum (percent)
        /// </summary>
        int HardwoodMax
        {
            get;
        }
        
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
        /// Conifer Index for species
        /// </summary>
        /*
        bool[] ConiferIndex
        {
            get;
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// Deciduous Index for species
        /// </summary>
        bool[] DecidIndex
        {
            get;
        }*/
        //---------------------------------------------------------------------

        /// <summary>
        /// Fuel maps
        /// </summary>
        IFuelType[] FuelTypes
        {
            get;
        }

        ISlashType[] SlashTypes
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

        //-----Added by BRM-----
        /// <summary>
        /// Template for the filenames for percent conifer maps.
        /// </summary>
        string PctConiferFileName
        {
            get;
        }

        //---------------------------------------------------------------------

        //-----Added by BRM-----
        /// <summary>
        /// Template for the filenames for percent dead fir maps.
        /// </summary>
        string PctDeadFirFileName
        {
            get;
        }
    
    }
}
