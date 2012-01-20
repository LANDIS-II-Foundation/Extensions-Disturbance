//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:  Robert M. Scheller, James B. Domingo

using Landis.Core;

namespace Landis.Extension.BaseHarvest
{
    /// <summary>
    /// A collection of parameters for computing the economic ranks of species.
    /// </summary>
    public class FireRiskTable
    {
        private FireRiskParameters[] parameters;

        //---------------------------------------------------------------------

        public FireRiskParameters this[ISpecies species]
        {
            get {
                return parameters[species.Index];
            }

            set {
                parameters[species.Index] = value;
            }
        }

        //---------------------------------------------------------------------

        public FireRiskTable()
        {
            parameters = new FireRiskParameters[PlugIn.ModelCore.Species.Count];
        }
    }
}
