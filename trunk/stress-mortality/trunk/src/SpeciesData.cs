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
        //public static SpeciesAuxParm<Dictionary<int, Dictionary<int, int>>> CumulativeMortality;
        

        //---------------------------------------------------------------------
        public static void Initialize(IInputParameters parameters)
        {
            //foreach (ISpecies spp in PlugIn.ModelCore.Species)
            //{
                // Dictionary = time, age, reduction.

                //Dictionary<int, int> cohortAgeReductions = new Dictionary<int, int>();
                //CumulativeMortality[spp].Add(0, cohortAgeReductions);
                //PlugIn.ModelCore.Log.WriteLine("  Reading age classes for {0}.", spp.Name);
                //foreach (AgeClass ac in parameters.PartialMortalityTable[spp])
                //{
                //    PlugIn.ModelCore.Log.WriteLine("  Reading {0} age class: {1}-{2}, fraction={3}.", spp.Name, ac.LwrAge, ac.UprAge, ac.MortalityFraction);
                //}
            //}

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
