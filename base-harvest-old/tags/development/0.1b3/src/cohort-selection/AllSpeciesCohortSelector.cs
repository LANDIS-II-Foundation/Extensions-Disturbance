// Copyright 2013 Green Code LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// Contributors:
//   James Domingo, Green Code LLC

using Landis.Library.AgeOnlyCohorts;
//using Landis.Core;

namespace Landis.Extension.BaseHarvest
{
    /// <summary>
    /// Removes certain cohorts of all species from a site.
    /// </summary>
    public class AllSpeciesCohortSelector
        : ICohortSelector
    {
        /// <summary>
        /// The cohort selection method applied to each species.
        /// </summary>
        public SelectCohorts.Method SelectionMethod { get; set; }

        //---------------------------------------------------------------------

    	/// <summary>
    	/// Selects which of a species' cohorts are harvested.
    	/// </summary>
    	public void Harvest(ISpeciesCohorts         cohorts,
                            ISpeciesCohortBoolArray isHarvested)
    	{
    	    SelectionMethod(cohorts, isHarvested);
    	}
    }
}
