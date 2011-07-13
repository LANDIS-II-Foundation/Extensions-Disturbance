
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
        int Timestep { get; set; }
        string MapNamesTemplate { get; set; }
        string LogFileName { get; set; }
        SpeciesAuxParm<List<AgeClass>> PartialMortalityTable { get; }
        SpeciesAuxParm<int> CompleteMortalityTable { get; }
        
    }

    class InputParameters
        : IInputParameters
    {
        private int timestep;
        private string mapNamesTemplate;
        private string logFileName;
        private SpeciesAuxParm<List<AgeClass>> partialMortalityTable;
        private SpeciesAuxParm<int> completeMortalityTable;
        //---------------------------------------------------------------------
        /// <summary>
        /// Timestep (years)
        /// </summary>
        public int Timestep
        {
            get
            {
                return timestep;
            }
            set
            {
                if (value < 0)
                    throw new InputValueException(value.ToString(),
                                                  "Value must be = or > 0.");
                timestep = value;
            }
        }

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
        public SpeciesAuxParm<int> CompleteMortalityTable
        {
            get
            {
                return completeMortalityTable;
            }
        }

        //---------------------------------------------------------------------
        public void SetPartialMortalityTable(ISpecies species, List<AgeClass> newValue)
        {
            Debug.Assert(species != null);
            partialMortalityTable[species] = newValue;
        }
        //---------------------------------------------------------------------
        public void SetCompleteMortalityTable(ISpecies species, int newValue)
        {
            Debug.Assert(species != null);
            completeMortalityTable[species] = newValue;
        }
        //---------------------------------------------------------------------
        public InputParameters()
        {
            partialMortalityTable = new SpeciesAuxParm<List<AgeClass>>(PlugIn.ModelCore.Species);
            foreach (ISpecies spp in PlugIn.ModelCore.Species)
            {
                partialMortalityTable[spp] = new List<AgeClass>();
            }

            completeMortalityTable = new SpeciesAuxParm<int>(PlugIn.ModelCore.Species);
        }
    }
}
