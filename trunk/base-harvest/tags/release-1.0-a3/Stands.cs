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
        public static void ReadMap(string path)
        {
            Dictionary<ushort, Stand> stands = new Dictionary<ushort, Stand>();

            IInputRaster<MapCodePixel> map = Model.Core.OpenRaster<MapCodePixel>(path);
            using (map) {
                // TODO: make sure its dimensions match landscape's dimensions
                foreach (Site site in Model.Core.Landscape.AllSites) {
                    MapCodePixel pixel = map.ReadPixel();
                    //  Process the pixel only if the site is active and it's
                    //  in an active management area.
                    if (site.IsActive && SiteVars.ManagementArea[site] != null) {
                        ushort mapCode = pixel.Band0;
                        Stand stand;
                        if (stands.TryGetValue(mapCode, out stand)) {
                            if (SiteVars.ManagementArea[site] != stand.ManagementArea)
                                throw new PixelException(site.Location,
                                                         "Stand {0} is in management areas {1} and {2}",
                                                         stand.MapCode,
                                                         stand.ManagementArea.MapCode,
                                                         SiteVars.ManagementArea[site].MapCode);
                        }
                        else {
                            stand = new Stand(mapCode);
                            SiteVars.ManagementArea[site].Add(stand);
                            stands[mapCode] = stand;
                        }
                        stand.Add((ActiveSite) site);
                    }
                }
            }
        }
    }
}
