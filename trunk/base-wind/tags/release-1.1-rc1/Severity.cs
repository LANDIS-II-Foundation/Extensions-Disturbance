//  Copyright 2005 University of Wisconsin
//  Authors:  Jimm Domingo, Robert M. Scheller
//  License:  Available at  
//  http://landis.forest.wisc.edu/developers/LANDIS-IISourceCodeLicenseAgreement.pdf

using Edu.Wisc.Forest.Flel.Util;

namespace Landis.Wind
{
	/// <summary>
	/// Definition of a wind severity.
	/// </summary>
	public class Severity
		: ISeverity
	{
		private byte number;
		private Range<double> ageRange;
		private float mortalityProbability;

		//---------------------------------------------------------------------

		/// <summary>
		/// The severity's number (between 1 and 254).
		/// </summary>
		public byte Number
		{
			get {
				return number;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The range of cohort ages (as % of species longevity) that the
		/// severity applies to.
		/// </summary>
		public Range<double> AgeRange
		{
			get {
				return ageRange;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The probability of cohort mortality due to wind.
		/// </summary>
		public float MortalityProbability
		{
			get {
				return mortalityProbability;
			}
		}

		//---------------------------------------------------------------------

		public Severity(byte   number,
		                double minAge,
		                double maxAge,
		                float  mortalityProbability)
		{
			this.number = number;
			this.ageRange = minAge < new Range<double>() <= maxAge;
			this.mortalityProbability = mortalityProbability;
		}
	}
}
