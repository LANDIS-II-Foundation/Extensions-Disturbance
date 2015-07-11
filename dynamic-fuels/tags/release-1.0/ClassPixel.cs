//  Copyright 2007-2008 USFS Northern Research Station, Conservation Biology Institute, University of Wisconsin
//  Authors:  
//      Robert M. Scheller
//      Brian R. Miranda
//  License:  Available at  
//  http://www.landis-ii.org/developers/LANDIS-IISourceCodeLicenseAgreement.pdf

using Landis.RasterIO;

namespace Landis.Fuels
{
    public class ClassPixel
        : RasterIO.SingleBandPixel<byte>
    {
        //---------------------------------------------------------------------

        public ClassPixel()
            : base()
        {
        }

        //---------------------------------------------------------------------

        public ClassPixel(byte band0)
            : base(band0)
        {
        }
    }
}
