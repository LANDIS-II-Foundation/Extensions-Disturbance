
using System.Collections.Generic;
using Landis.Core;
using Edu.Wisc.Forest.Flel.Util;

namespace Landis.Extension.StressMortality
{
    class InputParametersParser
        :TextParser<IInputParameters>
    {

        //---------------------------------------------------------------------

        public override string LandisDataValue
        {
            get
            {
                return PlugIn.ExtensionName;
            }
        }


        //---------------------------------------------------------------------
        private Dictionary<string, int> speciesLineNums;
        private InputVar<string> speciesName;
        
        //---------------------------------------------------------------------
        public InputParametersParser()
        {
            this.speciesLineNums = new Dictionary<string, int>();
            this.speciesName = new InputVar<string>("Species");
        }

        //---------------------------------------------------------------------
        protected override IInputParameters Parse()
        {

            ReadLandisDataVar();
            
            //InputVar<string> landisData = new InputVar<string>("LandisData");
            //ReadVar(landisData);
            //if (landisData.Value.Actual != PlugIn.ExtensionName)
            //    throw new InputValueException(landisData.Value.String, "The value is not \"{0}\"", PlugIn.ExtensionName);

            InputParameters parameters = new InputParameters();

            //InputVar<int> timestep = new InputVar<int>("Timestep");
            //ReadVar(timestep);
            //parameters.Timestep = timestep.Value;

            //-------------------------
            //  Species Mortality table
            ReadName("StressOnsetTable");
            Dictionary<int, List<IDynamicInputRecord>> allData = new Dictionary<int, List<IDynamicInputRecord>>();

            //---------------------------------------------------------------------
            //Read in onset table data:
            InputVar<int> year = new InputVar<int>("Time step for updating values");
            InputVar<string> ecoregionName = new InputVar<string>("Ecoregion Name");
            InputVar<string> speciesName = new InputVar<string>("Species Name");

            while (!AtEndOfInput && CurrentName != "PartialMortalityTable")
            {
                StringReader currentLine = new StringReader(CurrentLine);

                ReadValue(year, currentLine);
                int yr = year.Value.Actual;

                if (yr < 1)
                    throw new InputVariableException(year, "The stress year must be > 1 {0}", "");
                
                if (!allData.ContainsKey(yr))
                {
                    List<IDynamicInputRecord> inputTable = new List<IDynamicInputRecord>();
                    allData.Add(yr, inputTable);
                    PlugIn.ModelCore.UI.WriteLine("  Dynamic Input Parser:  Add new year = {0}.", yr);
                }

                IDynamicInputRecord dynamicInputRecord = new DynamicInputRecord();
                
                ReadValue(ecoregionName, currentLine);
                IEcoregion ecoregion = GetEcoregion(ecoregionName.Value);
                dynamicInputRecord.OnsetEcoregion = ecoregion;

                ReadValue(speciesName, currentLine);
                ISpecies species = GetSpecies(speciesName.Value);
                
                dynamicInputRecord.OnsetSpecies = species;

                allData[yr].Add(dynamicInputRecord);

                //allData[yr] = dynamicInputRecord;

                GetNextLine();

            }

            DynamicInputs.AllData = allData;

            // ---------------------------------------------------------------------
            // Partial Mortality Table

            ReadName("PartialMortalityTable");
            speciesLineNums.Clear();  //  If parser re-used (i.e., for testing purposes)

            InputVar<string> speciesNameVar = new InputVar<string>("Species");
            AgeClass ageClass = new AgeClass();
           
            while (!AtEndOfInput && CurrentName != "CompleteMortalityTable")
            {
                List<AgeClass> ageClasses = new List<AgeClass>();
                string word = "";
                bool success = false;
                //int lineNumber = 0;
                
                StringReader currentLine = new StringReader(CurrentLine);
                ISpecies species = ReadSpecies(currentLine);

                Dictionary<string, int> lineNumbers = new Dictionary<string, int>();
                lineNumbers[species.Name] = LineNumber;

                if (currentLine.Peek() == -1)
                    throw new InputVariableException(speciesNameVar, "No age classes were defined for species: {0}", species.Name);
                while (currentLine.Peek() != -1)
                {
                    TextReader.SkipWhitespace(currentLine);
                    word = TextReader.ReadWord(currentLine);
                    if (word == "")
                    {
                        if (!success)
                            throw new InputVariableException(speciesNameVar, "No age classes were defined for species: {0}", species.Name);
                        else
                            break;
                    }
                    ageClass = new AgeClass();
                    success = ageClass.Parse(word);
                    if (!success)
                        throw new InputVariableException(speciesNameVar, "Entry is not a valid age class: {0}", word);
                    
                    //PlugIn.ModelCore.Log.WriteLine("  Adding {0} age class: {1}-{2}, fraction={3}.", species.Name, ageClass.LwrAge, ageClass.UprAge, ageClass.MortalityFraction);
                    ageClasses.Add(ageClass);

                }
                success = false;

                parameters.SetPartialMortalityTable(species, ageClasses);
                //foreach (AgeClass ac in parameters.PartialMortalityTable[species])
                //    PlugIn.ModelCore.Log.WriteLine("  Added {0} age class: {1}-{2}, fraction={3}.", species.Name, ac.LwrAge, ac.UprAge, ac.MortalityFraction);
                GetNextLine();
            }

            // ---------------------------------------------------------------------
            // Complete Mortality Table
            
            ReadName("CompleteMortalityTable");
            speciesLineNums.Clear();  //  If parser re-used (i.e., for testing purposes)

            InputVar<int> cmp = new InputVar<int>("Complete Mortality Threshold");
            InputVar<int> cmt = new InputVar<int>("Complete Mortality Time Window");

            while (!AtEndOfInput && CurrentName != "MapName")
            {

                StringReader currentLine = new StringReader(CurrentLine);
                ISpecies species = ReadSpecies(currentLine);

                ReadValue(cmp, currentLine);
                parameters.SetCompleteMortalityThreshold(species, cmp.Value);
                
                ReadValue(cmt, currentLine);
                parameters.SetCompleteMortalityTime(species, cmt.Value);

                CheckNoDataAfter(cmt.Name, currentLine);
                GetNextLine();
            }

            // ---------------------------------------------------------------------

            InputVar<string> mapNames = new InputVar<string>("MapName");
            ReadVar(mapNames);
            parameters.MapNamesTemplate = mapNames.Value;

            // ---------------------------------------------------------------------

            InputVar<string> logFile = new InputVar<string>("LogFile");
            ReadVar(logFile);
            parameters.LogFileName = logFile.Value;

            return parameters;

        }
        /// <summary>
        /// Reads a species name from the current line, and verifies the name.
        /// </summary>
        private ISpecies ReadSpecies(StringReader currentLine)
        {
            ReadValue(speciesName, currentLine);
            ISpecies species = PlugIn.ModelCore.Species[speciesName.Value.Actual];
            if (species == null)
                throw new InputValueException(speciesName.Value.String,
                                              "{0} is not a species name.",
                                              speciesName.Value.String);
            int lineNumber;
            if (speciesLineNums.TryGetValue(species.Name, out lineNumber))
                throw new InputValueException(speciesName.Value.String,
                                              "The species {0} was previously used on line {1}",
                                              speciesName.Value.String, lineNumber);
            else
                speciesLineNums[species.Name] = LineNumber;
            return species;
        }
        //---------------------------------------------------------------------

        private IEcoregion GetEcoregion(InputValue<string> ecoregionName)
        {
            IEcoregion ecoregion = PlugIn.ModelCore.Ecoregions[ecoregionName.Actual];
            if (ecoregion == null)
                throw new InputValueException(ecoregionName.String,
                                              "{0} is not an ecoregion name.",
                                              ecoregionName.String);
            if (!ecoregion.Active)
                throw new InputValueException(ecoregionName.String,
                                              "{0} is not an active ecoregion.",
                                              ecoregionName.String);

            return ecoregion;
        }

        //---------------------------------------------------------------------

        private ISpecies GetSpecies(InputValue<string> speciesName)
        {
            ISpecies species = PlugIn.ModelCore.Species[speciesName.Actual];
            if (species == null)
                throw new InputValueException(speciesName.String,
                                              "{0} is not a recognized species name.",
                                              speciesName.String);

            return species;
        }


    }
}
