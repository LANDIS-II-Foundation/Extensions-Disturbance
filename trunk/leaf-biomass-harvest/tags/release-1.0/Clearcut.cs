//using Landis.AgeCohort;
using Edu.Wisc.Forest.Flel.Util;

using Landis.Biomass;
using Landis.Harvest;

using AgeCohorts = Landis.AgeCohort;
using BaseHarvest = Landis.Harvest;

namespace Landis.Extension.LeafBiomassHarvest
{
    /// <summary>
    /// Removes all the cohorts at a site.
    /// </summary>
    public class ClearCut
        : ICohortSelector
    {
        public ClearCut()
        {
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Selects which of a species' cohorts are harvested.
        /// </summary>
        public void Harvest(AgeCohorts.ISpeciesCohorts         cohorts,
                            AgeCohorts.ISpeciesCohortBoolArray isHarvested)
        {
            foreach (ICohort cohort in ((ISpeciesCohorts) cohorts)) 
                PartialHarvestDisturbance.RecordBiomassReduction(cohort, (cohort.LeafBiomass + cohort.WoodBiomass));
            
            for (int i = 0; i < isHarvested.Count; i++)
                isHarvested[i] = true;
        }
    }
}
