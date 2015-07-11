//  Copyright 2005 University of Wisconsin
//  Authors:  Jimm Domingo, Robert M. Scheller
//  License:  Available at  
//  http://landis.forest.wisc.edu/developers/LANDIS-IISourceCodeLicenseAgreement.pdf

using Edu.Wisc.Forest.Flel.Util;

namespace Landis.Wind
{
	/// <summary>
	/// Editable parameters (size and frequency) for wind events for a
	/// collection of ecoregions.
	/// </summary>
	public class EventParameterDataset
		: IEditable<IEventParameters[]>
	{
		private IEditableEventParameters[] parameters;

		//---------------------------------------------------------------------

		/// <summary>
		/// The number of ecoregions in the dataset.
		/// </summary>
		public int Count
		{
			get {
				return parameters.Length;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The event parameters for an ecoregion.
		/// </summary>
		public IEditableEventParameters this[int ecoregionIndex]
		{
			get {
				return parameters[ecoregionIndex];
			}

			set {
				parameters[ecoregionIndex] = value;
			}
		}

		//---------------------------------------------------------------------

		public EventParameterDataset(int ecoregionCount)
		{
			parameters = new IEditableEventParameters[ecoregionCount];
		}

		//---------------------------------------------------------------------

		public bool IsComplete
		{
			get {
				foreach (IEditableEventParameters editableParms in parameters) {
					if (editableParms != null && !editableParms.IsComplete)
						return false;
				}
				return true;
			}
		}

		//---------------------------------------------------------------------

		public IEventParameters[] GetComplete()
		{
			if (IsComplete) {
				IEventParameters[] eventParms = new IEventParameters[parameters.Length];
				for (int i = 0; i < parameters.Length; i++) {
					IEditableEventParameters editableParms = parameters[i];
					if (editableParms != null)
						eventParms[i] = editableParms.GetComplete();
					else
						eventParms[i] = new EventParameters();
				}
				return eventParms;
			}
			else
				return null;
		}
	}
}
