using Landis.SpatialModeling;
using Landis.Library.BiomassCohorts;
using Landis.Core;
using System.Collections.Generic;
using Edu.Wisc.Forest.Flel.Util;
using Landis.Library.Succession;

namespace Landis.Extension.StressMortality
{
    public class SpeciesData
    {

        public static SpeciesAuxParm<List<AgeClass>> PartialMortalityTable;
        public static SpeciesAuxParm<int> CompleteMortalityThreshold;
        public static SpeciesAuxParm<int> CompleteMortalityTime;
        public static SpeciesAuxParm<EcoregionsAuxParm<int>> SppBiomassRemoved;
        public static SpeciesAuxParm<EcoregionsAuxParm<int>> CohortsKilled;
        

        //---------------------------------------------------------------------
        public static void Initialize(IInputParameters parameters)
        {

            PartialMortalityTable = parameters.PartialMortalityTable;
            CompleteMortalityThreshold = parameters.CompleteMortalityThreshold;
            CompleteMortalityTime = parameters.CompleteMortalityThreshold;
            SppBiomassRemoved = CreateSpeciesEcoregionParm<int>(PlugIn.ModelCore.Species, PlugIn.ModelCore.Ecoregions);
            CohortsKilled = CreateSpeciesEcoregionParm<int>(PlugIn.ModelCore.Species, PlugIn.ModelCore.Ecoregions);
        }

        public static bool IsOnsetYear(int year, ISpecies species, IEcoregion ecoregion)
        {

            if (DynamicInputs.AllData.ContainsKey(year))
            {

               DynamicInputs.TimestepData = DynamicInputs.AllData[year];
               foreach (IDynamicInputRecord dynrec in DynamicInputs.TimestepData)
                   if (dynrec.OnsetEcoregion == ecoregion && dynrec.OnsetSpecies == species)
                       return true;

            }

            return false;

        }
        //---------------------------------------------------------------------

        private static SpeciesAuxParm<EcoregionsAuxParm<int>> CreateSpeciesEcoregionParm<T>(ISpeciesDataset speciesDataset, IEcoregionDataset ecoregionDataset)
        {
            SpeciesAuxParm<EcoregionsAuxParm<int>> newParm;
            newParm = new SpeciesAuxParm<EcoregionsAuxParm<int>>(speciesDataset);
            foreach (ISpecies species in speciesDataset)
            {
                newParm[species] = new EcoregionsAuxParm<int>(ecoregionDataset);
            }
            return newParm;
        }
    
    }
}
