//  Copyright 2006-2010 Portland State University, USFS Northern Research Station, University of Wisconsin-Madison
//  Authors:  Robert Scheller, Brian Miranda, Jimm Domingo

using Edu.Wisc.Forest.Flel.Util;

using System.Collections.Generic;

namespace Landis.Extension.BiomassFuels
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
                //if (value != null) {
                    if (value < 0)
                        throw new InputValueException(value.ToString(),
                                                      "Value must be = or > 0.");
                //}
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
                    throw new InputValueException(value.ToString(),"Value must be >= 0 and <= 100.");
                hardwoodMax = value;
            }
        }

        //---------------------------------------------------------------------
        public int DeadFirMaxAge
        {
            get {
                return deadFirMaxAge;
            }
            set {
                if (value < 0 || value > 100)
                    throw new InputValueException(value.ToString(),"Value must be >= 0 and <= 100.");
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
            set {
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
            set {
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
            get {
                return pctConiferFileName;
            }
            set {
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
            get {
                return pctDeadFirFileName;
            }
            set {
                MapNames.CheckTemplateVars(value);
                pctDeadFirFileName = value;
            }
        }

        //---------------------------------------------------------------------

        public InputParameters(int speciesCount)
        {
            coefficients = new double[speciesCount];
            for(int i=0; i < speciesCount; i++)
                coefficients[i] = 1.0;
            fuelTypes = new List<IFuelType>();
            disturbanceTypes = new List<IDisturbanceType>();

        }


        //---------------------------------------------------------------------
/*
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="timestep"></param>
        /// <param name="coefficients"></param>
        /// <param name="mapDefns"></param>
        /// <param name="mapFileNames"></param>
        public Parameters(int              timestep,
                          double[]         coefficients,
                          int               hardwoodMax,
                          int               deadFirMaxAge,
                          IFuelType[]       fuelTypes,
                          IDisturbanceType[]      disturbanceTypes,
                          string           mapFileNames,
                          string            pctConiferFileName,
                          string            pctDeadFirFileName)
        {
            this.timestep = timestep;
            this.coefficients = coefficients;
            this.hardwoodMax = hardwoodMax;
            this.deadFirMaxAge = deadFirMaxAge;
            this.fuelTypes = fuelTypes;
            this.disturbanceTypes = disturbanceTypes;
            this.mapFileNames = mapFileNames;
            this.pctConiferFileName = pctConiferFileName;
            this.pctDeadFirFileName = pctDeadFirFileName;
        }*/
    }
}
