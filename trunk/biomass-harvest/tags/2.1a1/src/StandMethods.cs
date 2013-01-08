using Landis.Core;
using Landis.Extension.BaseHarvest;
using System.Collections.Generic;

namespace Landis.Extension.BiomassHarvest
{
    public static class StandMethods
    {
        // Keys are stand map codes; values are mapping of species' names and biomass totals
        private static IDictionary<uint, IList<int>> biomassRemovedBySpecies;

        //--------------------------------------------------------------------

        static void Initialize()
        {
            biomassRemovedBySpecies = new Dictionary<uint, IList<int>>();
        }

        //--------------------------------------------------------------------

        public static void RecordBiomassRemoved(this Stand stand,
                                                ISpecies species,
                                                int reduction)
        {
            if (biomassRemovedBySpecies == null)
                Initialize();
            if (biomassRemovedBySpecies[stand.MapCode] == null)
                biomassRemovedBySpecies[stand.MapCode] = new List<int>(Model.Core.Species.Count);

            biomassRemovedBySpecies[stand.MapCode][species.Index] += reduction;
        }

        //--------------------------------------------------------------------

        public static int GetBiomassRemoved(this Stand stand,
                                            ISpecies species)
        {
            if (biomassRemovedBySpecies == null)
                Initialize();
            return biomassRemovedBySpecies[stand.MapCode][species.Index];
        }

        //--------------------------------------------------------------------

        public static void ResetBiomassRemoved(this Stand stand)
        {
            if (biomassRemovedBySpecies == null)
                Initialize();
            foreach (IList<int> speciesTotals in biomassRemovedBySpecies.Values)
            {
                for (int i = 0; i < speciesTotals.Count; i++)
                    speciesTotals[i] = 0;
            }
        }
    }
}
