//  TODO: This file needs to be moved to Landis.Util project
namespace Landis.Util.Random
{
	public class Uniform<T>
	{
		public T NextValue {
			get {
				return default(T);
			}
		}

		//---------------------------------------------------------------------

		public Uniform(T lowBound,
		               T highBound,
		               Uniform<T> masterNumGen)
		{
			// TODO: implement
		}
	}
}
