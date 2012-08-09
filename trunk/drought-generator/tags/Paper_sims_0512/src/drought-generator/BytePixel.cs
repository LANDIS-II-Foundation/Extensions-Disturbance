﻿using Landis.SpatialModeling;

namespace Landis.Extension.DroughtGenerator
{
    public class BytePixel : Pixel
    {
        public Band<byte> MapCode = "The numeric code for each raster cell";

        public BytePixel()
        {
            SetBands(MapCode);
        }
    }
}