// Copyright 2008-2010 Green Code LLC, Portland State University
// Authors:  James B. Domingo, Robert M. Scheller, Srinivas S.

using Landis.Extension.BaseHarvest;
using Landis.SpatialModeling;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

using BaseHarvest = Landis.Extension.BaseHarvest;

namespace Landis.Extension.LeafBiomassHarvest
{
    // This class exists so that AppliedPrescription will access the base class
    // when it casts as StandSpreading.
    public class PartialStandSpreading
        : BaseHarvest.PartialStandSpreading, ISiteSelector
    {
        MethodInfo baseClassSelectSites;

        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="targetSize">
        /// The target size (area) to harvest.  Units: hectares.
        /// </param>
        public PartialStandSpreading(double minTargetSize, double maxTargetSize)
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
