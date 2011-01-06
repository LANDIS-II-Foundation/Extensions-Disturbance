// Copyright 2008-2010 Green Code LLC, Portland State University
// Authors:  James B. Domingo, Robert M. Scheller, Srinivas S.

using Wisc.Flel.GeospatialModeling.Landscapes;
using Wisc.Flel.GeospatialModeling.RasterIO;

using BaseHarvest = Landis.Extension.BaseHarvest;

namespace Landis.Extensions.LeafBiomassHarvest
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
            string path = BaseHarvest.MapNames.ReplaceTemplateVars(nameTemplate, timestep);
            using (IOutputRaster<BiomassPixel> map = CreateMap(path)) {
                BiomassPixel pixel = new BiomassPixel();
                foreach (Site site in PlugIn.ModelCore.Landscape.AllSites) {
                    pixel.Band0 = (ushort) (SiteVars.BiomassRemoved[site] / 10.0);
                    map.WritePixel(pixel);
                }
            }
        }

        //---------------------------------------------------------------------

        private IOutputRaster<BiomassPixel> CreateMap(string path)
        {
            PlugIn.ModelCore.Log.WriteLine("Writing biomass-removed map to {0} ...", path);
            return PlugIn.ModelCore.CreateRaster<BiomassPixel>(path,
                                                         PlugIn.ModelCore.Landscape.Dimensions,
                                                         PlugIn.ModelCore.LandscapeMapMetadata);
        }
    }
}
