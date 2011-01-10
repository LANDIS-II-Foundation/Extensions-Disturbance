using Landis.Landscape;

namespace Landis.Succession
{
	public static class SiteVars
	{
		private static SiteVariable<ICohorts> cohortsVar;

		//---------------------------------------------------------------------

		public static SiteVariable<ICohorts> Cohorts
		{
			get {
				return cohortsVar;
			}
		}
	}
}
