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
        public static SpeciesAuxParm<int> CompleteMortalityTable;
        public static SpeciesAuxParm<int> SppBiomassRemoved;
        public static SpeciesAuxParm<int> CohortsKilled;
        

        //---------------------------------------------------------------------
        public static void Initialize(IInputParameters parameters)
        {

            PartialMortalityTable = parameters.PartialMortalityTable;
            CompleteMortalityTable = parameters.CompleteMortalityTable;
            SppBiomassRemoved = new SpeciesAuxParm<int>(PlugIn.ModelCore.Species);
            CohortsKilled = new SpeciesAuxParm<int>(PlugIn.ModelCore.Species);
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
    
    }
}
