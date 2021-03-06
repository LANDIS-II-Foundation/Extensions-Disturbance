using Landis.AgeCohort;
using Landis.Species;

using System.Collections.Generic;

namespace Landis.Harvest
{
    /// <summary>
    /// Selects specific ages and ranges of ages among a species' cohorts
    /// for harvesting.
    /// </summary>
    public class SpecificAgesCohortSelector
    {
        private List<ushort> ages;
        private List<AgeRange> ranges;

        //---------------------------------------------------------------------

        public SpecificAgesCohortSelector(List<ushort>   ages,
                                          List<AgeRange> ranges)
        {
            this.ages = new List<ushort>(ages);
            this.ranges = new List<AgeRange>(ranges);
        }

        //---------------------------------------------------------------------

    	/// <summary>
    	/// Selects which of a species' cohorts are harvested.
    	/// </summary>
    	public void SelectCohorts(ISpeciesCohorts         cohorts,
                                  ISpeciesCohortBoolArray isHarvested)
    	{
    	    // TODO
    	    throw new System.NotImplementedException();
    	}
    }
}
