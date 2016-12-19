// Copyright 2008-2010 Green Code LLC, Portland State University
// Authors:  James B. Domingo, Robert M. Scheller, Srinivas S.

using Landis.Core;
using Landis.Library.LeafBiomassCohorts;
using Landis.SpatialModeling;
using System.Collections.Generic;
using Landis.Extension.BaseHarvest;
using BaseHarvest = Landis.Extension.BaseHarvest;


namespace Landis.Extension.LeafBiomassHarvest
{
    /// <summary>
    /// A biomass disturbance that handles partial thinning of cohorts.
    /// </summary>
    public class PartialHarvestDisturbance
        : IDisturbance
    {
        private static PartialHarvestDisturbance singleton;
        private static IDictionary<ushort, float>[] reductions;

        private static ActiveSite currentSite;
        private static Stand originalStand;  // the originating stand

        private static int numberCohortsReduced;
        private static double capacityReduction;
        //---------------------------------------------------------------------

        ActiveSite IDisturbance.CurrentSite
        {
            get {
                return currentSite;
            }
        }

        //---------------------------------------------------------------------

        ExtensionType IDisturbance.Type
        {
            get {
                return PlugIn.type;
            }
        }

        //---------------------------------------------------------------------

        static PartialHarvestDisturbance()
        {
            singleton = new PartialHarvestDisturbance();
        }

        //---------------------------------------------------------------------

        public PartialHarvestDisturbance()
        {
        }

        //---------------------------------------------------------------------

        float[] IDisturbance.ReduceOrKillMarkedCohort(ICohort cohort)
        {
            float reduction;
            float[] leafWoodReduction = new float[2]{0F, 0F};
            
            if (reductions[cohort.Species.Index].TryGetValue(cohort.Age, out reduction)) {
            
                leafWoodReduction[0] = cohort.WoodBiomass / (cohort.LeafBiomass + cohort.WoodBiomass) * (float) reduction;
                leafWoodReduction[1] = cohort.LeafBiomass / (cohort.LeafBiomass + cohort.WoodBiomass) * (float) reduction;
                
                SiteVars.BiomassRemoved[currentSite] += (int) reduction;
                SiteVars.CohortsPartiallyDamaged[currentSite]++;

                if (originalStand.LastPrescription.PreventEstablishment)
                {
                    numberCohortsReduced++;
                    capacityReduction += (double)reduction / (double)cohort.Biomass;
                }
                // Record any cohort touched, not just killed:
                BaseHarvest.SiteVars.Stand[currentSite].UpdateDamageTable(cohort.Species.Name);
                
                return leafWoodReduction;
            }
            else
                return leafWoodReduction;
        }

        //---------------------------------------------------------------------

        public static void Initialize()
        {
            reductions = new IDictionary<ushort, float>[PlugIn.ModelCore.Species.Count];
            for (int i = 0; i < reductions.Length; i++)
                reductions[i] = new Dictionary<ushort, float>();
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Records the biomass reduction for a particular cohort.
        /// </summary>
        public static void RecordBiomassReduction(ICohort cohort,
                                                  float     reduction)
        {
            reductions[cohort.Species.Index][cohort.Age] = reduction;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Reduces the biomass of cohorts that have been marked for partial
        /// reduction.
        //  Original stand is the originating stand in cases where there is stand spreading.
        /// </summary>
        public static void ReduceCohortBiomass(ActiveSite site, Stand stand)
        {
            currentSite = site;
            originalStand = stand;
            numberCohortsReduced = 0;
            capacityReduction = 0.0;

            SiteVars.Cohorts[site].ReduceOrKillBiomassCohorts(singleton);

            //The function above will have gone through all the cohorts.  Now summarize
            //site level information.

            if (SiteVars.BiomassRemoved[site] > 0)
                BaseHarvest.SiteVars.Prescription[site] = originalStand.LastPrescription;

            if (SiteVars.BiomassRemoved[site] > 0 && BaseHarvest.SiteVars.CohortsDamaged[site] == 0)
                originalStand.LastAreaHarvested += PlugIn.ModelCore.CellArea;

            if (originalStand.LastPrescription.SpeciesToPlant != null)
                Landis.Library.Succession.Reproduction.ScheduleForPlanting(originalStand.LastPrescription.SpeciesToPlant, site);

            if (originalStand.LastPrescription.PreventEstablishment)
                SiteVars.CapacityReduction[site] = capacityReduction / (double)numberCohortsReduced;

            for (int i = 0; i < reductions.Length; i++)
                reductions[i].Clear();
        }
    }
}
