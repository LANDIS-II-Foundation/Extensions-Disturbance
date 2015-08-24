using Edu.Wisc.Forest.Flel.Util;
using Landis.Landscape;
using System.Collections.Generic;

namespace Landis.Harvest
{
    /// <summary>
    /// A site-selection method that harvests all the sites in a stand.
    /// </summary>
    public class CompleteStand
        : ISiteSelector
    {
        private double areaSelected;

        //---------------------------------------------------------------------

        public CompleteStand()
        {
        }

        //---------------------------------------------------------------------

        double ISiteSelector.AreaSelected
        {
            get {
                return areaSelected;
            }
        }

        //---------------------------------------------------------------------

        IEnumerable<ActiveSite> ISiteSelector.SelectSites(Stand stand)
        {
            areaSelected = stand.ActiveArea;
            stand.Harvested = true;
            return stand;
        }
    }
}
