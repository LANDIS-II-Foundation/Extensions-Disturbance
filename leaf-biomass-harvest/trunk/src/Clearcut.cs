// Copyright 2008-2010 Green Code LLC, Portland State University
// Authors:  James B. Domingo, Robert M. Scheller, Srinivas S.

using Edu.Wisc.Forest.Flel.Util;

using Landis.Library.LeafBiomassCohorts;
using Landis.Extension.BaseHarvest;

using AgeCohorts = Landis.Library.AgeOnlyCohorts;
using BaseHarvest = Landis.Extension.BaseHarvest;

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
