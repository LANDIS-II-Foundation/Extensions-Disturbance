
using System.Collections.Generic;
using System.Diagnostics;
using Landis.Core;
using Edu.Wisc.Forest.Flel.Util;


namespace Landis.Extension.StressMortality
{
    /// <summary>
    /// Parameters for the plug-in.
    /// </summary>
    public interface IInputParameters
    {
        ///int Timestep { get; set; }
        string MapNamesTemplate { get; set; }
        string LogFileName { get; set; }
        SpeciesAuxParm<List<AgeClass>> PartialMortalityTable { get; }
        SpeciesAuxParm<int> CompleteMortalityThreshold { get; }
        SpeciesAuxParm<int> CompleteMortalityTime { get; }
        
    }

    class InputParameters
        : IInputParameters
    {
        //private int timestep;
        private string mapNamesTemplate;
        private string logFileName;
        private SpeciesAuxParm<List<AgeClass>> partialMortalityTable;
        private SpeciesAuxParm<int> completeMortalityThreshold;
        private SpeciesAuxParm<int> completeMortalityTime;
        //---------------------------------------------------------------------
        /// <summary>
        /// Template for the filenames for output maps.
        /// </summary>
        public string MapNamesTemplate
        {
            get
            {
                return mapNamesTemplate;
            }
            set
            {
                MapNames.CheckTemplateVars(value);
                mapNamesTemplate = value;
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Name of log file.
        /// </summary>
        public string LogFileName
        {
            get
            {
                return logFileName;
            }
            set
            {
                // FIXME: check for null or empty path (value);
                logFileName = value;
            }
        }

        //---------------------------------------------------------------------
        public SpeciesAuxParm<List<AgeClass>> PartialMortalityTable
        {
            get
            {
                return partialMortalityTable;
            }
        }

        //---------------------------------------------------------------------
        public SpeciesAuxParm<int> CompleteMortalityThreshold
        {
            get
            {
                return completeMortalityThreshold;
            }
        }

        //---------------------------------------------------------------------
        public SpeciesAuxParm<int> CompleteMortalityTime
        {
            get
            {
                return completeMortalityTime;
            }
        }
        //---------------------------------------------------------------------
        public void SetPartialMortalityTable(ISpecies species, List<AgeClass> newValue)
        {
            Debug.Assert(species != null);
            partialMortalityTable[species] = newValue;
        }
        //---------------------------------------------------------------------
        public void SetCompleteMortalityThreshold(ISpecies species, int newValue)
        {
            Debug.Assert(species != null);
            completeMortalityThreshold[species] = newValue;
        }
        //---------------------------------------------------------------------
        public void SetCompleteMortalityTime(ISpecies species, int newValue)
        {
            Debug.Assert(species != null);
            if (newValue > 10)
                throw new InputValueException(newValue.ToString(), "Value must be <= 10");

            completeMortalityTime[species] = newValue;
        }
        //---------------------------------------------------------------------
        public InputParameters()
        {
            partialMortalityTable = new SpeciesAuxParm<List<AgeClass>>(PlugIn.ModelCore.Species);
            foreach (ISpecies spp in PlugIn.ModelCore.Species)
            {
                partialMortalityTable[spp] = new List<AgeClass>();
            }

            completeMortalityThreshold = new SpeciesAuxParm<int>(PlugIn.ModelCore.Species);
            completeMortalityTime = new SpeciesAuxParm<int>(PlugIn.ModelCore.Species);
        }
    }
}
