//  Copyright 2006-2010 USFS Portland State University, Northern Research Station, University of Wisconsin
//  Authors:  Robert M. Scheller, Brian R. Miranda 

using Landis.SpatialModeling;
using System.IO;


namespace Landis.Extension.DynamicFire
{
    internal static class Topography
    {
        //---------------------------------------------------------------------

        internal static void ReadGroundSlopeMap(string path)
        {
            IInputRaster<ShortPixel> map;
            try
            {
                map = PlugIn.ModelCore.OpenRaster<ShortPixel>(path);
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

            using (map)
            {
                ShortPixel pixel = map.BufferPixel;
                foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                {
                    map.ReadBufferPixel();
                    short mapCode = pixel.MapCode.Value;
                    if (site.IsActive)
                    {
                        if (mapCode < 0)
                        {
                            string mesg = string.Format("Ground Slope invalid map code: {0}", mapCode);
                            throw new System.ApplicationException(mesg);
                        }
                        SiteVars.GroundSlope[site] = (ushort) mapCode;
                    }
                }
            }
        }
        //---------------------------------------------------------------------

        internal static void ReadUphillSlopeAzimuthMap(string path)
        {
            IInputRaster<ShortPixel> map;

            try
            {
                map = PlugIn.ModelCore.OpenRaster<ShortPixel>(path);
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
                ShortPixel pixel = map.BufferPixel;
                foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                {
                    map.ReadBufferPixel();
                    short mapCode = pixel.MapCode.Value;
                    if (site.IsActive)
                    {
                        if (mapCode < 0 || mapCode > 360)
                        {
                            string mesg = string.Format("Uphill slope azimuth invalid map code (<0 or >360): {0}", mapCode);
                            throw new System.ApplicationException(mesg);
                        }
                        SiteVars.UphillSlopeAzimuth[site] = (ushort) mapCode;
                    }
                }
            }
        }

    }
}
