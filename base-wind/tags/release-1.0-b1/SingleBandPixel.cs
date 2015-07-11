//  Author: Jimm Domingo

using Landis.Raster;
using System;

namespace Landis.Raster
{
	public class SingleBandPixel<T>
		: Landis.Raster.Pixel, IPixel
		where T : struct
	{
		private IPixelBandValue<T> band0;

		//---------------------------------------------------------------------

		public T Band0
		{
			get {
				return band0.Value;
			}
			set {
				band0.Value = value;
			}
		}

		//---------------------------------------------------------------------

		private void InitializeBands()
		{
			band0 = NewPixelBand(typeof(T)) as IPixelBandValue<T>;
			SetBands(band0);
		}

		//---------------------------------------------------------------------

		private IPixelBand NewPixelBand(Type bandType)
		{
			switch (Type.GetTypeCode(bandType)) {
				case TypeCode.Byte:
					return new PixelBandByte();

				case TypeCode.SByte:
					return new PixelBandSByte();

				case TypeCode.Int16:
					return new PixelBandShort();

				case TypeCode.UInt16:
					return new PixelBandUShort();

				case TypeCode.Int32:
					return new PixelBandInt();

				case TypeCode.UInt32:
					return new PixelBandUInt();

				case TypeCode.Single:
					return new PixelBandFloat();

				case TypeCode.Double:
					return new PixelBandDouble();

				default:
					return null;
			}
		}

		//---------------------------------------------------------------------

		public SingleBandPixel()
		{
			InitializeBands();
		}

		//---------------------------------------------------------------------

		public SingleBandPixel(T band0)
		{
			InitializeBands();
			Band0 = band0;
		}
	}
}
