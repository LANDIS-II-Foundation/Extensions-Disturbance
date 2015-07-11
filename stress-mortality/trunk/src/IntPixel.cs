﻿using Landis.SpatialModeling;

namespace Landis.Extension.StressMortality
{
    public class IntPixel : Pixel
    {
        public Band<int> MapCode = "The numeric code for each raster cell";

        public IntPixel()
        {
            SetBands(MapCode);
        }
    }
}
