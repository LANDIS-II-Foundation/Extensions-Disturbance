using Landis.Core;
using Landis.Extension.BaseHarvest;
using System.Collections.Generic;

namespace Landis.Extension.BiomassHarvest
{
    public static class StandMethods
    {
        // Keys are stand map codes; values are mapping of species' names and biomass totals
        private static IDictionary<uint, int[]> biomassRemovedBySpecies;

        //--------------------------------------------------------------------

        static int[] BiomassRemovedBySpecies(this Stand stand)
        {
            if (biomassRemovedBySpecies == null)
                biomassRemovedBySpecies = new Dictionary<uint, int[]>();

            int[] biomassRemovedPerSpecies;
            if (biomassRemovedBySpecies.TryGetValue(stand.MapCode, out biomassRemovedPerSpecies))
                return biomassRemovedPerSpecies;

            biomassRemovedPerSpecies = new int[Model.Core.Species.Count];
            biomassRemovedBySpecies[stand.MapCode] = biomassRemovedPerSpecies;
            return biomassRemovedPerSpecies;
        }

        //--------------------------------------------------------------------

        public static void RecordBiomassRemoved(this Stand stand,
                                                ISpecies species,
                                                int reduction)
        {
            stand.BiomassRemovedBySpecies()[species.Index] += reduction;
        }

        //--------------------------------------------------------------------

        public static int GetBiomassRemoved(this Stand stand,
                                            ISpecies species)
        {
            return stand.BiomassRemovedBySpecies()[species.Index];
        }

        //--------------------------------------------------------------------

        public static void ResetBiomassRemoved(this Stand stand)
        {
            int[] biomassRemovedPerSpecies = stand.BiomassRemovedBySpecies();
            for (int i = 0; i < biomassRemovedPerSpecies.Length; i++)
                biomassRemovedPerSpecies[i] = 0;
        }
    }
}
