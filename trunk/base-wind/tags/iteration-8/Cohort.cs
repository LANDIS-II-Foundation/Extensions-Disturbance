namespace Landis.Succession
{
	public class Cohort
	{
		public delegate bool FilterMethod(ISpecies species,
		                                  int      cohortAge);
	}
}
