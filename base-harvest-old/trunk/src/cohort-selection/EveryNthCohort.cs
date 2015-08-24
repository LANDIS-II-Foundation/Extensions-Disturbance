// This file is part of the Base Harvest extension for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/trunk/base-harvest/trunk/

using Landis.Library.AgeOnlyCohorts;

namespace Landis.Extension.BaseHarvest
{
    /// <summary>
    /// Selects every Nth cohort among a species' cohorts for harvesting.
    /// </summary>
    /// <remarks>
    /// The cohorts are traversed from youngest to oldest.
    /// </remarks>
    public class EveryNthCohort
    {
        private int N;

        //---------------------------------------------------------------------

        public EveryNthCohort(int N)
        {
            this.N = N;
        }

        //---------------------------------------------------------------------

    	/// <summary>
    	/// Selects which of a species' cohorts are harvested.
    	/// </summary>
    	public void SelectCohorts(ISpeciesCohorts         cohorts,
                                  ISpeciesCohortBoolArray isHarvested)
    	{
    	    for (int i = isHarvested.Count - N; i >= 0; i -= N)
    	        isHarvested[i] = true;
    	}
    }
}
