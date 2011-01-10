namespace Landis
{
	public static class Model
	{
		private static object log;
		private static Ecoregion[] ecoregions;
		private static Raster.IRasterFactory rasterFactory;
		private static Landscape.Landscape landscape;
		private static Raster.Dimensions landscapeRasterDims;
		private static Util.Random.Uniform<float> randomNumGen;
		private static float cellSizeHectares;

		//---------------------------------------------------------------------

		public static object Log
		{
			get {
				return log;
			}
		}

		//---------------------------------------------------------------------

		public static Ecoregion[] Ecoregions
		{
			get {
				return ecoregions;
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

		//---------------------------------------------------------------------

		public static float CellSizeHectares
		{
			get {
				return cellSizeHectares;
			}
		}

		//---------------------------------------------------------------------

		public static class SiteVars
		{
			private static Landscape.SiteVariable<Ecoregion> ecoregionVar;
			private static Landscape.SiteVariable<bool> disturbedVar;

			//-----------------------------------------------------------------

			public static Landscape.SiteVariable<Ecoregion> Ecoregion
			{
				get {
					return ecoregionVar;
				}
			}
			//-----------------------------------------------------------------

			public static Landscape.SiteVariable<bool> Disturbed
			{
				get {
					return disturbedVar;
				}
			}
		}
	}
}
