using Edu.Wisc.Forest.Flel.Util;

namespace Landis.Wind
{
	/// <summary>
	/// Editable definition of a wind severity.
	/// </summary>
	public class EditableSeverity
		: IEditableSeverity
	{
		private InputValue<byte> number;
		private InputValue<Percentage> minAge;
		private InputValue<Percentage> maxAge;
		private InputValue<float> mortalityProbability;

		//---------------------------------------------------------------------

		/// <summary>
		/// The severity's number (between 1 and 254).
		/// </summary>
		public InputValue<byte> Number
		{
			get {
				return number;
			}

			set {
				if (value != null) {
					if (value.Actual == 255)
						throw new InputValueException(value.String,
						                              "Value must be between 1 and 254.");
				}
				number = value;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The minimum value of the range of cohort ages (as % of species
		/// longevity) that the severity applies to.
		/// </summary>
		public InputValue<Percentage> MinAge
		{
			get {
				return minAge;
			}

			set {
				if (value != null) {
					ValidateAge(value);
					if (maxAge != null && value.Actual > maxAge.Actual)
						throw new InputValueException(value.String,
						                              "Value must be < or = MaxAge");
				}
				minAge = value;
			}
		}

		//---------------------------------------------------------------------

		private void ValidateAge(InputValue<Percentage> age)
		{
			if (age.Actual < 0.0 || age.Actual > 1.0)
				throw new InputValueException(age.String,
				                              "Value must be between 0% and 100%");
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The maximum value of the range of cohort ages (as % of species
		/// longevity) that the severity applies to.
		/// </summary>
		public InputValue<Percentage> MaxAge
		{
			get {
				return maxAge;
			}

			set {
				if (value != null) {
					ValidateAge(value);
					if (minAge != null && value.Actual < minAge.Actual)
						throw new InputValueException(value.String,
						                              "Value must be = or > MinAge");
				}
				maxAge = value;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The probability of cohort mortality due to wind.
		/// </summary>
		public InputValue<float> MortalityProbability
		{
			get {
				return mortalityProbability;
			}

			set {
				if (value != null) {
					if (value.Actual < 0.0 || value.Actual > 1.0)
						throw new InputValueException(value.String,
						                              "Value must be between 0.0 and 1.0");
				}
				mortalityProbability = value;
			}
		}

		//---------------------------------------------------------------------

		public EditableSeverity()
		{
		}

		//---------------------------------------------------------------------

		public bool IsComplete
		{
			get {
				foreach (object parameter in new object[]{ number,
				                                           minAge,
				                                           maxAge,
				                                           mortalityProbability }) {
					if (parameter == null)
						return false;
				}
				return true;
			}
		}

		//---------------------------------------------------------------------

		public ISeverity GetComplete()
		{
			if (IsComplete)
				return new Severity(number.Actual,
				                    minAge.Actual,
				                    maxAge.Actual,
				                    mortalityProbability.Actual);
			else
				return null;
		}
	}
}
