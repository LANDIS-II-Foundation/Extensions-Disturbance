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
