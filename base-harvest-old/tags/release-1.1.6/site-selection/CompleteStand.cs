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
        //mark the whole area selected as harvested
        IEnumerable<ActiveSite> ISiteSelector.SelectSites(Stand stand)
        {
            areaSelected = stand.ActiveArea;
            stand.MarkAsHarvested();
			//mark this stand's event id
			stand.EventId = PlugIn.EventId;
			
			//increment global event id number
			PlugIn.EventId++;
			
            return stand;
        }
		
    }
}
