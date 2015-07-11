// Copyright 2008-2010 Green Code LLC, Portland State University
// Authors:  James B. Domingo, Robert M. Scheller, Srinivas S.

using Edu.Wisc.Forest.Flel.Util;
using Landis.Library.LeafBiomassCohorts;
using Landis.Core;
using Landis.Extension.BaseHarvest;

using System.Collections.Generic;

using AgeCohorts = Landis.Library.AgeOnlyCohorts;

namespace Landis.Extension.LeafBiomassHarvest
{
    /// <summary>
    /// Selects specific ages and ranges of ages among a species' cohorts
    /// for harvesting.
    /// </summary>
    public class SpecificAgesCohortSelector
    {
        private static Percentage defaultPercentage;

        private IList<ushort> ages;
        private IList<AgeRange> ranges;
        private IDictionary<ushort, Percentage> percentages;

        //---------------------------------------------------------------------

        static SpecificAgesCohortSelector()
        {
            defaultPercentage = Percentage.Parse("100%");
        }

        //---------------------------------------------------------------------

        public SpecificAgesCohortSelector(IList<ushort>                   ages,
                                          IList<AgeRange>                 ranges,
                                          IDictionary<ushort, Percentage> percentages)
        {
            this.ages = new List<ushort>(ages);
            this.ranges = new List<AgeRange>(ranges);
            this.percentages = new Dictionary<ushort, Percentage>(percentages);
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Selects which of a species' cohorts are harvested.
        /// </summary>
        public void SelectCohorts(AgeCohorts.ISpeciesCohorts         cohorts,
                                  AgeCohorts.ISpeciesCohortBoolArray isHarvested)
        {
            int i = 0;
            foreach (ICohort cohort in ((ISpeciesCohorts) cohorts)) {
                bool cohortSelected = false;
                ushort ageToLookUp = 0;
                if (ages.Contains(cohort.Age)) {
                    cohortSelected = true;
                    ageToLookUp = cohort.Age;
                }
                else {
                    foreach (AgeRange range in ranges) {
                        if (range.Contains(cohort.Age)) {
                            cohortSelected = true;
                            ageToLookUp = range.Start;
                            break;
                        }
                    }
                }
                if (cohortSelected) {
                    Percentage percentage;
                    if (! percentages.TryGetValue(ageToLookUp, out percentage))
                        percentage = defaultPercentage;
                    int reduction = (int) System.Math.Round((cohort.LeafBiomass + cohort.WoodBiomass) * percentage);
                    if (reduction < (cohort.LeafBiomass + cohort.WoodBiomass))
                        PartialHarvestDisturbance.RecordBiomassReduction(cohort, reduction);
                    else
                    {
                        isHarvested[i] = true;
                        PartialHarvestDisturbance.RecordBiomassReduction(cohort, reduction);
                    }
                }
                i++;
            }
        }
    }
}
