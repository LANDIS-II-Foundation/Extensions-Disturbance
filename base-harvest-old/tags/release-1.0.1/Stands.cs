using Landis.Landscape;
using Landis.RasterIO;

using System.Collections.Generic;

namespace Landis.Harvest
{
    /// <summary>
    /// Utility methods for stands.
    /// </summary>
    public static class Stands
    {
        /// <summary>
        /// Reads the input map of stands.
        /// </summary>
        /// <param name="path">
        /// Path to the map.
        /// </param>

        public static void ReadMap(string path) {
            Stand stand;
            Dictionary<ushort, Stand> stands = new Dictionary<ushort, Stand>();

            IInputRaster<MapCodePixel> map = Model.Core.OpenRaster<MapCodePixel>(path);
            using (map) {
            
                //loop through every single site in management area, assigning them to a stand.
                foreach (Site site in Model.Core.Landscape.AllSites) {
                    MapCodePixel pixel = map.ReadPixel();
                     //  Process the pixel only if the site is active and it's
                    //  in an active management area.
                    if (site.IsActive && SiteVars.ManagementArea[site] != null) {
                        ushort mapCode = pixel.Band0;
                        //check if this stand is already in the dictionary
                        if (stands.TryGetValue(mapCode, out stand)) {
                            //if the stand is already in the dictionary, check if it is in the same management area.
                            //if it's not in the same MA, throw exception.
                            if (SiteVars.ManagementArea[site] != stand.ManagementArea) {
                                throw new PixelException(site.Location,
                                "Stand {0} is in management areas {1} and {2}",
                                stand.MapCode,
                                stand.ManagementArea.MapCode,
                                SiteVars.ManagementArea[site].MapCode);
                            }
                        
                        }
                        //valid site location which has not been keyed by the dictionary.
                        else {
                            //assign stand (trygetvalue set it to null when it wasn't found in the dictionary)
                            stand = new Stand(mapCode);
                            //add this stand to the correct management area (pointed to by the site)
                            SiteVars.ManagementArea[site].Add(stand);
                            stands[mapCode] = stand;
                        }                        
                        //add this site to this stand
                        stand.Add((ActiveSite) site);
                    }
                }
                
            }
        }
    }
}