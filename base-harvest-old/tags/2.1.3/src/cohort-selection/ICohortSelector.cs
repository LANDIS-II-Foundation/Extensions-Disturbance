//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:  Robert M. Scheller, James B. Domingo

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
