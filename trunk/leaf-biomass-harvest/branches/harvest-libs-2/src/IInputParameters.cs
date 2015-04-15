// Copyright 2008-2010 Green Code LLC, Portland State University
// Authors:  James B. Domingo, Robert M. Scheller, Srinivas S.

using BaseHarvest = Landis.Extension.BaseHarvest;

namespace Landis.Extension.LeafBiomassHarvest
{
    /// <summary>
    /// The parameters for biomass harvest.
    /// </summary>
    public interface IInputParameters
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
