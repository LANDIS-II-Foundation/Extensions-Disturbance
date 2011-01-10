namespace Landis.Core
{
	public static class Framework
	{
		private static object log;
		private static Raster.IRasterFactory rasterFactory;
		private static Landscape.Landscape landscape;
		private static Raster.Dimensions landscapeRasterDims;
		private static Util.Random.Uniform<float> randomNumGen;

		//---------------------------------------------------------------------

		public static object Log
		{
			get {
				return log;
			}
		}

		//---------------------------------------------------------------------

		public static Raster.IRasterFactory RasterFactory
		{
			get {
				return rasterFactory;
			}
		}

		//---------------------------------------------------------------------

		public static Landscape.Landscape Landscape
		{
			get {
				return landscape;
			}
		}

		//---------------------------------------------------------------------

		public static Raster.Dimensions LandscapeRasterDimensions
		{
			get {
				return landscapeRasterDims;
			}
		}

		//---------------------------------------------------------------------

		public static Util.Random.Uniform<float> RandomNumGenerator
		{
			get {
				return randomNumGen;
			}
		}
	}
}
