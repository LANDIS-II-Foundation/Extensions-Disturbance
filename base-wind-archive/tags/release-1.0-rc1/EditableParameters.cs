using Edu.Wisc.Forest.Flel.Util;
using System.Collections.Generic;

namespace Landis.Wind
{
	/// <summary>
	/// An editable set of parameters for the plug-in.
	/// </summary>
	public class EditableParameters
		: IEditable<IParameters>
	{
		private InputValue<int> timestep;
		private EventParameterDataset eventParameters;
		private ListOfEditable<IEditableSeverity, ISeverity> severities;
		private InputValue<string> mapNamesTemplate;
		private InputValue<string> logFileName;

		//---------------------------------------------------------------------

		public InputValue<int> Timestep
		{
			get {
				return timestep;
			}

			set {
				if (value != null)
					if (value.Actual < 0)
						throw new InputValueException(value.String,
					                                  "Value must be = or > 0.");
				timestep = value;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Wind event parameters for each ecoregion.
		/// </summary>
		public EventParameterDataset EventParameters
		{
			get {
				return eventParameters;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Definitions of wind severities.
		/// </summary>
		public IList<IEditableSeverity> WindSeverities
		{
			get {
				return severities;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Template for pathnames for output maps.
		/// </summary>
		public InputValue<string> MapNamesTemplate
		{
			get {
				return mapNamesTemplate;
			}

			set {
				if (value != null) {
					MapNames.CheckTemplateVars(value.Actual);
				}
				mapNamesTemplate = value;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Name for the log file.
		/// </summary>
		public InputValue<string> LogFileName
		{
			get {
				return logFileName;
			}

			set {
				if (value != null) {
					// FIXME: check for null or empty path (value.Actual);
				}
				logFileName = value;
			}
		}

		//---------------------------------------------------------------------

		public EditableParameters(int ecoregionCount)
		{
			eventParameters = new EventParameterDataset(ecoregionCount);
			severities = new ListOfEditable<IEditableSeverity, ISeverity>();
		}

		//---------------------------------------------------------------------

		public bool IsComplete
		{
			get {
				foreach (object parameter in new object[]{ timestep,
				                                           mapNamesTemplate,
				                                           logFileName }) {
					if (parameter == null)
						return false;
				}
				return eventParameters.IsComplete &&
				       severities.IsEachItemComplete &&
				       severities.Count >= 1;
			}
		}

		//---------------------------------------------------------------------

		public IParameters GetComplete()
		{
			if (IsComplete)
				return new Parameters(timestep.Actual,
				                      eventParameters.GetComplete(),
				                      severities.GetComplete(),
				                      mapNamesTemplate.Actual,
				                      logFileName.Actual);
			else
				return null;
		}
	}
}
