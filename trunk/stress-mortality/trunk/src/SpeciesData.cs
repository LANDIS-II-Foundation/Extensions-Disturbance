using Landis.SpatialModeling;
using Landis.Library.BiomassCohorts;
using Landis.Core;
using System.Collections.Generic;
using Edu.Wisc.Forest.Flel.Util;
using Landis.Library.Succession;
using Landis.Extension.Succession.Biomass.Species;

namespace Landis.Extension.StressMortality
{
    public class SpeciesData
    {

        public static Landis.Extension.Succession.Biomass.Species.AuxParm<List<AgeClass>> PartialMortalityTable;
        public static Landis.Extension.Succession.Biomass.Species.AuxParm<int> CompleteMortalityTable;

        //---------------------------------------------------------------------
        public static void Initialize(IInputParameters parameters)
        {
            /*foreach (ISpecies spp in PlugIn.ModelCore.Species)
            {
                PlugIn.ModelCore.Log.WriteLine("  Reading age classes for {0}.", spp.Name);
                foreach (AgeClass ac in parameters.PartialMortalityTable[spp])
                {
                    PlugIn.ModelCore.Log.WriteLine("  Reading {0} age class: {1}-{2}, fraction={3}.", spp.Name, ac.LwrAge, ac.UprAge, ac.MortalityFraction);
                }
            }*/

            PartialMortalityTable = parameters.PartialMortalityTable;
            CompleteMortalityTable = parameters.CompleteMortalityTable;
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
