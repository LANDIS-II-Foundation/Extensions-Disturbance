// Copyright 2008-2010 Green Code LLC, Portland State University
// Authors:  James B. Domingo, Robert M. Scheller,  
 
using Landis.SpatialModeling;
using System;
using Landis.Extension.BaseHarvest;

namespace Landis.Extension.BiomassHarvest
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
            PlugIn.ModelCore.Log.WriteLine("   Writing biomass-removed map to {0} ...", path);
            using (IOutputRaster<UShortPixel> outputRaster = PlugIn.ModelCore.CreateRaster<UShortPixel>(path, PlugIn.ModelCore.Landscape.Dimensions))
            {
                UShortPixel pixel = outputRaster.BufferPixel;
                foreach (Site site in PlugIn.ModelCore.Landscape.AllSites) {
                    pixel.MapCode.Value = (ushort) Math.Round(SiteVars.BiomassRemoved[site] * 10.0);  //Convert to kg/ha
                    outputRaster.WriteBufferPixel();
                }
            }
        }

    }
}
