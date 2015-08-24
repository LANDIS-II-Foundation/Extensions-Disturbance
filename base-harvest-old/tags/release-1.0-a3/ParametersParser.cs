using Edu.Wisc.Forest.Flel.Util;
using System.Collections.Generic;

namespace Landis.Harvest
{
    /// <summary>
    /// A parser that reads harvest parameters from text input.
    /// </summary>
    public class ParametersParser
        : Landis.TextParser<IParameters>
    {
        private int startTime;
        private int endTime;
        private const string PrescriptionMaps = "PrescriptionMaps";

        //---------------------------------------------------------------------

        public override string LandisDataValue
        {
            get {
                return "Harvesting";
            }
        }

        //---------------------------------------------------------------------

        static ParametersParser()
        {
			// FIXME: Hack to ensure that Percentage is registered with InputValues
			Edu.Wisc.Forest.Flel.Util.Percentage p = new Edu.Wisc.Forest.Flel.Util.Percentage();
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="startTime">
        /// The year that the model scenario starts.
        /// </param>
        /// <param name="endTime">
        /// The year that the model scenario ends.
        /// </param>
        public ParametersParser(int startTime,
                                int endTime)
        {
            this.startTime = startTime;
            this.endTime = endTime;
        }

        //---------------------------------------------------------------------

        protected override IParameters Parse()
        {
            ReadLandisDataVar();

            EditableParameters parameters = new EditableParameters(/* args? */);

            InputVar<int> timestep = new InputVar<int>("Timestep");
            ReadVar(timestep);
            parameters.Timestep = timestep.Value;

            InputVar<string> mgmtAreaMap = new InputVar<string>("ManagementAreas");
            ReadVar(mgmtAreaMap);
            parameters.ManagementAreaMap = mgmtAreaMap.Value;

            InputVar<string> standMap = new InputVar<string>("Stands");
            ReadVar(standMap);
            parameters.StandMap = standMap.Value;

            ReadPrescriptions(parameters.Prescriptions);

            ReadHarvestImplementations(parameters.ManagementAreas,
                                       parameters.Prescriptions);

            //  Output file parameters

            InputVar<string> prescriptionMapNames = new InputVar<string>(PrescriptionMaps);
            ReadVar(prescriptionMapNames);
            parameters.PrescriptionMapNames = prescriptionMapNames.Value;

            InputVar<string> eventLogFile = new InputVar<string>("EventLog");
            ReadVar(eventLogFile);
            parameters.EventLog = eventLogFile.Value;

            CheckNoDataAfter("the " + eventLogFile.Name + " parameter");
            return parameters.GetComplete();
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Reads 0 or more prescriptions from text input.
        /// </summary>
        protected void ReadPrescriptions(List<Prescription> prescriptions)
        {
            Dictionary<string, int> lineNumbers = new Dictionary<string, int>();

            int nameLineNumber = LineNumber;
            InputVar<string> prescriptionName = new InputVar<string>("Prescription");
            while (ReadOptionalVar(prescriptionName)) {
                string name = prescriptionName.Value.Actual;
                int lineNumber;
                if (lineNumbers.TryGetValue(name, out lineNumber))
                    throw new InputValueException(prescriptionName.Value.String,
                                                  "The name {0} was previously used on line {1}",
                                                  prescriptionName.Value.String, lineNumber);
                else
                    lineNumbers[name] = nameLineNumber;

                IStandRankingMethod rankingMethod = ReadRankingMethod();
                ISiteSelector siteSelector = ReadSiteSelector();
                ICohortSelector cohortSelector = ReadCohortSelector();

                //int number = prescriptions.Count + 1;
                Prescription prescription = new Prescription(// number,
                                                             name,
                                                             rankingMethod,
                                                             siteSelector,
                                                             cohortSelector);
                prescriptions.Add(prescription);
            }
        }

        //---------------------------------------------------------------------

        protected IStandRankingMethod ReadRankingMethod()
        {
            InputVar<string> rankingName = new InputVar<string>("StandRanking");
            ReadVar(rankingName);

            if (rankingName.Value.Actual == "MaxCohortAge")
                return new MaxCohortAge();

            if ((rankingName.Value.Actual == "Economic") ||
                (rankingName.Value.Actual == "Random") ||
                (rankingName.Value.Actual == "RegulateAges") ||
                (rankingName.Value.Actual == "SpeciesBiomass") ||
                (rankingName.Value.Actual == "TotalBiomass")) {
                throw new InputValueException(rankingName.Value.String,
                                              rankingName.Value.String + " is not implemented yet");
            }

            string[] methodList = new string[]{"Stand ranking methods:",
                                               "  Economic",
                                               "  MaxCohortAge",
                                               "  Random",
                                               "  RegulateAges",
                                               "  SpeciesBiomass",
                                               "  TotalBiomass"};
            throw new InputValueException(rankingName.Value.String,
                                          rankingName.Value.String + " is not a valid stand ranking",
                                          new MultiLineText(methodList));
        }

        //---------------------------------------------------------------------

        protected ISiteSelector ReadSiteSelector()
        {
            if (ReadOptionalName("CompleteStand"))
                return new CompleteStand();

            InputVar<double> completeSpread = new InputVar<double>("Complete&Spread");
            ReadVar(completeSpread);
            StandSpreading.ValidateTargetSize(completeSpread.Value);
            return new CompleteStandSpreading(completeSpread.Value.Actual);
        }

        //---------------------------------------------------------------------

        protected ICohortSelector ReadCohortSelector()
        {
            InputVar<string> cohortSelection = new InputVar<string>("CohortsRemoved");
            ReadVar(cohortSelection);

            if (cohortSelection.Value.Actual == "ClearCut")
                return new ClearCut();

            if (cohortSelection.Value.Actual == "SpeciesList")
                throw new InputValueException(cohortSelection.Value.String,
                                              "SpeciesList is not implemented yet");

            throw new InputValueException(cohortSelection.Value.String,
                                          cohortSelection.Value.String + " is not a valid cohort selection",
                                          new MultiLineText("Valid values: ClearCut or SpeciesList"));
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Reads harvest implementations: which prescriptions are applied to
        /// which management areas.
        /// </summary>
        protected void ReadHarvestImplementations(ManagementAreaDataset mgmtAreas,
                                                  List<Prescription>    prescriptions)
        {
            ReadName("HarvestImplementations");

            InputVar<ushort> mgmtAreaMapCode = new InputVar<ushort>("Mgmt Area");
            InputVar<string> prescriptionName = new InputVar<string>("Prescription");
            InputVar<Percentage> areaToHarvest = new InputVar<Percentage>("Area To Harvest");
            while (! AtEndOfInput && CurrentName != PrescriptionMaps) {
                StringReader currentLine = new StringReader(CurrentLine);

                //  Mgmt Area column
                ReadValue(mgmtAreaMapCode, currentLine);
                ushort mapCode = mgmtAreaMapCode.Value.Actual;
                ManagementArea mgmtArea = mgmtAreas.Find(mapCode);
                if (mgmtArea == null) {
                    mgmtArea = new ManagementArea(mapCode);
                    mgmtAreas.Add(mgmtArea);
                }

                //  Prescription column
                ReadValue(prescriptionName, currentLine);
                string name = prescriptionName.Value.Actual;
                Prescription prescription = prescriptions.Find(new MatchName(name).Predicate);
                if (prescription == null)
                    throw new InputValueException(prescriptionName.Value.String,
                                                  prescriptionName.Value.String + " is an unknown prescription name");
                if (mgmtArea.IsApplied(prescription))
                    throw new InputValueException(prescriptionName.Value.String,
                                                  "Prescription {0} has already been applied to management area {1}",
                                                  prescriptionName.Value.String, mgmtArea.MapCode);

                //  Area to Harvest column
                ReadValue(areaToHarvest, currentLine);
                Percentage percentageToHarvest = areaToHarvest.Value.Actual;
                if (percentageToHarvest < 0 || percentageToHarvest > 1.0)
                    throw new InputValueException(areaToHarvest.Value.String,
                                                  "Percentage must be between 0% and 100%");

                mgmtArea.ApplyPrescription(prescription,
                                           percentageToHarvest,
                                           startTime,
                                           endTime);

                CheckNoDataAfter("the " + prescriptionName.Name + " column",
                                 currentLine);
                GetNextLine();
            }
        }

        //---------------------------------------------------------------------

        public class MatchName
        {
            private string name;

            //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

            public MatchName(string name)
            {
                this.name = name;
            }

            //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

            public bool Predicate(Prescription prescription)
            {
                return prescription.Name == name;
            }
        }
    }
}
