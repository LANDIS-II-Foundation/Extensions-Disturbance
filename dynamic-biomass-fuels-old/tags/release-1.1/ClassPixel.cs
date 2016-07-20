//  Copyright 2006 University of Wisconsin-Madison
//  Authors:  Robert M. Scheller
//  License:  Available at  
//  http://www.landis-ii.org/developers/LANDIS-IISourceCodeLicenseAgreement.pdf

using Landis.RasterIO;

namespace Landis.Extension.Fuels
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
