using Landis.Core;

namespace Landis.Extension.StressMortality
{
    /// <summary>
    /// An auxiliary parameter for species.
    /// </summary>
    public class EcoregionsAuxParm<T>
    {
        private T[] values;

        //---------------------------------------------------------------------

        public T this[IEcoregion ecoregion]
        {
            get
            {
                return values[ecoregion.Index];
            }

            set
            {
                values[ecoregion.Index] = value;
            }
        }

        //---------------------------------------------------------------------

        public EcoregionsAuxParm(IEcoregionDataset ecoregions)
        {
            values = new T[ecoregions.Count];
        }
    }
}

