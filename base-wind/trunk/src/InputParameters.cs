//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:    Robert M. Scheller, James B. Domingo

using Edu.Wisc.Forest.Flel.Util;
using System.Collections.Generic;

namespace Landis.Extension.BaseWind
{
	/// <summary>
	/// Parameters for the plug-in.
	/// </summary>
	public class InputParameters
		: IInputParameters
	{
		private int timestep;
        // ## Change ##
        private double lwRatioMean;
        private double lwRatioStDev;
        // ## Change ##
        private List<double> windDirPct;
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
        // ## Change ##
        /// <summary>
        /// Length to Width Ratio Mean
        /// </summary>
        public double LWRatioMean
        {
            get
            {
                return lwRatioMean;
            }
            set
            {
                if (value <= 0)
                    throw new InputValueException(value.ToString(),
                                                  "Value must be > 0.");
                lwRatioMean = value;
            }
        }
        //---------------------------------------------------------------------
        // ## Change ##
        /// <summary>
        /// Lenght to Width Ratio StDev
        /// </summary>
        public double LWRatioStDev
        {
            get
            {
                return lwRatioStDev;
            }
            set
            {
                lwRatioStDev = value;
            }
        }
        //---------------------------------------------------------------------
        // ## Change ##
        /// <summary>
        /// Wind direction Pct
        /// </summary>
        public List<double> WindDirPct
        {
            get
            {
                return windDirPct;
            }
            set
            {
                windDirPct = value;
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
            windDirPct = new List<double>(4);
        }
		//---------------------------------------------------------------------
	}
}
