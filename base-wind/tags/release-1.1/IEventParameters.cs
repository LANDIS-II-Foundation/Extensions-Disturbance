//  Copyright 2005 University of Wisconsin
//  Authors:  Jimm Domingo, Robert M. Scheller
//  License:  Available at  
//  http://landis.forest.wisc.edu/developers/LANDIS-IISourceCodeLicenseAgreement.pdf

namespace Landis.Wind
{
	/// <summary>
	/// Size and frequency parameters for wind events in an ecoregion.
	/// </summary>
	public interface IEventParameters
	{
		/// <summary>
		/// Maximum event size (hectares).
		/// </summary>
		int MaxSize
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Mean event size (hectares).
		/// </summary>
		int MeanSize
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Minimum event size (hectares).
		/// </summary>
		int MinSize
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Wind rotation period (years).
		/// </summary>
		int RotationPeriod
		{
			get;
		}
	}
}
