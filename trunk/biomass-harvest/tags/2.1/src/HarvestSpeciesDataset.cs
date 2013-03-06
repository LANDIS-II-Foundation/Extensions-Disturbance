// Copyright 2008 Green Code LLC
// Copyright 2010 Portland State University
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
//   Robert M. Scheller, Portland State University
 

using Edu.Wisc.Forest.Flel.Util;
using Landis.Core;

using System.Collections;
using System.Collections.Generic;

namespace Landis.Extension.BiomassHarvest
{
    // Wrapper around a species dataset so we can know what species was
    // most recently fetched from the dataset.
    public class HarvestSpeciesDataset
        : ISpeciesDataset
    {
        private static ISpecies mostRecentlyFetched;
        private ISpeciesDataset dataset;

        //---------------------------------------------------------------------

        public static ISpecies MostRecentlyFetchedSpecies
        {
            get {
                return mostRecentlyFetched;
            }
        }

        //---------------------------------------------------------------------

        public int Count
        {
            get {
                return dataset.Count;
            }
        }

        //---------------------------------------------------------------------

        public ISpecies this[int index]
        {
            get {
                mostRecentlyFetched = dataset[index];
                return mostRecentlyFetched;
            }
        }

        //---------------------------------------------------------------------

        public ISpecies this[string name]
        {
            get {
                mostRecentlyFetched = dataset[name];
                return mostRecentlyFetched;
            }
        }

        //---------------------------------------------------------------------

        public HarvestSpeciesDataset(ISpeciesDataset dataset)
        {
            Require.ArgumentNotNull(dataset);
            this.dataset = dataset;
        }

        //---------------------------------------------------------------------

        IEnumerator<ISpecies> IEnumerable<ISpecies>.GetEnumerator()
        {
            return ((IEnumerable<ISpecies>) dataset).GetEnumerator();
        }

        //---------------------------------------------------------------------

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) dataset).GetEnumerator();
        }
    }
}
