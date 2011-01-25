using Landis.Species;

namespace Landis.Harvest
{
    /// <summary>
    /// A collection of parameters for computing the economic ranks of species.
    /// </summary>
    public class EconomicRankTable
    {
        private EconomicRankParameters[] parameters;

        //---------------------------------------------------------------------

        public EconomicRankParameters this[ISpecies species]
        {
            get {
                return parameters[species.Index];
            }

            set {
                parameters[species.Index] = value;
            }
        }

        //---------------------------------------------------------------------

        public EconomicRankTable(IDataset speciesDataset)
        {
            parameters = new EconomicRankParameters[speciesDataset.Count];
        }
    }
}
