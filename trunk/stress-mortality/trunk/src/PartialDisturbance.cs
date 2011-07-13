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
                return PlugIn.Type;
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
            int reduction;
            if (reductions[cohort.Species.Index].TryGetValue(cohort.Age, out reduction))
            {

                SiteVars.StressBioRemoved[currentSite] += reduction;
                //SiteVars.CohortsPartiallyDamaged[currentSite]++;

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
        public static void RecordBiomassReduction(ActiveSite site, ICohort cohort, int reduction)
        {
            if (reduction <= 0)
                return;

            //PlugIn.ModelCore.Log.WriteLine("Recording reduction:  {0:0.0}/{1:0.0}/{2}.", cohort.Species.Name, cohort.Age, reduction);
            reductions[cohort.Species.Index][cohort.Age] = reduction;

            // ADD TO THE DICTIONARY HERE.
            Dictionary<int,int> newEntry = new Dictionary<int,int>();
            newEntry.Add(cohort.Age,reduction);

            PlugIn.ModelCore.Log.WriteLine("R/C={0}/{1}:  Trying to add key: {2} time:{3}, age:{4}, reduction:{5}, AGB={6}.", site.Location.Row, site.Location.Column, cohort.Species.Name, PlugIn.ModelCore.CurrentTime, cohort.Age, reduction, cohort.Biomass);

            if (SiteVars.CumulativeMortality[site][cohort.Species].ContainsKey(PlugIn.ModelCore.CurrentTime))
                SiteVars.CumulativeMortality[site][cohort.Species][PlugIn.ModelCore.CurrentTime].Add(cohort.Age, reduction);
            else
                SiteVars.CumulativeMortality[site][cohort.Species].Add(PlugIn.ModelCore.CurrentTime, newEntry);

            // Calculate cumulative mortality; begin by including this year's mortality (reduction).
            // Look at the past 3 years only.
            int cumulativeMortality = reduction;  
            Dictionary<int, int> cohortAgeReductions;
            for (int y = 1; y <= 3; y++)
            {
                if (SiteVars.CumulativeMortality[site][cohort.Species].TryGetValue(PlugIn.ModelCore.CurrentTime - y, out cohortAgeReductions))
                {
                    int annualReduction = 0;
                    if(cohortAgeReductions.TryGetValue(cohort.Age - y, out annualReduction))
                        cumulativeMortality += annualReduction;
                }
            }

            // If exceeds the limit, remove cohort.
            if (cumulativeMortality > SpeciesData.CompleteMortalityTable[cohort.Species])
                PartialDisturbance.RecordBiomassReduction(site, cohort, cohort.Biomass); 

            // Remove any keys more than 4 years old to keep this dictionary relatively small.
            if (SiteVars.CumulativeMortality[site][cohort.Species].TryGetValue(PlugIn.ModelCore.CurrentTime - 4, out cohortAgeReductions))
                SiteVars.CumulativeMortality[site][cohort.Species].Remove(PlugIn.ModelCore.CurrentTime - 4);

        }
    }
}
