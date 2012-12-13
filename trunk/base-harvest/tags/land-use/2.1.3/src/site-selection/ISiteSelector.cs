//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:  Robert M. Scheller, James B. Domingo

using Landis.SpatialModeling;
using System.Collections.Generic;

namespace Landis.Extension.BaseHarvest
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
