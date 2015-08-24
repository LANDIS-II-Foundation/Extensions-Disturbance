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

using Landis.Harvest;
using Landis.Landscape;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

using BaseHarvest = Landis.Harvest;

namespace Landis.Extension.BiomassHarvest
{
    // This class exists so that AppliedPrescription will access the base class
    // when it casts it as StandSpreading.
    public class CompleteStandSpreading
        : BaseHarvest.CompleteStandSpreading, ISiteSelector
    {
        MethodInfo baseClassSelectSites;

        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="targetSize">
        /// The target size (area) to harvest.  Units: hectares.
        /// </param>
        public CompleteStandSpreading(double minTargetSize, double maxTargetSize)
            : base(minTargetSize, maxTargetSize)
        {
            InterfaceMapping map = GetType().BaseType.GetInterfaceMap(typeof(ISiteSelector));
            for (int i = 0; i < map.InterfaceMethods.Length; i++) {
                if (map.InterfaceMethods[i].Name == "SelectSites") {
                    baseClassSelectSites = map.TargetMethods[i];
                    break;
                }
            }
            Debug.Assert(baseClassSelectSites != null);
        }

        //---------------------------------------------------------------------

        IEnumerable<ActiveSite> ISiteSelector.SelectSites(Stand stand)
        {
            IEnumerable<ActiveSite> selectedSites = (IEnumerable<ActiveSite>) baseClassSelectSites.Invoke(this, new object[] {stand});
            foreach (ActiveSite activeSite in selectedSites) {
                yield return activeSite;

                //  At this point, a prescription is done harvesting the
                //  site with age-only cohort selectors.  See if any
                //  specific-age cohort selectors have flagged some cohorts
                //  for partial thinning.
                PartialHarvestDisturbance.ReduceCohortBiomass(activeSite, stand);
            }
        }
    }
}
