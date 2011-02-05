using Landis.Biomass;
using Landis.Harvest;

using AgeCohorts = Landis.AgeCohort;
using BaseHarvest = Landis.Harvest;


namespace Landis.Extension.LeafBiomassHarvest
{
    /// <summary>
    /// Various methods for selecting which of a species' cohorts to harvest.
    /// </summary>
    public static class SelectCohorts 
    {
        /// <summary>
        /// A method for selecting which of a species' cohorts to harvest.
        /// </summary>
        public delegate void Method(AgeCohorts.ISpeciesCohorts         cohorts,
                                    AgeCohorts.ISpeciesCohortBoolArray isHarvested);

        //---------------------------------------------------------------------

        /// <summary>
        /// Selects all of a species' cohorts for harvesting.
        /// </summary>
        public static void All(AgeCohorts.ISpeciesCohorts         cohorts,
                               AgeCohorts.ISpeciesCohortBoolArray isHarvested)
        {
            //loop through all cohorts and mark as harvested
            foreach (ICohort cohort in ((ISpeciesCohorts) cohorts)) 
                PartialHarvestDisturbance.RecordBiomassReduction(cohort, (cohort.LeafBiomass + cohort.WoodBiomass));

            for (int i = 0; i < isHarvested.Count; i++)
                isHarvested[i] = true;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Selects the oldest of a species' cohorts for harvesting.
        /// </summary>
        public static void Oldest(AgeCohorts.ISpeciesCohorts         cohorts,
                                  AgeCohorts.ISpeciesCohortBoolArray isHarvested)
        {
            //  Oldest is first.
            isHarvested[0] = true;
            foreach (ICohort cohort in ((ISpeciesCohorts) cohorts)) 
            {
                PartialHarvestDisturbance.RecordBiomassReduction(cohort, (cohort.LeafBiomass + cohort.WoodBiomass));
                break;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Selects the youngest of a species' cohorts for harvesting.
        /// </summary>
        public static void Youngest(AgeCohorts.ISpeciesCohorts         cohorts,
                                    AgeCohorts.ISpeciesCohortBoolArray isHarvested)
        {
            //  Youngest is last.
            isHarvested[isHarvested.Count - 1] = true;
            
            int i=0;
            foreach (ICohort cohort in ((ISpeciesCohorts) cohorts)) 
            {
                if (i == isHarvested.Count - 1)
                    PartialHarvestDisturbance.RecordBiomassReduction(cohort, (cohort.LeafBiomass + cohort.WoodBiomass));
                i++;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Selects all of a species' cohorts for harvesting except the oldest.
        /// </summary>
        public static void AllExceptOldest(AgeCohorts.ISpeciesCohorts         cohorts,
                                           AgeCohorts.ISpeciesCohortBoolArray isHarvested)
        {
            //  Oldest is first (so start at i = 1 instead of i = 0)
            for (int i = 1; i < isHarvested.Count; i++)
                isHarvested[i] = true;
            
            int j=0;
            foreach (ICohort cohort in ((ISpeciesCohorts) cohorts)) 
            {
                if (j != 0)
                    PartialHarvestDisturbance.RecordBiomassReduction(cohort, (cohort.LeafBiomass + cohort.WoodBiomass));
                j++;
            }



        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Selects all of a species' cohorts for harvesting except the
        /// youngest.
        /// </summary>
        public static void AllExceptYoungest(AgeCohorts.ISpeciesCohorts         cohorts,
                                             AgeCohorts.ISpeciesCohortBoolArray isHarvested)
        {
            //  Youngest is last.
            int youngestIndex = isHarvested.Count - 1;
            for (int i = 0; i < youngestIndex; i++)
                isHarvested[i] = true;
            
            int j=0;
            foreach (ICohort cohort in ((ISpeciesCohorts) cohorts)) 
            {
                if (j != youngestIndex)
                    PartialHarvestDisturbance.RecordBiomassReduction(cohort, (cohort.LeafBiomass + cohort.WoodBiomass));
                j++;
            }


        }
    }
}
