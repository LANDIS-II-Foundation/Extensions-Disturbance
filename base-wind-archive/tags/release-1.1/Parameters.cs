//  Copyright 2005 University of Wisconsin
//  Authors:  Jimm Domingo, Robert M. Scheller
//  License:  Available at  
//  http://landis.forest.wisc.edu/developers/LANDIS-IISourceCodeLicenseAgreement.pdf

namespace Landis.Wind
{
	/// <summary>
	/// Parameters for the plug-in.
	/// </summary>
	public class Parameters
		: IParameters
	{
		private int timestep;
		private IEventParameters[] eventParameters;
		private ISeverity[] severities;
		private string mapNamesTemplate;
		private string logFileName;

		//---------------------------------------------------------------------

		/// <summary>
		/// Timestep (years)
		/// </summary>
		public int Timestep
		{
			get {
				return timestep;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Wind event parameters for each ecoregion.
		/// </summary>
		/// <remarks>
		/// Use Ecoregion.Index property to index this array.
		/// </remarks>
		public IEventParameters[] EventParameters
		{
			get {
				return eventParameters;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Definitions of wind severities.
		/// </summary>
		public ISeverity[] WindSeverities
		{
			get {
				return severities;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Template for the filenames for output maps.
		/// </summary>
		public string MapNamesTemplate
		{
			get {
				return mapNamesTemplate;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Name of log file.
		/// </summary>
		public string LogFileName
		{
			get {
				return logFileName;
			}
		}

		//---------------------------------------------------------------------

		public Parameters(int                timestep,
		                  IEventParameters[] eventParameters,
		                  ISeverity[]        severities,
		                  string             mapNameTemplate,
		                  string             logFileName)
		{
			this.timestep = timestep;
			this.eventParameters = eventParameters;
			this.severities = severities;
			this.mapNamesTemplate = mapNameTemplate;
			this.logFileName = logFileName;
		}
	}
}
