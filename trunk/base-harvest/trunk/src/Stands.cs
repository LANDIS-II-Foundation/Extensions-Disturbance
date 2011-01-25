//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:  Robert M. Scheller, James B. Domingo

using Landis.SpatialModeling;
using System.Collections.Generic;
using System.IO;

namespace Landis.Extension.BaseHarvest
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

            IInputRaster<UShortPixel> map;

            try
            {
                map = PlugIn.ModelCore.OpenRaster<UShortPixel>(path);
            }
            catch (FileNotFoundException)
            {
                string mesg = string.Format("Error: The file {0} does not exist", path);
                throw new System.ApplicationException(mesg);
            }

            if (map.Dimensions != PlugIn.ModelCore.Landscape.Dimensions)
            {
                string mesg = string.Format("Error: The input map {0} does not have the same dimension (row, column) as the ecoregions map", path);
                throw new System.ApplicationException(mesg);
            }

            using (map) {
                UShortPixel pixel = map.BufferPixel;
                foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                {
                    map.ReadBufferPixel();
                    ushort mapCode = pixel.MapCode.Value;
                    if (site.IsActive && SiteVars.ManagementArea[site] != null)
                    {
                        if (stands.TryGetValue(mapCode, out stand)) {
                            //if the stand is already in the dictionary, check if it is in the same management area.
                            //if it's not in the same MA, throw exception.
                            if (SiteVars.ManagementArea[site] != stand.ManagementArea) {
                                string mesg = string.Format("Stand {0} is in management areas {1} and {2}",
                                    stand.MapCode,
                                    stand.ManagementArea.MapCode,
                                    SiteVars.ManagementArea[site].MapCode);
                                throw new System.ApplicationException(mesg);
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

                /*using (map) {
            
                //loop through every single site in management area, assigning them to a stand.
                foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                {
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
                }*/
                
            }
        }
    }
}