using Landis.Library.BiomassCohorts;
using Landis.SpatialModeling;
using Landis.Core;

using System.Collections.Generic;

namespace Landis.Extension.StressMortality
{
    /// <summary>
    /// A biomass disturbance that handles partial thinning of cohorts.
    /// </summary>
    public class PartialDisturbance
        : IDisturbance
    {
        private static PartialDisturbance singleton;
        private static IDictionary<ushort, int>[] reductions;
        private static ActiveSite currentSite;

        //---------------------------------------------------------------------
        ActiveSite Landis.Library.BiomassCohorts.IDisturbance.CurrentSite
        {
            get
            {
                return currentSite;
            }
        }

        //---------------------------------------------------------------------
        ExtensionType IDisturbance.Type
        {
            get
            {
                return PlugIn.ExtType;
            }
        }

        //---------------------------------------------------------------------
        static PartialDisturbance()
        {
            singleton = new PartialDisturbance();
        }

        //---------------------------------------------------------------------
        public PartialDisturbance()
        {
        }

        //---------------------------------------------------------------------
        int IDisturbance.ReduceOrKillMarkedCohort(ICohort cohort)
        {
            IEcoregion ecoregion = PlugIn.ModelCore.Ecoregion[currentSite];
            int reduction;
            if (reductions[cohort.Species.Index].TryGetValue(cohort.Age, out reduction))
            {
                if (reduction == cohort.Biomass)
                {
                    PlugIn.StressCohortsKilled[ecoregion.Index]++;
                    SpeciesData.CohortsKilled[cohort.Species][ecoregion]++;
                }
                SiteVars.StressBioRemoved[currentSite] += reduction;
                SpeciesData.SppBiomassRemoved[cohort.Species][ecoregion] += reduction;
                return reduction;
            }
            else
            return 0;
        }
        //---------------------------------------------------------------------

        public static void Initialize()
        {
            reductions = new IDictionary<ushort, int>[PlugIn.ModelCore.Species.Count];
            for (int i = 0; i < reductions.Length; i++)
                reductions[i] = new Dictionary<ushort, int>();
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Reduces the biomass of cohorts that have been marked for partial
        /// reduction.
        /// </summary>
        public static void ReduceCohortBiomass(ActiveSite site)
        {
            currentSite = site;

            //PlugIn.ModelCore.Log.WriteLine("ReducingCohortBiomass NOW!");

            SiteVars.Cohorts[site].ReduceOrKillBiomassCohorts(singleton);

        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Records the biomass reduction for a particular cohort.
        /// </summary>
        public static void RecordBiomassReduction(ActiveSite site, ICohort cohort, double reductionFraction)
        {
            if (reductionFraction <= 0.0)
                return;

            int currentYear = PlugIn.ModelCore.CurrentTime;
            int successionTime = Landis.Extension.Succession.Biomass.PlugIn.SuccessionTimeStep;

            //PlugIn.ModelCore.Log.WriteLine("Recording reduction:  {0:0.0}/{1:0.0}/{2}.", cohort.Species.Name, cohort.Age, reduction);
            reductions[cohort.Species.Index][cohort.Age] = (int) (reductionFraction * cohort.Biomass);

            int reduction = (int)(reductionFraction * 100.0);

            // ADD TO THE DICTIONARY HERE.  
            // Dictionary = Year of Reduction, Year Cohort Added, Amount of Reduction
            // Year cohort added used as cohort age will change over time.  If we used cohort age,
            // we would lose track of cohorts during succession time steps.
            Dictionary<int,int> newEntry = new Dictionary<int,int>();

            int cohortAddYear = currentYear - cohort.Age - currentYear%successionTime;  
            newEntry.Add(cohortAddYear,reduction);

            //PlugIn.ModelCore.Log.WriteLine("R/C={0}/{1}:  Trying to add key: {2} time:{3}, add year:{4}, reduction:{5}, AGB={6}.", site.Location.Row, site.Location.Column, cohort.Species.Name, PlugIn.ModelCore.CurrentTime, cohortAddYear, reduction, cohort.Biomass);

            if (SiteVars.CumulativeMortality[site][cohort.Species].ContainsKey(currentYear))
                SiteVars.CumulativeMortality[site][cohort.Species][currentYear].Add(cohortAddYear, reduction);
            else
                SiteVars.CumulativeMortality[site][cohort.Species].Add(currentYear, newEntry);

            // Calculate cumulative mortality; begin by including this year's mortality (reduction).
            // Look at the past 3 years only.
            int cumulativeMortality = reduction;
            int numYears = SpeciesData.CompleteMortalityTime[cohort.Species];

            Dictionary<int, int> cohortAgeReductions;
            for (int y = 1; y <= numYears; y++)
            {
                if (SiteVars.CumulativeMortality[site][cohort.Species].TryGetValue(currentYear - y, out cohortAgeReductions))
                {
                    int annualReduction = 0;
                    if(cohortAgeReductions.TryGetValue(cohortAddYear, out annualReduction))
                        cumulativeMortality += annualReduction;
                }
            }

            // If exceeds the limit, remove cohort.
            if (cumulativeMortality > SpeciesData.CompleteMortalityThreshold[cohort.Species])
            {
                //PlugIn.ModelCore.Log.WriteLine("R/C={0}/{1}:  Trying to add mortality: {2} time:{3}, age:{4}, cumulative mortality:{5}, trigger={6}.", site.Location.Row, site.Location.Column, cohort.Species.Name, PlugIn.ModelCore.CurrentTime, cohort.Age, cumulativeMortality, SpeciesData.CompleteMortalityTable[cohort.Species]);
                reductions[cohort.Species.Index][cohort.Age] = cohort.Biomass;
            }

            // Remove any keys more than 4 years old to keep this dictionary relatively small.
            if (SiteVars.CumulativeMortality[site][cohort.Species].TryGetValue(currentYear - numYears + 1, out cohortAgeReductions))
                SiteVars.CumulativeMortality[site][cohort.Species].Remove(currentYear - numYears + 1);

        }
    }
}
