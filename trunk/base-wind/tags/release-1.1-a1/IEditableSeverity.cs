//  Copyright 2005 University of Wisconsin
//  Authors:  Jimm Domingo, Robert M. Scheller
//  License:  Available at  
//  http://landis.forest.wisc.edu/developers/LANDIS-IISourceCodeLicenseAgreement.pdf

using Edu.Wisc.Forest.Flel.Util;

namespace Landis.Wind
{
	/// <summary>
	/// Editable definition of a wind severity.
	/// </summary>
	public interface IEditableSeverity
		: IEditable<ISeverity>
	{
		/// <summary>
		/// The severity's number (between 1 and 254).
		/// </summary>
		InputValue<byte> Number
		{
			get;
			set;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The minimum value of the range of cohort ages (as % of species
		/// longevity) that the severity applies to.
		/// </summary>
		InputValue<Percentage> MinAge
		{
			get;
			set;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The maximum value of the range of cohort ages (as % of species
		/// longevity) that the severity applies to.
		/// </summary>
		InputValue<Percentage> MaxAge
		{
			get;
			set;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The probability of cohort mortality due to wind.
		/// </summary>
		InputValue<float> MortalityProbability
		{
			get;
			set;
		}
	}
}
