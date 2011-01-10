namespace Landis.Wind
{
	public class Parameters
		: Landis.ParameterSet
	{
		private int timestep;
		private string pathTemplate;
		private EventParameters[] eventParms;

		//---------------------------------------------------------------------

		/// <summary>
		/// Read values for parameters from a stream.
		/// </summary>
		/// <param name="stream"></param>
		/// <remarks>
		/// Sample input file:
		/// <code>
		/// //  Sample parameter file for Landis Wind component
		/// 
		/// Timestep  7   <-- years
		/// 
		/// Event-Parameters
		/// //  Ecoregion   WEP  Max Size   Mean Size   Min Size
		/// //  ---------   ---  --------   ---------   --------
		///     ecoregion1  .004    43        22           5
		/// //  ...
		/// 
		/// Cohort-Damage
		/// //  Mortality     Upper end
		/// //  Probability   Age Range
		/// //  -----------   ---------
		///         .20          0..20
		///         .50          50-70
		///         .95          95-100
		/// </code>
		/// </remarks>
		public override void ReadValues(Util.IStreamReader stream)
		{
			ParameterLine line = new ParameterLine(stream);
			try {
				line.GetNext();
				Parameter<int> timestep = new Parameter<int>("Timestep");
				line.ReadParm(timestep /* , ParameterOptions.Optional */ );
				this.Timestep = timestep;  // or timestep.Value

				line.GetNext();
				Parameter<string> windClassPath = new Parameter<string>(
															"Wind-class-path");
				line.ReadParm(windClassPath);
				this.pathTemplate = windClassPath;

				this.eventParms = new EventParameters[Model.Ecoregions.Count];

				line.GetNext();
				line.ReadName("Event-Parameters" /* , ParameterOptions.Optional */ );

				Parameter<string> ecoregionName = new Parameter<string>("Ecoregion");
				Parameter<float> eventProb = new Parameter<float>("Event Probability");
				Parameter<int> maxSize = new Parameter<int>("Max Event Size");
				Parameter<int> meanSize = new Parameter<int>("Mean Event Size");
				Parameter<int> minSize = new Parameter<int>("Min Event Size");

				string cohortDamage = "Cohort-Damage";
				while (line.GetNext() && line.ParmName != cohortDamage) {
					line.ReadParm(ecoregionName, ParameterOptions.ValueOnly);
					Ecoregion ecoregion = Model.Ecoregions[ecoregionName];
					if (ecoregion == null)
						throw ParameterError.New("Unknown ecoregion: {0}",
						                         ecoregionName.Value);
					if lineNums[ecoregion.Index] > 0
						throw ParameterError.New("Ecoregion {0} seen earlier on line {1}",
						                         ecoregion.Name, lineNums[ecoregion.Index]);
					lineNums[ecoregion.Index] = line.Number;

					EventParameters eventParms = new EventParameters();
					line.ReadParm(eventProb, ParameterOptions.ValueOnly);
					eventParms.Probability = eventProb;

					line.ReadParm(maxSize, ParameterOptions.ValueOnly);
					eventParms.MaxSize = maxSize;

					line.ReadParm(meanSize, ParameterOptions.ValueOnly);
					eventParms.MeanSize = meanSize;

					line.ReadParm(minSize, ParameterOptions.ValueOnly);
					eventParms.MinSize = minSize;

					this.eventParms[ecoregion.Index] = eventParms;
					line.EnsureNoExtraData();
				}  // while reading event parms

				line.ReadName(cohortDamage);

				this.mortalityParms = new List<MortalityParameters>();

				Parameter<float> mortProb = new Parameter<float>("Mortality Probability");
				Parameter<Percent> age = new Parameter<Percent>("Cohort Age");
				float prevUpperBound = 0;
				string prevUpperBoundAsStr = "";
				while (line.GetNext()) {
					MortalityParameters mortParms = new MortalityParameters();
					line.ReadParm(mortProb, ParameterOptions.ValueOnly);
					mortParms.Probability = mortProb;

					line.ReadParm(age, ParameterOptions.ValueOnly);
					mortParms.AgeRangeUpperBound = age.Value.AsDecimal;
					//  Age ranges must be in increasing order.
					if (mortParms.AgeRangeUpperBound <= prevUpperBound)
						throw new ParameterError.New("Age ranges must be in increasing order;",
						                             "upper bound {0} <= previous upper bound {1}",
						                             age.ToString(), prevUpperBoundAsStr);
					prevUpperBound = age.Value.AsDecimal;
					prevUpperBoundAsStr = age.ToString();

					this.mortalityParms.Add(mortParms);
					line.EnsureNoExtraData();
				}  // while reading event parms
			}
			catch (ParameterError err) {
				throw line.ErrorWithLastParmRead(err);
			}
		}

		//---------------------------------------------------------------------

		public int Timestep
		{
			get {
				return timestep;
			}
			set {
				//  Validate new value for Timestep parameter
				if (value < 1)
					throw new HissyFit();
				timestep = value;
			}
		}

		//---------------------------------------------------------------------

		public string PathTemplate
		{
			get {
				return pathTemplate;
			}
		}

		//---------------------------------------------------------------------

		public EventParameters[] EventParms
		{
			get {
				return eventParms;
			}
		}
	}
}
