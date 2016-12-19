// Copyright 2008-2010 Green Code LLC, Portland State University
// Authors:  James B. Domingo, Robert M. Scheller, Srinivas S.

using Landis.SpatialModeling;
using Landis.Library.HarvestManagement;

namespace Landis.Extension.LeafBiomassHarvest
{
    /// <summary>
    /// Utility class for writing biomass-removed maps.
    /// </summary>
    public class BiomassMaps
    {
        private string nameTemplate;

        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="nameTemplate">
        /// The template for the pathnames to the maps.
        /// </param>
        public BiomassMaps(string nameTemplate)
        {
            this.nameTemplate = nameTemplate;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Writes an output map of biomass removed from each active site.
        /// </summary>
        /// <param name="timestep">
        /// Timestep to use in the map's name.
        /// </param>
        public void WriteMap(int timestep)
        {
            string path = MapNames.ReplaceTemplateVars(nameTemplate, timestep);
            PlugIn.ModelCore.UI.WriteLine("   Writing biomass-removed map to {0} ...", path);
            using (IOutputRaster<IntPixel> outputRaster = PlugIn.ModelCore.CreateRaster<IntPixel>(path, PlugIn.ModelCore.Landscape.Dimensions))
            {
                IntPixel pixel = outputRaster.BufferPixel;
                foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                {
                    pixel.MapCode.Value = (SiteVars.BiomassRemoved[site]);
                    outputRaster.WriteBufferPixel();
                }
            }
        }

    }
}
