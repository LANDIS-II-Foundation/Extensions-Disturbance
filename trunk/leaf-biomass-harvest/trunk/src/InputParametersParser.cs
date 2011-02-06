// Copyright 2008-2010 Green Code LLC, Portland State University
// Authors:  James B. Domingo, Robert M. Scheller, Srinivas S.

using Edu.Wisc.Forest.Flel.Util;
using Landis.Extension.BaseHarvest;
using Landis.Core;
using Landis.Library.Succession;

using System.Collections.Generic;
using System.Text;

using BaseHarvest = Landis.Extension.BaseHarvest;

namespace Landis.Extension.LeafBiomassHarvest
{
    /// <summary>
    /// A parser that reads the extension's parameters from text input.
    /// </summary>
    public class InputParametersParser
        : BaseHarvest.InputParametersParser
    {
        private static bool ageOrRangeWasRead = false;
        private static IList<ushort> ages;
        private static IList<AgeRange> ranges;
        private static IDictionary<ushort, Percentage> percentages;
        private static ISpecies currentSpecies;
        private static SpecificAgesCohortSelector[] ageSelectors;

        private ISpeciesDataset speciesDataset;
        private double standSpreadMinTargetSize;
        private double standSpreadMaxTargetSize;

        //---------------------------------------------------------------------

        static InputParametersParser()
        {
            // The base class's static ctor registers a read method for age
            // ranges.  Replace it with this project's read method that
            // handles percentages for partial thinning.
            InputValues.Register<AgeRange>(PartialThinning.ReadAgeOrRange);

            PartialThinning.ReadAgeOrRangeEvent += AgeOrRangeWasRead;

            ages = new List<ushort>();
            ranges = new List<AgeRange>();
            percentages = new Dictionary<ushort, Percentage>();
            currentSpecies = null;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="speciesDataset">
        /// The dataset of species to look up species' names in.  Important note:  the base harvest
        /// speciesDataset must be overwritten with the HarvestSpeciesDataset.  Methods within base harvest
        /// will set the MostRecentlyFetchedSpecies parameter when they are reading in species names 
        /// from a list of cohorts to be removed.  The value of MostRecentlyFetchedSpecies is not set within 
        /// biomass harvest.
        /// </param>
        public InputParametersParser(ISpeciesDataset speciesDataset)
                                //int              scenarioStart,
                                //int              scenarioEnd)
            : base(new HarvestSpeciesDataset(speciesDataset))
        {
            this.speciesDataset = speciesDataset;
            ageSelectors = new SpecificAgesCohortSelector[speciesDataset.Count];
        }

        //---------------------------------------------------------------------

        public static void AgeOrRangeWasRead(AgeRange   ageRange,
                                             Percentage percentage)
        {
            ageOrRangeWasRead = true;

            //  Have we started reading ages and ranges for another species?
            //  If so, then first create a cohort selector for the previous
            //  species.
            if (currentSpecies != HarvestSpeciesDataset.MostRecentlyFetchedSpecies) {
                if (currentSpecies != null)
                    ageSelectors[currentSpecies.Index] = new SpecificAgesCohortSelector(ages, ranges, percentages);
                currentSpecies = HarvestSpeciesDataset.MostRecentlyFetchedSpecies;
                ages.Clear();
                ranges.Clear();
                percentages.Clear();
            }

            if (ageRange.Start == ageRange.End)
                ages.Add(ageRange.Start);
            else
                ranges.Add(ageRange);
            if (percentage != null)
                percentages[ageRange.Start] = percentage;
        }

        //---------------------------------------------------------------------

        // Wraps a site selector with a class that handles partial cohort
        // thinning.
        protected ISiteSelector WrapSiteSelector(ISiteSelector siteSelector)
        {
            if (siteSelector is BaseHarvest.CompleteStandSpreading)
                return new CompleteStandSpreading(standSpreadMinTargetSize, standSpreadMaxTargetSize);
            else if (siteSelector is BaseHarvest.PartialStandSpreading)
                return new PartialStandSpreading(standSpreadMinTargetSize, standSpreadMaxTargetSize);
            else
                return new SiteSelectorWrapper(siteSelector);
        }

        //---------------------------------------------------------------------

        // Replaces all the instances of BaseHarvest.SpecificAgesCohortSelector
        // with biomass counterparts.
        protected void ReplaceSpecificAgeSelectors(ICohortSelector selector)
        {
            if (! ageOrRangeWasRead)
                return;

            ageOrRangeWasRead = false;
            MultiSpeciesCohortSelector multiSpeciesCohortSelector = (MultiSpeciesCohortSelector) selector;
            foreach (ISpecies species in speciesDataset) {
                SpecificAgesCohortSelector ageSelector = ageSelectors[species.Index];
                if (ageSelector != null) {
                    multiSpeciesCohortSelector[species] = ageSelector.SelectCohorts;
                    ageSelectors[species.Index] = null;
                }
            }
        }

        //---------------------------------------------------------------------

        // This following methods are copies of their counterparts in the
        // Base Harvest's parser because we need to do some extra processing
        // when reading prescriptions.  Since ReadPrescriptions isn't virtual,
        // we need a local copy of the Parse method which calls the local
        // copy of the ReadPrescriptions method.
        //
        // This following code is copyrighted by the University of Wisconsin.

        //---------------------------------------------------------------------

        protected static class Names
        {
            public const string CohortRemoval = "CohortsRemoved";
            public const string ForestTypeTable = "ForestTypeTable";
            public const string MultipleRepeat = "MultipleRepeat";
            public const string Plant = "Plant";
            public const string Prescription = "Prescription";
            public const string PrescriptionMaps = "PrescriptionMaps";
            public const string SingleRepeat = "SingleRepeat";
            public const string SiteSelection = "SiteSelection";
            public const string MinTimeSinceDamage = "MinTimeSinceDamage";
            public const string PreventEstablishment = "PreventEstablishment";
        }

        //---------------------------------------------------------------------

        // Since this class derives from Base Harvest's parser, then the Parse
        // method below must return Base Harvest's interface to the parameters.
        // But the method uses Biomass Harvest's class for editable parameters,
        // so that the new BiomassMaps parameter can be read and validated.
        // The method returns an instance of Biomass Harvest's Parameters, so
        // the caller must cast the reference to Biomass Harvest's parameters
        // interface in order to access the new BiomassMaps parameter.
        protected override BaseHarvest.IInputParameters Parse()
        {
            RoundedRepeatIntervals.Clear();
            // ReadLandisDataVar();

            InputVar<string> landisData = new InputVar<string>("LandisData");
            ReadVar(landisData);
            if (landisData.Value.Actual != PlugIn.PlugInName)
                throw new InputValueException(landisData.Value.String, "The value is not \"{0}\"", PlugIn.PlugInName);

            InputParameters parameters = new InputParameters();

            InputVar<int> timestep = new InputVar<int>("Timestep");
            ReadVar(timestep);
            parameters.Timestep = timestep.Value;

            InputVar<string> mgmtAreaMap = new InputVar<string>("ManagementAreas");
            ReadVar(mgmtAreaMap);
            parameters.ManagementAreaMap = mgmtAreaMap.Value;

            InputVar<string> standMap = new InputVar<string>("Stands");
            ReadVar(standMap);
            parameters.StandMap = standMap.Value;

            ReadPrescriptions(parameters.Prescriptions, timestep.Value.Actual);

            ReadHarvestImplementations(parameters.ManagementAreas, parameters.Prescriptions);


            //  Output file parameters

            InputVar<string> prescriptionMapNames = new InputVar<string>(Names.PrescriptionMaps);
            ReadVar(prescriptionMapNames);
            parameters.PrescriptionMapNames = prescriptionMapNames.Value;

            // TO DO: Probably should be required in the final release but made
            // it optional for now so that CBI doesn't have to update every
            // scenario in the short term.
            InputVar<string> biomassMapNames = new InputVar<string>("BiomassMaps");
            if (ReadOptionalVar(biomassMapNames))
                parameters.BiomassMapNames = biomassMapNames.Value;

            InputVar<string> eventLogFile = new InputVar<string>("EventLog");
            ReadVar(eventLogFile);
            parameters.EventLog = eventLogFile.Value;

            InputVar<string> summaryLogFile = new InputVar<string>("SummaryLog");
            ReadVar(summaryLogFile);
            parameters.SummaryLog = summaryLogFile.Value;

            CheckNoDataAfter("the " + eventLogFile.Name + " parameter");
            return parameters; 
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Reads 0 or more prescriptions from text input.
        /// </summary>
        new protected void ReadPrescriptions(List<Prescription> prescriptions,
                                             int                harvestTimestep)
        {
            Dictionary<string, int> lineNumbers = new Dictionary<string, int>();

            InputVar<int> singleRepeat = new InputVar<int>(Names.SingleRepeat);
            InputVar<int> multipleRepeat = new InputVar<int>(Names.MultipleRepeat);

            int nameLineNumber = LineNumber;
            InputVar<string> prescriptionName = new InputVar<string>(Names.Prescription);
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

                // Modify the ranking method with the forest-type table
                ReadForestTypeTable(rankingMethod);

                ISiteSelector siteSelector = ReadSiteSelector();
                bool isSiteSelectorWrapped = false;

                //get the minTimeSinceDamage
                int minTimeSinceDamage = 0;
                InputVar<int> minTimeSinceDamageVar = new InputVar<int>("MinTimeSinceDamage");
                if (ReadOptionalVar(minTimeSinceDamageVar))
                {
                    minTimeSinceDamage = minTimeSinceDamageVar.Value;
                }

                //get preventEstablishment
                bool preventEstablishment = false;
                if (ReadOptionalName(Names.PreventEstablishment))
                {
                    preventEstablishment = true;
                }

                ICohortSelector cohortSelector = ReadCohortSelector(false);
                if (ageOrRangeWasRead) {
                    siteSelector = WrapSiteSelector(siteSelector);
                    isSiteSelectorWrapped = true;
                    ReplaceSpecificAgeSelectors(cohortSelector);
                }

                Planting.SpeciesList speciesToPlant = ReadSpeciesToPlant();

                //  Repeat harvest?
                int repeatParamLineNumber = LineNumber;
                if (ReadOptionalVar(singleRepeat)) {
                    int interval = ValidateRepeatInterval(singleRepeat.Value,
                                                          repeatParamLineNumber,
                                                          harvestTimestep);
                    ICohortSelector additionalCohortSelector = ReadCohortSelector(true);
                    if (ageOrRangeWasRead) {
                        ReplaceSpecificAgeSelectors(cohortSelector);
                        if (! isSiteSelectorWrapped) {
                            siteSelector = WrapSiteSelector(siteSelector);
                            isSiteSelectorWrapped = true;
                        }
                    }
                    Planting.SpeciesList additionalSpeciesToPlant = ReadSpeciesToPlant();
                    prescriptions.Add(new SingleRepeatHarvest(name,
                                                              rankingMethod,
                                                              siteSelector,
                                                              cohortSelector,
                                                              speciesToPlant,
                                                              additionalCohortSelector,
                                                              additionalSpeciesToPlant,
                                                              minTimeSinceDamage,
                                                              preventEstablishment,
                                                              interval));
                }
                else if (ReadOptionalVar(multipleRepeat)) {
                    int interval = ValidateRepeatInterval(multipleRepeat.Value,
                                                          repeatParamLineNumber,
                                                          harvestTimestep);
                    prescriptions.Add(new RepeatHarvest(name,
                                                        rankingMethod,
                                                        siteSelector,
                                                        cohortSelector,
                                                        speciesToPlant,
                                                        minTimeSinceDamage,
                                                        preventEstablishment,
                                                        interval));
                }
                else {
                    prescriptions.Add(new Prescription(name,
                                                       rankingMethod,
                                                       siteSelector,
                                                       cohortSelector,
                                                       speciesToPlant,
                                                       minTimeSinceDamage,
                                                       preventEstablishment
                                                       ));
                }
            }
        }

        //---------------------------------------------------------------------

        protected static List<string> namesThatFollowForestType = new List<string>(
            new string[]{
                Names.SiteSelection,
                Names.CohortRemoval
            }
        );

        //----------------------------------------------------------------------

        //  Need to include a copy of this method because it modifies an
        //  instance member "rankingMethod" in the original version.  Bad
        //  design; the ranking method should be passed as a parameter.

        protected void ReadForestTypeTable(IStandRankingMethod rankingMethod)
        {
            // Can't initialize a private instance member in the base class.
            // Therefore, a parser instance is not re-usable.
            //speciesLineNumbers.Clear();  // in case parser re-used

            int optionalStatements = 0;

            //check if this is the ForestTypeTable
            if (CurrentName == Names.ForestTypeTable) {
                ReadName(Names.ForestTypeTable);

                //fresh input variables for table
                InputVar<string> inclusionRule = new InputVar<string>("Inclusion Rule");
                //InputVar<ushort> minAge = new InputVar<ushort>("Min Age");
                //InputVar<ushort> maxAge = new InputVar<ushort>("Max Age");
                InputVar<AgeRange> age_range = new InputVar<AgeRange>("Age Range", ParseAgeOrRange);
                InputVar<string> percentOfCells = new InputVar<string>("PercentOfCells");  //as a string so it can include keyword 'highest'
                InputVar<string> speciesName = new InputVar<string>("Species");


                //list for each rule- each line is a separate rule
                List<InclusionRule> rule_list = new List<InclusionRule>();
                //keep reading until no longer in the ForestTypeTable
                while (! AtEndOfInput && !namesThatFollowForestType.Contains(CurrentName)) {
                    StringReader currentLine = new StringReader(CurrentLine);

                    //  inclusionRule column
                    ReadValue(inclusionRule, currentLine);

                    //verify inclusion rule = 'optional', 'required', or 'forbidden'
                    if (inclusionRule.Value.Actual != "Optional" && inclusionRule.Value.Actual != "Required"
                                    && inclusionRule.Value.Actual != "Forbidden") {
                        string[] ic_list = new string[]{"Valid Inclusion Rules:",
                                                                           "    Optional",
                                                                           "    Required",
                                                                           "    Forbidden"};
                        throw new InputValueException(CurrentName, CurrentName + " is not a valid inclusion rule.",
                                                  new MultiLineText(ic_list));
                    }

                    if (inclusionRule.Value.Actual == "Optional")
                        optionalStatements++;


                    TextReader.SkipWhitespace(currentLine);
                    ReadValue(age_range, currentLine);

                    //percentage column
                    TextReader.SkipWhitespace(currentLine);
                    ReadValue(percentOfCells, currentLine);
                    //UI.WriteLine("percentOfCells = {0}", percentOfCells.Value.String);
                    //cannot validate until parsing is done.  will do this in the inclusionRule constructor

                    //a list in case there are multiple species on this line
                    List<string> species_list = new List<string>();
                    //add each species to this rule's species list
                    TextReader.SkipWhitespace(currentLine);
                    while (currentLine.Peek() != -1) {
                        //species column (build list)

                        ReadValue(speciesName, currentLine);
                        string name = speciesName.Value.String;

                        ISpecies species = GetSpecies(new InputValue<string>(name, speciesName.Value.String));
                        if (species_list.Contains(species.Name))
                            throw NewParseException("The species {0} appears more than once.", species.Name);
                        species_list.Add(species.Name);

                        //species_list.Add(species.Value.String);
                        TextReader.SkipWhitespace(currentLine);
                    }

                    //add this new inclusion rule (by parameters)  to the requirement
                    rule_list.Add(new InclusionRule(inclusionRule.Value.String,
                                                    age_range.Value.Actual,
                                                    percentOfCells.Value.String,
                                                    species_list));

                    GetNextLine();
                }
                //create a new requirement with this list of rules
                IRequirement inclusionRequirement = new InclusionRequirement(rule_list);
                //add this requirement to the ranking method
                rankingMethod.AddRequirement(inclusionRequirement);
            }

            if(optionalStatements > 0 && optionalStatements < 2)
                throw new InputValueException(CurrentName, "If there are optional statements, there must be more than one",
                                                  "ForestTypeTable");

        }

        //---------------------------------------------------------------------

        new protected ISiteSelector ReadSiteSelector()
        {
            InputVar<ISiteSelector> siteSelector = new InputVar<ISiteSelector>(Names.SiteSelection, ReadSiteSelector);
            ReadVar(siteSelector);


            return siteSelector.Value.Actual;
        }

        //---------------------------------------------------------------------

        private static class SiteSelection
        {
            //Names for each acceptable selection method
            public const string Complete                = "Complete";               //harvest whole stands
            public const string CompleteAndSpreading    = "CompleteStandSpread";    //spread by complete stand
            public const string TargetAndSpreading      = "PartialStandSpread";     //spread by site
            public const string Patch                   = "PatchCutting";           //make patches of specified size
        }

        //---------------------------------------------------------------------

        new protected InputValue<ISiteSelector> ReadSiteSelector(StringReader reader,
                                                                 out int      index)
        {
            TextReader.SkipWhitespace(reader);
            index = reader.Index;
            string name = TextReader.ReadWord(reader);
            if (name == "")
                throw new InputValueException();  // Missing value

            ISiteSelector selector;
            StringBuilder valueAsStr = new StringBuilder(name);
            //  Site selection -- Complete stand
            if (name == SiteSelection.Complete) {
                selector = new CompleteStand();
            }
            //  Site selection -- Target size with partial or complete spread
            else if (name == SiteSelection.CompleteAndSpreading || name == SiteSelection.TargetAndSpreading) {

                InputVar<double> minTargetSize = new InputVar<double>("the minimum target harvest size");
                ReadValue(minTargetSize, reader);

                InputVar<double> maxTargetSize = new InputVar<double>("the maximum target harvest size");
                ReadValue(maxTargetSize, reader);
                
                //validate the target size for spreading algorithms
                StandSpreading.ValidateTargetSize(minTargetSize.Value);
                standSpreadMinTargetSize = minTargetSize.Value;
                standSpreadMaxTargetSize = maxTargetSize.Value;

                if (name == SiteSelection.TargetAndSpreading) {
                    //  Site selection -- partial spread
                    selector = new PartialStandSpreading(minTargetSize.Value.Actual, maxTargetSize.Value.Actual);
                }
                else {
                    //  Site selection -- complete stand
                    selector = new CompleteStandSpreading(minTargetSize.Value.Actual, maxTargetSize.Value.Actual);
                }
                valueAsStr.AppendFormat(" {0} {1}", minTargetSize.Value.String, maxTargetSize.Value.String);

            }

            //  Site selection -- Patch cutting
            else if (name == SiteSelection.Patch) {
                InputVar<Percentage> percentage = new InputVar<Percentage>("the site percentage for patch cutting");
                ReadValue(percentage, reader);
                PatchCutting.ValidatePercentage(percentage.Value);

                InputVar<double> size = new InputVar<double>("the target patch size");
                ReadValue(size, reader);
                PatchCutting.ValidateSize(size.Value);

                selector = new PatchCutting(percentage.Value.Actual, size.Value.Actual);
                valueAsStr.AppendFormat(" {0} {1}", percentage.Value.String,
                                                    size.Value.String);
            }

            else {
                string[] methodList = new string[]{"Site selection methods:",
                                                   "  " + SiteSelection.Complete,
                                                   "  " + SiteSelection.CompleteAndSpreading,
                                                   "  " + SiteSelection.TargetAndSpreading,
                                                   "  " + SiteSelection.Patch};
                throw new InputValueException(name,
                                              name + " is not a valid site selection method",
                                              new MultiLineText(methodList));
            }
            return new InputValue<ISiteSelector>(selector, valueAsStr.ToString());
        }
    }
}
