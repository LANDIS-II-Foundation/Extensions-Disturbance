using Edu.Wisc.Forest.Flel.Util;

namespace Landis.Wind
{
	/// <summary>
	/// Definition of a wind severity.
	/// </summary>
	public interface ISeverity
	{
		/// <summary>
		/// The range of cohort ages (as % of species longevity) that the
		/// severity applies to.
		/// </summary>
		Range<double> AgeRange
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The probability of cohort mortality due to wind.
		/// </summary>
		float MortalityProbability
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The severity's number (between 1 and 254).
		/// </summary>
		byte Number
		{
			get;
		}
	}
}
