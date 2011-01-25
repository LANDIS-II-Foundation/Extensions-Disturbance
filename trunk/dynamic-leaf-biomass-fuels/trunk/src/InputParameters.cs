//  Copyright 2007-2010 Portland State University, USFS Northern Research Station
//  Authors:  Robert M. Scheller, Brian R. Miranda

using Edu.Wisc.Forest.Flel.Util;
using System.Collections.Generic;

namespace Landis.Extension.Fuels.LeafBiomass
{
    /// <summary>
    /// The parameters for the plug-in.
    /// </summary>
    public class InputParameters
        : IInputParameters
    {
        private int timestep;
        private int hardwoodMax;
        private int deadFirMaxAge;
        private double[] coefficients;
        private List<IFuelType> fuelTypes;
        private List<IDisturbanceType> disturbanceTypes;
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
            set {
                if (value < 0)
                        throw new InputValueException(value.ToString(), "Value must be = or > 0.");
                timestep = value;
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
            set {
                if (value < 0 || value > 100)
                        throw new InputValueException(value.ToString(), "Value must be >= 0 and <= 100.");
                hardwoodMax = value;
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Maximum age that the dead fir cohort count will contribute to the percent conifer calculation
        /// </summary>

        public int DeadFirMaxAge
        {
            get {
                return deadFirMaxAge;
            }
            set {
                if (value < 0 || value > 100)
                        throw new InputValueException(value.ToString(), "Value must be >= 0 and <= 100.");
                deadFirMaxAge = value;
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
            set {
                coefficients = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The Fuel types that cells are being classified into
        /// </summary>
        public List<IFuelType> FuelTypes
        {
            get {
                return fuelTypes;
            }
            set
            {
                fuelTypes = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Disturbance types that can be used to force the conversion of fuel types.
        /// </summary>
        public List<IDisturbanceType> DisturbanceTypes
        {
            get {
                return disturbanceTypes;
            }
            set
            {
                disturbanceTypes = value;
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
            set {
                MapNames.CheckTemplateVars(value);
                mapFileNames = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Template for the filenames for percent conifer maps.
        /// </summary>
        public string PctConiferFileName
        {
            get
            {
                return pctConiferFileName;
            }
            set
            {
                MapNames.CheckTemplateVars(value);
                pctConiferFileName = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Template for the filenames for percent dead fir maps.
        /// </summary>
        public string PctDeadFirFileName
        {
            get
            {
                return pctDeadFirFileName;
            }
            set
            {
                MapNames.CheckTemplateVars(value);
                pctDeadFirFileName = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance.
        /// </summary>

        public InputParameters(int speciesCount)
        {
            coefficients = new double[speciesCount]; // Coefficients(speciesCount);
            fuelTypes = new List<IFuelType>();
            disturbanceTypes = new List<IDisturbanceType>();

        }
        //---------------------------------------------------------------------

    }
}
