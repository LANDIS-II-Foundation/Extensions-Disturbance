using Landis.Landscape;
using System.Collections.Generic;

namespace Landis.Harvest
{
    /// <summary>
    /// A site-selection method.
    /// </summary>
    public interface ISiteSelector
    {
        /// <summary>
        /// Returns a collection of the sites selected from a stand and its
        /// neighbors.
        /// </summary>
        IEnumerable<ActiveSite> SelectSites(Stand stand);

        //---------------------------------------------------------------------

        /// <summary>
        /// The total area of the selected sites.
        /// </summary>
        double AreaSelected
        {
            get;
        }
    }
}
