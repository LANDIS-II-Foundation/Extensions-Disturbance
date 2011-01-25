// Copyright 2008-2010 Green Code LLC, Portland State University
// Authors:  James B. Domingo, Robert M. Scheller,  
 

using Edu.Wisc.Forest.Flel.Util;
using Landis.Core;

using System.Collections;
using System.Collections.Generic;

namespace Landis.Extension.BiomassHarvest
{
    // Wrapper around a species dataset so we can know what species was
    // most recently fetched from the dataset.
    public class SpeciesDataset
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

        public SpeciesDataset(ISpeciesDataset dataset)
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
