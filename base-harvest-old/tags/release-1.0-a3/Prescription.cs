using Landis.AgeCohort;
using Landis.Landscape;
using Landis.PlugIns;

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

        public Prescription(string              name,
                            IStandRankingMethod rankingMethod,
                            ISiteSelector       siteSelector,
                            ICohortSelector     cohortSelector)
        {
            this.number = nextNumber;
            nextNumber++;

            this.name = name;
            this.rankingMethod = rankingMethod;
            this.siteSelector = siteSelector;
            this.cohortSelector = cohortSelector;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Harvests a stand (and possibly its neighbors) according to the
        /// prescription's site-selection method.
        /// </summary>
        /// <returns>
        /// The area that was harvested (units: hectares).
        /// </returns>
        public double Harvest(Stand stand)
        {
            foreach (ActiveSite site in siteSelector.SelectSites(stand)) {
                currentSite = site;
                ISiteCohorts cohorts = Model.LandscapeCohorts[site];
                cohorts.DamageBy(this);
                SiteVars.Prescription[site] = this;
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
