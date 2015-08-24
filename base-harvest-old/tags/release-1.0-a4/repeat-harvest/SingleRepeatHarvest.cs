using Landis.Succession;

namespace Landis.Harvest
{
    /// <summary>
    /// A single repeat-harvest harvests stands and then sets them aside for
    /// just one additional harvest.  The additional harvest can remove a
    /// different set of cohorts than the initial harvest.
    /// </summary>
    public class SingleRepeatHarvest
        : RepeatHarvest
    {
        private ICohortSelector initialCohortSelector;
        private Planting.SpeciesList initialSpeciesToPlant;

        private ICohortSelector additionalCohortSelector;
        private Planting.SpeciesList additionalSpeciesToPlant;

        //---------------------------------------------------------------------

        public SingleRepeatHarvest(string               name,
                                   IStandRankingMethod  rankingMethod,
                                   ISiteSelector        siteSelector,
                                   ICohortSelector      cohortSelector,
                                   Planting.SpeciesList speciesToPlant,
                                   ICohortSelector      additionalCohortSelector,
                                   Planting.SpeciesList additionalSpeciesToPlant,
                                   int                  interval)
            : base(name, rankingMethod, siteSelector, cohortSelector, speciesToPlant, interval)
        {
            this.initialCohortSelector = cohortSelector;
            this.initialSpeciesToPlant = speciesToPlant;

            this.additionalCohortSelector = additionalCohortSelector;
            this.additionalSpeciesToPlant = additionalSpeciesToPlant;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Harvests a stand (and possibly its neighbors) according to the
        /// repeat-harvest's site-selection method.
        /// </summary>
        /// <returns>
        /// The area that was harvested (units: hectares).
        /// </returns>
        public override double Harvest(Stand stand)
        {
            if (stand.IsSetAside) {
                CohortSelector = additionalCohortSelector;
                SpeciesToPlant = additionalSpeciesToPlant;
            }
            else {
                CohortSelector = initialCohortSelector;
                SpeciesToPlant = initialSpeciesToPlant;
            }
            return base.Harvest(stand);
        }
    }
}
