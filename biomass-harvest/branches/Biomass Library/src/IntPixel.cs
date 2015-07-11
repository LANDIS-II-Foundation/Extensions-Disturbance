//  Copyright 2005 University of Wisconsin
//  Copyright 2010 Portland State University
//
//  Contributors:
//    James B. Domingo, University of Wisconsin
//    Robert M. Scheller, Portland State University

using Landis.SpatialModeling;

namespace Landis.Extension.BiomassHarvest
{
    public class IntPixel : Pixel
    {
        public Band<int> MapCode  = "The numeric code for each raster cell";

        public IntPixel()
        {
            SetBands(MapCode);
        }
    }
}
