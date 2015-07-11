using Landis.Core;

namespace Landis.Extension.StressMortality
{
    /// <summary>
    /// An auxiliary parameter for species.
    /// </summary>
    public class SpeciesAuxParm<T>
    {
        private T[] values;

        //---------------------------------------------------------------------

        public T this[ISpecies species]
        {
            get
            {
                return values[species.Index];
            }

            set
            {
                values[species.Index] = value;
            }
        }

        //---------------------------------------------------------------------

        public SpeciesAuxParm(ISpeciesDataset species)
        {
            values = new T[species.Count];
        }
    }
}

