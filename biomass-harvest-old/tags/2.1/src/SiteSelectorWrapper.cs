// Copyright 2008 Green Code LLC
// Copyright 2010 Portland State University
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// Contributors:
//   James Domingo, Green Code LLC
//   Robert M. Scheller, Portland State University

 
using Edu.Wisc.Forest.Flel.Util;
using Landis.SpatialModeling;
using Landis.Extension.BaseHarvest;

using System.Collections.Generic;

namespace Landis.Extension.BiomassHarvest
{
    // Wrapper around a site selector that invokes partial harvesting for
    // selected site.
    public class SiteSelectorWrapper
        : ISiteSelector
    {
        private ISiteSelector originalSelector;

        //---------------------------------------------------------------------

        public double AreaSelected
        {
            get {
                
                return originalSelector.AreaSelected;
            }
        }

        //---------------------------------------------------------------------

        public SiteSelectorWrapper(ISiteSelector siteSelector)
        {
            Require.ArgumentNotNull(siteSelector);
            this.originalSelector = siteSelector;
        }

        //---------------------------------------------------------------------

        public IEnumerable<ActiveSite> SelectSites(Stand stand)
        {
            //PlugIn.ModelCore.UI.WriteLine("Site Selector Wrapper");

            foreach (ActiveSite activeSite in originalSelector.SelectSites(stand)) {
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
