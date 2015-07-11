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

        public FireRiskParameters this[int fuelTypeIndex]
        {
            get {
                return parameters[fuelTypeIndex];
            }

            set {
                parameters[fuelTypeIndex] = value;
            }
        }

        //---------------------------------------------------------------------

        public FireRiskTable()
        {
            parameters = new FireRiskParameters[150];  //up to 150 fuel types
            //foreach (FireRiskParameters fireRiskParm in parameters)
            //{
            //    fireRiskParm 
            //}
        }
    }
}
