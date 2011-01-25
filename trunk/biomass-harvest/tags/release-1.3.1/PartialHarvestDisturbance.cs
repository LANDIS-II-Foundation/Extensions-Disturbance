/*
 * Copyright 2008 Green Code LLC
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using Landis.Biomass;
using Landis.Landscape;
using Landis.PlugIns;
using Landis.Harvest;
using System.Collections.Generic;

using BaseHarvest = Landis.Harvest;


namespace Landis.Extension.BiomassHarvest
{
    /// <summary>
    /// A biomass disturbance that handles partial thinning of cohorts.
    /// </summary>
    public class PartialHarvestDisturbance
        : IDisturbance
    {
        private static PartialHarvestDisturbance singleton;
        private static IDictionary<ushort, int>[] reductions;

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

        PlugInType IDisturbance.Type
        {
            get {
                return PlugIn.Type;
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

        int IDisturbance.Damage(ICohort cohort)
        {
            int reduction;
            if (reductions[cohort.Species.Index].TryGetValue(cohort.Age, out reduction))
            {

                //UI.WriteLine("Removing:  {0:0.0}/{1:0.0}.", reduction, cohort.Biomass);

                SiteVars.BiomassRemoved[currentSite] += reduction;

                SiteVars.CohortsPartiallyDamaged[currentSite]++;

                if (originalStand.LastPrescription.PreventEstablishment)
                {
                    numberCohortsReduced++;
                    capacityReduction += (double) reduction / (double) cohort.Biomass;
                }

                // Record any cohort touched, not just killed:
                BaseHarvest.SiteVars.Stand[currentSite].UpdateDamageTable(cohort.Species.Name);

                return reduction;
            }
            else
                return 0;
        }

        //---------------------------------------------------------------------

        public static void Initialize()
        {
            reductions = new IDictionary<ushort, int>[Model.Core.Species.Count];
            for (int i = 0; i < reductions.Length; i++)
                reductions[i] = new Dictionary<ushort, int>();
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Records the biomass reduction for a particular cohort.
        /// </summary>
        public static void RecordBiomassReduction(ICohort cohort,
                                                  int     reduction)
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

            Model.LandscapeCohorts[site].DamageBy(singleton);

            //The function above will have gone through all the cohorts.  Now summarize
            //site level information.

            if(SiteVars.BiomassRemoved[site] > 0)
                BaseHarvest.SiteVars.Prescription[site] = originalStand.LastPrescription;

            if(SiteVars.BiomassRemoved[site] > 0 && BaseHarvest.SiteVars.CohortsDamaged[site] == 0)
                originalStand.LastAreaHarvested += Model.Core.CellArea;

            if (originalStand.LastPrescription.SpeciesToPlant != null)
                Succession.Reproduction.ScheduleForPlanting(originalStand.LastPrescription.SpeciesToPlant, site);

            if (originalStand.LastPrescription.PreventEstablishment)
                SiteVars.CapacityReduction[site] = capacityReduction / (double) numberCohortsReduced;

            for (int i = 0; i < reductions.Length; i++)
                reductions[i].Clear();
        }
    }
}
