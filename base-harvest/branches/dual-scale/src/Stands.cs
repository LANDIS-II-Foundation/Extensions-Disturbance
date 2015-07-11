using Landis.DualScale;
using System.Collections.Generic;
using Wisc.Flel.GeospatialModeling.Landscapes.DualScale;
using Wisc.Flel.GeospatialModeling.RasterIO;

namespace Landis.Harvest
{
    /// <summary>
    /// Utility methods for stands.
    /// </summary>
    public static class Stands
    {
        private static Dictionary<ushort, Stand> stands;

        //---------------------------------------------------------------------

        static Stands()
        {
            stands = new Dictionary<ushort, Stand>();
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Reads the input map of stands.
        /// </summary>
        /// <param name="path">
        /// Path to the map.
        /// </param>
        public static void ReadMap(string path)
        {
            IInputRaster<MapCodePixel> map = Model.Core.OpenRaster<MapCodePixel>(path);
            InputMap.ReadWithMajorityRule(map, Model.Core.Landscape, AssignSiteToStand);
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Assigns an active site to a particular stand.
        /// </summary>
        /// <param name="activeSite">
        /// The active site that is being assigned to a stand.
        /// </param>
        /// <param name="mapCode">
        /// The map code of the stand that the site is being assigned to.
        /// </param>
        public static void AssignSiteToStand(ActiveSite activeSite,
                                             ushort     mapCode)
        {
            double blockArea = (activeSite.SharesData ? Model.BlockArea : Model.Core.CellArea)
            
            //  Skip the site if its management area is not active.
            if (SiteVars.ManagementArea[activeSite] == null)
                return;

            Stand stand;
            //check if this stand is already in the dictionary
            if (stands.TryGetValue(mapCode, out stand)) {
                //if the stand is already in the dictionary, check if it is in the same management area.
                //if it's not in the same MA, throw exception.
                if (SiteVars.ManagementArea[activeSite] != stand.ManagementArea) {
                    throw new PixelException(activeSite.Location,
                                             "Stand {0} is in management areas {1} and {2}",
                                             stand.MapCode,
                                             stand.ManagementArea.MapCode,
                                             SiteVars.ManagementArea[activeSite].MapCode);
                }

            }
            //valid site location which has not been keyed by the dictionary.
            else {
                //assign stand (trygetvalue set it to null when it wasn't found in the dictionary)
                stand = new Stand(mapCode, blockArea);
                //add this stand to the correct management area (pointed to by the site)
                SiteVars.ManagementArea[activeSite].Add(stand);
                stands[mapCode] = stand;
            }                        

            //add this site to this stand
            stand.Add(activeSite);
        }
    }
}