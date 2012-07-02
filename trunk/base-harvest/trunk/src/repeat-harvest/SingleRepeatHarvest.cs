//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:  Robert M. Scheller, James B. Domingo

using Landis.Core;
using Landis.Library.Succession;

namespace Landis.Extension.BaseHarvest
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
        private ISiteSelector additionalSiteSelector;

        //---------------------------------------------------------------------

        public SingleRepeatHarvest(string               name,
                                   IStandRankingMethod  rankingMethod,
                                   ISiteSelector        siteSelector,
                                   ICohortSelector      cohortSelector,
                                   Planting.SpeciesList speciesToPlant,
                                   ICohortSelector      additionalCohortSelector,
                                   Planting.SpeciesList additionalSpeciesToPlant,
                                   int                  minTimeSinceDamage,
                                   bool                 preventEstablishment,
                                   int                  interval)
            : base(name, rankingMethod, siteSelector, cohortSelector, speciesToPlant, minTimeSinceDamage, preventEstablishment, interval)
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
        public override void Harvest(Stand stand)
        {
            if (stand.IsSetAside) {
                CohortSelector = additionalCohortSelector;
                SpeciesToPlant = additionalSpeciesToPlant;
                SiteSelector = new CompleteStand();
                
            }
            else {
                CohortSelector = initialCohortSelector;
                SpeciesToPlant = initialSpeciesToPlant;
            }
            base.Harvest(stand);
            return; //base.Harvest(stand);

            //                 ISiteSelector siteSelector = ReadSiteSelector();

        }
    }
}
