//  Copyright 2005-2009 University of Wisconsin, Portland State University
//  Authors:  Jimm Domingo, Robert M. Scheller
//  License:  Available at
//  http://www.landis-ii.org/developers/LANDIS-IISourceCodeLicenseAgreement.pdf

using Edu.Wisc.Forest.Flel.Util;
using System.Collections.Generic;

namespace Landis.Wind
{
	/// <summary>
	/// Parameters for the plug-in.
	/// </summary>
	public class InputParameters
		: IInputParameters
	{
		private int timestep;
		private IEventParameters[] eventParameters;
		private List<ISeverity> severities;
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
            set {
                if (value < 0)
                    throw new InputValueException(value.ToString(),
                                                      "Value must be = or > 0.");
                timestep = value;
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
		public List<ISeverity> WindSeverities
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
            set {
                MapNames.CheckTemplateVars(value);
                mapNamesTemplate = value;
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
            set {
                if (value == null)
                    throw new InputValueException(value.ToString(), "Value must be a file path.");
                logFileName = value;
            }
		}

        //---------------------------------------------------------------------

        public InputParameters(int ecoregionCount)
        {
            eventParameters = new IEventParameters[ecoregionCount];
            severities = new List<ISeverity>();
        }
/*		//---------------------------------------------------------------------

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
		}*/
	}
}
