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
using System.Collections.Generic;

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

        float[] IDisturbance.Damage(ICohort cohort)
        {
            float reduction;
            float[] leafWoodReduction = new float[2]{0F, 0F};
            
            if (reductions[cohort.Species.Index].TryGetValue(cohort.Age, out reduction)) {
            
                leafWoodReduction[0] = cohort.WoodBiomass / (cohort.LeafBiomass + cohort.WoodBiomass) * (float) reduction;
                leafWoodReduction[1] = cohort.LeafBiomass / (cohort.LeafBiomass + cohort.WoodBiomass) * (float) reduction;
                SiteVars.BiomassRemoved[currentSite] += (int) reduction;
                
                return leafWoodReduction;
            }
            else
                return leafWoodReduction;
        }

        //---------------------------------------------------------------------

        public static void Initialize()
        {
            reductions = new IDictionary<ushort, float>[Model.Core.Species.Count];
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
        /// </summary>
        public static void ReduceCohortBiomass(ActiveSite site)
        {
            currentSite = site;
            Model.LandscapeCohorts[site].DamageBy(singleton);
            for (int i = 0; i < reductions.Length; i++)
                reductions[i].Clear();
        }
    }
}
