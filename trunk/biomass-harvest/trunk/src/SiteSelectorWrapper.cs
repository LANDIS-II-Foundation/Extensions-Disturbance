// Copyright 2008-2010 Green Code LLC, Portland State University
// Authors:  James B. Domingo, Robert M. Scheller,  
 
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
            foreach (ActiveSite activeSite in originalSelector.SelectSites(stand)) {
                
                //  At this point, a prescription is done harvesting the
                //  site with age-only cohort selectors.  See if any
                //  specific-age cohort selectors have flagged some cohorts
                //  for partial thinning.
                PartialHarvestDisturbance.ReduceCohortBiomass(activeSite, stand);
                
                //if(BaseHarvest.SiteVars.CohortsDamaged[currentSite] <= 0)  // don't double count
                //Landis.Harvest.SiteVars.Stand[activeSite].LastAreaHarvested += PlugIn.ModelCore.CellArea;
                
                
                yield return activeSite;

            }
        }
    }
}
