// This file is part of the Base Harvest extension for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/trunk/base-harvest/trunk/

using Landis.Library.AgeOnlyCohorts;

namespace Landis.Extension.BaseHarvest
{
    /// <summary>
    /// Selects which cohorts at a site are removed by a harvest event.
    /// </summary>
    public interface ICohortSelector
    {
    	/// <summary>
    	/// Selects which of a species' cohorts are harvested.
    	/// </summary>
    	void Harvest(ISpeciesCohorts         cohorts,
                     ISpeciesCohortBoolArray isHarvested);
    }
}
