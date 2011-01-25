using Landis.AgeCohort;
using Landis.Landscape;
using Landis.PlugIns;
using Landis.Succession;

namespace Landis.Harvest
{
    /// <summary>
    /// A prescription describes how stands are ranked, how sites are selected,
    /// and which cohorts are harvested.
    /// </summary>
    public class Prescription
        : ISpeciesCohortsDisturbance
    {
        private static int nextNumber = 1;
        private int number;
        private string name;
        private IStandRankingMethod rankingMethod;
        private ISiteSelector siteSelector;
        private ICohortSelector cohortSelector;
        private Planting.SpeciesList speciesToPlant;
        private ActiveSite currentSite;

        //---------------------------------------------------------------------

        /// <summary>
        /// The prescription's number.
        /// </summary>
        /// <remarks>
        /// Each prescription's number is unique, and is generated and assigned
        /// when the prescription is initialized.
        /// </remarks>
        public int Number
        {
            get {
                return number;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The prescription's name.
        /// </summary>
        public string Name
        {
            get {
                return name;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The prescription's method for ranking stands.
        /// </summary>
        public IStandRankingMethod StandRankingMethod
        {
            get {
                return rankingMethod;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The prescription's method for selecting sites in stands.
        /// </summary>
        public ISiteSelector SiteSelectionMethod
        {
            get {
                return siteSelector;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Sets the cohorts that will be removed by the prescription.
        /// </summary>
        /// <remarks>
        /// The purpose of this property is to allow derived classes to change
        /// the cohort selector; for example, a single repeat-harvest switching
        /// between its two cohort selectors.
        /// </remarks>
        protected ICohortSelector CohortSelector
        {
            set {
                cohortSelector = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Sets the optional list of species that are planted at each site
        /// harvested by the prescription.
        /// </summary>
        /// <remarks>
        /// The purpose of this property is to allow derived classes to change
        /// the species list; for example, a single repeat-harvest switching
        /// between the lists for initial harvests and repeat harvests.
        /// </remarks>
        protected Planting.SpeciesList SpeciesToPlant
        {
            set {
                speciesToPlant = value;
            }
        }

        //---------------------------------------------------------------------

        PlugInType IDisturbance.Type
        {
            get {
                return PlugIn.Type;
            }
        }

        //---------------------------------------------------------------------

        ActiveSite IDisturbance.CurrentSite
        {
            get {
                return currentSite;
            }
        }

        //---------------------------------------------------------------------

        public Prescription(string               name,
                            IStandRankingMethod  rankingMethod,
                            ISiteSelector        siteSelector,
                            ICohortSelector      cohortSelector,
                            Planting.SpeciesList speciesToPlant)
        {
            this.number = nextNumber;
            nextNumber++;

            this.name = name;
            this.rankingMethod = rankingMethod;
            this.siteSelector = siteSelector;
            this.cohortSelector = cohortSelector;
            this.speciesToPlant = speciesToPlant;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Harvests a stand (and possibly its neighbors) according to the
        /// prescription's site-selection method.
        /// </summary>
        /// <returns>
        /// The area that was harvested (units: hectares).
        /// </returns>
        public virtual double Harvest(Stand stand)
        {
            foreach (ActiveSite site in siteSelector.SelectSites(stand)) {
                currentSite = site;
                ISiteCohorts cohorts = Model.LandscapeCohorts[site];
                cohorts.DamageBy(this);
                SiteVars.Prescription[site] = this;

                if (speciesToPlant != null)
                    Succession.Reproduction.ScheduleForPlanting(speciesToPlant,
                                                                site);
            }
            return siteSelector.AreaSelected;
        }

        //---------------------------------------------------------------------

        void ISpeciesCohortsDisturbance.Damage(ISpeciesCohorts         cohorts,
                                               ISpeciesCohortBoolArray isDamaged)
        {
            cohortSelector.Harvest(cohorts, isDamaged);

            int cohortsKilled = 0;
            for (int i = 0; i < isDamaged.Count; i++)
                if (isDamaged[i])
                    cohortsKilled++;
            SiteVars.CohortsKilled[currentSite] = cohortsKilled;
        }
    }
}
