namespace Landis.Wind
{
	public class EventParameters
		: IEventParameters
	{
		private int maxSize;
		private int meanSize;
		private int minSize;
		private int rotationPeriod;

		//---------------------------------------------------------------------

		/// <summary>
		/// Maximum event size (hectares).
		/// </summary>
		public int MaxSize
		{
			get {
				return maxSize;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Mean event size (hectares).
		/// </summary>
		public int MeanSize
		{
			get {
				return meanSize;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Minimum event size (hectares).
		/// </summary>
		public int MinSize
		{
			get {
				return minSize;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Wind rotation period (years).
		/// </summary>
		public int RotationPeriod
		{
			get {
				return rotationPeriod;
			}
		}

		//---------------------------------------------------------------------

		public EventParameters(int maxSize,
		                       int meanSize,
		                       int minSize,
		                       int rotationPeriod)
		{
			this.maxSize = maxSize;
			this.meanSize = meanSize;
			this.minSize = minSize;
			this.rotationPeriod = rotationPeriod;
		}

		//---------------------------------------------------------------------

		public EventParameters()
		{
			this.maxSize = 0;
			this.meanSize = 0;
			this.minSize = 0;
			this.rotationPeriod = 0;
		}
	}
}
