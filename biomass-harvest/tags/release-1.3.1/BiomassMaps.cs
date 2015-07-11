/*
 * Copyright 2008 Green Code LLC
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using Landis.Landscape;
using Landis.RasterIO;
using System;

using BaseHarvest = Landis.Harvest;

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
            using (IOutputRaster<BiomassPixel> map = CreateMap(path)) {
                BiomassPixel pixel = new BiomassPixel();
                foreach (Site site in Model.Core.Landscape.AllSites) {
                    pixel.Band0 = (ushort) Math.Round(SiteVars.BiomassRemoved[site] * 10.0);  //Convert to kg/ha
                    map.WritePixel(pixel);
                }
            }
        }

        //---------------------------------------------------------------------

        private IOutputRaster<BiomassPixel> CreateMap(string path)
        {
            UI.WriteLine("Writing biomass-removed map to {0} ...", path);
            return Model.Core.CreateRaster<BiomassPixel>(path,
                                                         Model.Core.Landscape.Dimensions,
                                                         Model.Core.LandscapeMapMetadata);
        }
    }
}
