// This file is part of the Base Harvest extension for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/trunk/base-harvest/trunk/

using Landis.Library.AgeOnlyCohorts;
using Landis.Core;

using System.Collections.Generic;

namespace Landis.Extension.BaseHarvest
{
    /// <summary>
    /// Removes certain cohorts of one or more species from a site.
    /// </summary>
    public class MultiSpeciesCohortSelector
        : ICohortSelector
    {
        private Dictionary<ISpecies, SelectCohorts.Method> selectionMethods;

        //---------------------------------------------------------------------

        /// <summary>
        /// Gets or sets the selection method for a species.
        /// </summary>
        /// <remarks>
        /// When getting the selection method, if a species has none, null is
        /// returned.
        /// </remarks>
        public SelectCohorts.Method this[ISpecies species]
        {
            get {
                SelectCohorts.Method method;
                selectionMethods.TryGetValue(species, out method);
                return method;
            }

            set {
                selectionMethods[species] = value;
            }
        }

        //---------------------------------------------------------------------

        public MultiSpeciesCohortSelector()
        {
            selectionMethods = new Dictionary<ISpecies, SelectCohorts.Method>();
        }

        //---------------------------------------------------------------------

    	/// <summary>
    	/// Selects which of a species' cohorts are harvested.
    	/// </summary>
    	public void Harvest(ISpeciesCohorts         cohorts,
                            ISpeciesCohortBoolArray isHarvested)
    	{
    	    SelectCohorts.Method selectionMethod;
    	    if (selectionMethods.TryGetValue(cohorts.Species, out selectionMethod))
    	        selectionMethod(cohorts, isHarvested);
    	}
    }
}
