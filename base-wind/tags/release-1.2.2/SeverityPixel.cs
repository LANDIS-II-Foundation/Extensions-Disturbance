//  Copyright 2005 University of Wisconsin
//  Authors:  Jimm Domingo, Robert M. Scheller
//  License:  Available at  
//  http://landis.forest.wisc.edu/developers/LANDIS-IISourceCodeLicenseAgreement.pdf

using Landis.RasterIO;

namespace Landis.Wind
{
	public class SeverityPixel
		: SingleBandPixel<byte>
	{
		public SeverityPixel()
			: base()
		{
		}

		//---------------------------------------------------------------------

		public SeverityPixel(byte band0)
			: base(band0)
		{
		}
	}
}
