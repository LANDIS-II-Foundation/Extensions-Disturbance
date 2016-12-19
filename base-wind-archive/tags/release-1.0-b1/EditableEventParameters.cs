using Edu.Wisc.Forest.Flel.Util;

namespace Landis.Wind
{
	/// <summary>
	/// Editable parameters (size and frequency) for wind events in an
	/// ecoregion.
	/// </summary>
	public class EditableEventParameters
		: IEditableEventParameters
	{
		private InputValue<int> maxSize;
		private InputValue<int> meanSize;
		private InputValue<int> minSize;
		private InputValue<int> rotationPeriod;

		//---------------------------------------------------------------------

		/// <summary>
		/// Maximum event size (hectares).
		/// </summary>
		public InputValue<int> MaxSize
		{
			get {
				return maxSize;
			}

			set {
				if (value != null) {
					if (value.Actual < 0)
						throw new InputValueException(value.String,
						                              "Value must be = or > 0.");
					if (meanSize != null && value.Actual < meanSize.Actual)
						throw new InputValueException(value.String,
						                              "Value must be = or > MeanSize.");
					if (minSize != null && value.Actual < minSize.Actual)
						throw new InputValueException(value.String,
						                              "Value must be = or > MinSize.");
				}
				maxSize = value;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Mean event size (hectares).
		/// </summary>
		public InputValue<int> MeanSize
		{
			get {
				return meanSize;
			}

			set {
				if (value != null) {
					if (value.Actual < 0)
						throw new InputValueException(value.String,
						                              "Value must be = or > 0.");
					if (maxSize != null && value.Actual > maxSize.Actual)
						throw new InputValueException(value.String,
						                              "Value must be < or = MaxSize.");
					if (minSize != null && value.Actual < minSize.Actual)
						throw new InputValueException(value.String,
						                              "Value must be = or > MinSize.");
				}
				meanSize = value;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Minimum event size (hectares).
		/// </summary>
		public InputValue<int> MinSize
		{
			get {
				return minSize;
			}

			set {
				if (value != null) {
					if (value.Actual < 0)
						throw new InputValueException(value.String,
						                              "Value must be = or > 0.");
					if (meanSize != null && value.Actual > meanSize.Actual)
						throw new InputValueException(value.String,
						                              "Value must be < or = MeanSize.");
					if (maxSize != null && value.Actual > maxSize.Actual)
						throw new InputValueException(value.String,
						                              "Value must be < or = MaxSize.");
				}
				minSize = value;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Wind rotation period (years).
		/// </summary>
		public InputValue<int> RotationPeriod
		{
			get {
				return rotationPeriod;
			}

			set {
				if (value != null) {
					if (value.Actual < 0)
						throw new InputValueException(value.String,
						                              "Value must be = or > 0.");
				}
				rotationPeriod = value;
			}
		}

		//---------------------------------------------------------------------

		public EditableEventParameters()
		{
		}

		//---------------------------------------------------------------------

		public bool IsComplete
		{
			get {
				foreach (object parameter in new object[]{ maxSize,
				                                           meanSize,
				                                           minSize,
				                                           rotationPeriod }) {
					if (parameter == null)
						return false;
				}
				return true;
			}
		}

		//---------------------------------------------------------------------

		public IEventParameters GetComplete()
		{
			if (IsComplete)
				return new EventParameters(maxSize.Actual,
				                           meanSize.Actual,
				                           minSize.Actual,
				                           rotationPeriod.Actual);
			else
				return null;
		}
	}
}
