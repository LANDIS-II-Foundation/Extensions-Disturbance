//  Copyright 2005 University of Wisconsin
//  Authors:  Jimm Domingo, Robert M. Scheller
//  License:  Available at  
//  http://landis.forest.wisc.edu/developers/LANDIS-IISourceCodeLicenseAgreement.pdf

using Edu.Wisc.Forest.Flel.Util;

namespace Landis.Wind
{
	/// <summary>
	/// Editable parameters (size and frequency) for wind events in an
	/// ecoregion.
	/// </summary>
	public interface IEditableEventParameters
		: IEditable<IEventParameters>
	{
		/// <summary>
		/// Maximum event size (hectares).
		/// </summary>
		InputValue<int> MaxSize
		{
			get;
			set;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Mean event size (hectares).
		/// </summary>
		InputValue<int> MeanSize
		{
			get;
			set;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Minimum event size (hectares).
		/// </summary>
		InputValue<int> MinSize
		{
			get;
			set;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Wind rotation period (years).
		/// </summary>
		InputValue<int> RotationPeriod
		{
			get;
			set;
		}
	}
}
