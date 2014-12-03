// Copyright 2008 Green Code LLC
// Copyright 2010 Portland State University
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// Contributors:
//   James Domingo, Green Code LLC
//   Robert M. Scheller, Portland State University
 

using Edu.Wisc.Forest.Flel.Util;
using Landis.Core;
using Landis.Library.BiomassHarvest;
using Landis.Library.HarvestManagement;
using Landis.Library.SiteHarvest;
using Landis.Library.Succession;
using System.Collections.Generic;
using System.Text;


namespace Landis.Extension.BiomassHarvest
{
    /// <summary>
    /// A parser that reads the extension's parameters from text input.
    /// </summary>
    public class ParametersParser
        : InputParametersParser
    {
        public override string LandisDataValue
        {
            get
            {
                return PlugIn.ExtensionName;
            }
        }

        //---------------------------------------------------------------------

        static ParametersParser()
        {
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="speciesDataset">
        /// The dataset of species to look up species' names in.
        /// </param>
        public ParametersParser(ISpeciesDataset speciesDataset)
            : base(PlugIn.ExtensionName, speciesDataset)
        {
        }

        //---------------------------------------------------------------------

        protected override Landis.Library.HarvestManagement.InputParameters CreateEmptyParameters()
        {
            return new Parameters();
        }

        //---------------------------------------------------------------------

        protected override ICohortCutter CreateCohortCutter(ICohortSelector cohortSelector)
        {
            return CohortCutterFactory.CreateCutter(cohortSelector, HarvestExtensionMain.ExtType);
        }
 
        //---------------------------------------------------------------------

        /// <summary>
        /// Creates a cohort selection method for a specific set of ages and
        /// age ranges.
        /// </summary>
        /// <remarks>
        /// This overrides the base method so it can use the PartialThinning
        /// class to handle cohort selections with percentages.
        /// </remarks>
        protected override void CreateCohortSelectionMethodFor(ISpecies species,
                                                               IList<ushort> ages,
                                                               IList<AgeRange> ranges)
        {
            if (!PartialThinning.CreateCohortSelectorFor(species, ages, ranges))
            {
                // There were no percentages specified for this species' ages
                // and ranges.  So just create and store a whole cohort
                // selector using the base method.
                base.CreateCohortSelectionMethodFor(species, ages, ranges);
            }
        }

        //---------------------------------------------------------------------

        protected override void ReadBiomassMaps()
        {
            // TO DO: Probably should be required in the final release but made
            // it optional for now so that CBI doesn't have to update every
            // scenario in the short term.
            InputVar<string> biomassMapNames = new InputVar<string>("BiomassMaps");
            string foo;
            if (ReadOptionalVar(biomassMapNames))
                foo /*parameters.BiomassMapNames*/ = biomassMapNames.Value;
        }

        //---------------------------------------------------------------------

#if DISABLE
        public static void AgeOrRangeWasRead(AgeRange ageRange,
                                             Percentage percentage)
        {
            ageOrRangeWasRead = true;
            //  Have we started reading ages and ranges for another species?
            //  If so, then first clear the old values from the previous
            //  species.
            if (ageSelectors[HarvestSpeciesDataset.MostRecentlyFetchedSpecies.Index] == null)
            {
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

            if (HarvestSpeciesDataset.MostRecentlyFetchedSpecies != null)
                ageSelectors[HarvestSpeciesDataset.MostRecentlyFetchedSpecies.Index] = new SpecificAgesCohortSelector(ages, ranges, percentages);

            currentSpecies = HarvestSpeciesDataset.MostRecentlyFetchedSpecies;

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
        protected MultiSpeciesCohortSelector ReplaceSpecificAgeSelectors(ICohortSelector selector)
        {
            if (! ageOrRangeWasRead)
                return null;

            ageOrRangeWasRead = false;
            MultiSpeciesCohortSelector multiSpeciesCohortSelector = (MultiSpeciesCohortSelector) selector;
            foreach (ISpecies species in speciesDataset) {
                //PlugIn.ModelCore.UI.WriteLine("ReplaceSpecificAgeSelectors:  spp={0}", species.Name);
                SpecificAgesCohortSelector ageSelector = ageSelectors[species.Index];
                if (ageSelector != null) {
                    multiSpeciesCohortSelector[species] = ageSelector.SelectCohorts; 
                    ageSelectors[species.Index] = null;
                }
            }
            return multiSpeciesCohortSelector;
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
            public const string HarvestImplementations = "HarvestImplementations";
            public const string CohortRemoval = "CohortsRemoved";
            public const string ForestTypeTable = "ForestTypeTable";
            public const string MultipleRepeat = "MultipleRepeat";
            public const string Plant = "Plant";
            public const string Prescription = "Prescription";
            public const string PrescriptionMaps = "PrescriptionMaps";
            public const string SingleRepeat = "SingleRepeat";
            public const string SiteSelection = "SiteSelection";
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

            ReadLandisDataVar();

            Parameters parameters = new Parameters();

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

            CheckNoDataAfter("the " + summaryLogFile.Name + " parameter");
            return parameters; 
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// Reads 0 or more prescriptions from text input.
        /// </summary>
        new protected void ReadPrescriptions(List<Prescription> prescriptions,
                                             int harvestTimestep)
        {
            Dictionary<string, int> lineNumbers = new Dictionary<string, int>();

            InputVar<int> singleRepeat = new InputVar<int>(Names.SingleRepeat);
            InputVar<int> multipleRepeat = new InputVar<int>(Names.MultipleRepeat);

            int nameLineNumber = LineNumber;
            InputVar<string> prescriptionName = new InputVar<string>(Names.Prescription);
            while (ReadOptionalVar(prescriptionName))
            {
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
                minTimeSinceDamage = 0;
                InputVar<int> minTimeSinceDamageVar = new InputVar<int>("MinTimeSinceDamage");
                if (ReadOptionalVar(minTimeSinceDamageVar))
                {
                    minTimeSinceDamage = minTimeSinceDamageVar.Value;
                }

                //get preventEstablishment
                bool preventEstablishment = false;
                if (ReadOptionalName("PreventEstablishment"))
                {
                    preventEstablishment = true;
                }


                ICohortSelector cohortSelector = ReadCohortSelector(false);
                MultiSpeciesCohortSelector newCohortSelector = new MultiSpeciesCohortSelector();

                if (ageOrRangeWasRead)
                {
                    //PlugIn.ModelCore.UI.WriteLine("age or range was read");
                    siteSelector = WrapSiteSelector(siteSelector);
                    isSiteSelectorWrapped = true;
                    newCohortSelector = ReplaceSpecificAgeSelectors(cohortSelector);
                }

                Planting.SpeciesList speciesToPlant = ReadSpeciesToPlant();

                //  Repeat harvest?
                int repeatParamLineNumber = LineNumber;
                if (ReadOptionalVar(singleRepeat))
                {
                    MultiSpeciesCohortSelector newAddCohortSelector = new MultiSpeciesCohortSelector();
                    int interval = ValidateRepeatInterval(singleRepeat.Value,
                                                          repeatParamLineNumber,
                                                          harvestTimestep);
                    ICohortSelector additionalCohortSelector = ReadCohortSelector(true);
                    if (ageOrRangeWasRead)
                    {
                        newAddCohortSelector = ReplaceSpecificAgeSelectors(additionalCohortSelector);
                        if (!isSiteSelectorWrapped)
                        {
                            siteSelector = WrapSiteSelector(siteSelector);
                            isSiteSelectorWrapped = true;
                        }
                    }
                    Planting.SpeciesList additionalSpeciesToPlant = ReadSpeciesToPlant();
                    ISiteSelector additionalSiteSelector = WrapSiteSelector(new CompleteStand());
                    prescriptions.Add(new SingleRepeatHarvest(name,
                                                              rankingMethod,
                                                              siteSelector,
                                                              cohortSelector,
                                                              speciesToPlant,
                                                              additionalCohortSelector,
                                                              additionalSpeciesToPlant,
                                                              additionalSiteSelector,
                                                              minTimeSinceDamage,
                                                              preventEstablishment,
                                                              interval));
                }
                else if (ReadOptionalVar(multipleRepeat))
                {
                    int interval = ValidateRepeatInterval(multipleRepeat.Value,
                                                          repeatParamLineNumber,
                                                          harvestTimestep);
                    // After initial site selection repeats must be complete stand
                    ISiteSelector additionalSiteSelector = WrapSiteSelector(new CompleteStand());
                    prescriptions.Add(new RepeatHarvest(name,
                                                        rankingMethod,
                                                        siteSelector,
                                                        cohortSelector,
                                                        speciesToPlant,
                                                        additionalSiteSelector,
                                                        minTimeSinceDamage,
                                                        preventEstablishment,
                                                        interval));
                }
                else
                {
                    prescriptions.Add(new Prescription(name,
                                                       rankingMethod,
                                                       siteSelector,
                                                       cohortSelector,
                                                       speciesToPlant,
                                                       minTimeSinceDamage,
                                                       preventEstablishment));
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
            int optionalStatements = 0;
            
            //check if this is the ForestTypeTable
            if (CurrentName == Names.ForestTypeTable) {
                ReadName(Names.ForestTypeTable);

                //fresh input variables for table
                InputVar<string> inclusionRule = new InputVar<string>("Inclusion Rule");
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
                    //PlugIn.ModelCore.UI.WriteLine("percentOfCells = {0}", percentOfCells.Value.String);
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
                StandSpreading.ValidateTargetSizes(minTargetSize.Value, maxTargetSize.Value);
                standSpreadMinTargetSize = minTargetSize.Value;
                standSpreadMaxTargetSize = maxTargetSize.Value;
                
                
                if (name == SiteSelection.TargetAndSpreading) {
                    // Site selection -- partial spread
                    selector = new PartialStandSpreading(minTargetSize.Value.Actual,
                        maxTargetSize.Value.Actual);
                }
                else {
                    //  Site selection -- complete stand
                    selector = new CompleteStandSpreading(minTargetSize.Value.Actual,
                    maxTargetSize.Value.Actual);

                }
                valueAsStr.AppendFormat(" {0}", minTargetSize.Value.String);
                valueAsStr.AppendFormat(" {0}", maxTargetSize.Value.String);


                //validate the target size for spreading algorithms
                //StandSpreading.ValidateTargetSize(targetSize.Value);
                //standSpreadTargetSize = targetSize.Value;

                //if (name == SiteSelection.TargetAndSpreading) {
                    //  Site selection -- partial spread
                //    selector = new PartialStandSpreading(targetSize.Value.Actual);
                //}
                //else {
                    //  Site selection -- complete stand
                //    selector = new CompleteStandSpreading(targetSize.Value.Actual);
                //}
                //valueAsStr.AppendFormat(" {0}", targetSize.Value.String);

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
#endif
    }
}
