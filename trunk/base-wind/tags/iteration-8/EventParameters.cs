namespace Landis.Wind
{
	public class EventParameters
	{
		public readonly double Probability;
		public readonly int    MaxSize;  // hectares
		public readonly int    MinSize;  // hectares
		public readonly int    MeanSize; // hectares

		//---------------------------------------------------------------------

		public EventParameters(double probability,
		                       int    maxSize,
		                       int    minSize,
		                       int    meanSize)
		{
			this.Probability = probability;
			this.MaxSize = maxSize;
			this.MinSize = minSize;
			this.MeanSize = meanSize;
		}
	}
}
