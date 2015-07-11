//  Copyright 2006 University of Wisconsin-Madison
//  Authors:  Robert Scheller
//  License:  Available at  
//  http://landis.forest.wisc.edu/developers/LANDIS-IISourceCodeLicenseAgreement.pdf

using Edu.Wisc.Forest.Flel.Util;
using System.Collections.Generic;

namespace Landis.Fuels
{
    /// <summary>
    /// The parameters for the plug-in.
    /// </summary>
    public class Parameters
        : IParameters
    {
        private int timestep;
        private int hardwoodMax;
        private int deadFirMaxAge;
        private double[] coefficients;
        //private bool[] coniferIndex;
        //private bool[] decidIndex;
        private IFuelType[] fuelTypes;
        private ISlashType[] slashTypes;
        private string mapFileNames;
        private string pctConiferFileName;
        private string pctDeadFirFileName;

        //---------------------------------------------------------------------

        /// <summary>
        /// Timestep (years)
        /// </summary>
        public int Timestep
        {
            get {
                return timestep;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Hardwood maximum (percent)
        /// </summary>
        public int HardwoodMax
        {
            get {
                return hardwoodMax;
            }
        }
        
        public int DeadFirMaxAge
        {
            get {
                return deadFirMaxAge;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Fuel coefficients for species
        /// </summary>
        public double[] FuelCoefficients
        {
            get {
                return coefficients;
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Conifer index for a species (TRUE OR FALSE)
        /// </summary>
        /*public bool[] ConiferIndex
        {
            get {
                return coniferIndex;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Deciduous index for a species (TRUE OR FALSE)
        /// </summary>
        public bool[] DecidIndex
        {
            get {
                return decidIndex;
            }
        }*/
        //---------------------------------------------------------------------

        /// <summary>
        /// Fuel types
        /// </summary>
        public IFuelType[] FuelTypes
        {
            get {
                return fuelTypes;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Slash specific fuel types
        /// </summary>
        public ISlashType[] SlashTypes
        {
            get {
                return slashTypes;
            }
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// Template for the filenames for Fuel maps.
        /// </summary>
        public string MapFileNames
        {
            get {
                return mapFileNames;
            }
        }

        //---------------------------------------------------------------------


        //-----Added by BRM-----
        /// <summary>
        /// Template for the filenames for percent conifer maps.
        /// </summary>
        public string PctConiferFileName
        {
            get
            {
                return pctConiferFileName;
            }
        }
        //----------
        //---------------------------------------------------------------------


        //-----Added by BRM-----
        /// <summary>
        /// Template for the filenames for percent dead fir maps.
        /// </summary>
        public string PctDeadFirFileName
        {
            get
            {
                return pctDeadFirFileName;
            }
        }
        //----------
        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="timestep"></param>
        /// <param name="coefficients"></param>
        /// <param name="mapDefns"></param>
        /// <param name="mapFileNames"></param>
        public Parameters(int              timestep,
                          double[]         coefficients,
                          //bool[]            coniferIndex,
                          //bool[]            decidIndex,
                          int               hardwoodMax,
                          int               deadFirMaxAge,
                          IFuelType[]       fuelTypes,
                          ISlashType[]      slashTypes,
                          string           mapFileNames,
                          string            pctConiferFileName,
                          string            pctDeadFirFileName)
        {
            this.timestep = timestep;
            this.coefficients = coefficients;
            //this.coniferIndex = coniferIndex;
            //this.decidIndex = decidIndex;
            this.hardwoodMax = hardwoodMax;
            this.deadFirMaxAge = deadFirMaxAge;
            this.fuelTypes = fuelTypes;
            this.slashTypes = slashTypes;
            this.mapFileNames = mapFileNames;
            this.pctConiferFileName = pctConiferFileName;
            this.pctDeadFirFileName = pctDeadFirFileName;
        }
    }
}
