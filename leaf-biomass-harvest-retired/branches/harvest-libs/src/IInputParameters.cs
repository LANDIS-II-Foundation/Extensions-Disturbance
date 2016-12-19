// Copyright 2008-2010 Green Code LLC, Portland State University
// Authors:  James B. Domingo, Robert M. Scheller, Srinivas S.

namespace Landis.Extension.LeafBiomassHarvest
{
    /// <summary>
    /// The parameters for biomass harvest.
    /// </summary>
    public interface IParameters
        : Landis.Library.HarvestManagement.IInputParameters
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
