// Copyright 2008-2010 Green Code LLC, Portland State University
// Authors:  James B. Domingo, Robert M. Scheller,  
 

using BaseHarvest = Landis.Extension.BaseHarvest;

namespace Landis.Extension.BiomassHarvest
{
    /// <summary>
    /// The parameters for biomass harvest.
    /// </summary>
    public interface IParameters
        : BaseHarvest.IInputParameters
    {
        /// <summary>
        /// Template for pathnames for biomass-removed maps.
        /// </summary>
        string BiomassMapNames
        {
            get;
        }

    }
}
