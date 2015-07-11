//  Copyright 2006 University of Wisconsin-Madison
//  Authors:  Robert Scheller, Jimm Domingo
//  License:  Available at  
//  http://landis.forest.wisc.edu/developers/LANDIS-IISourceCodeLicenseAgreement.pdf

using Edu.Wisc.Forest.Flel.Util;
using Landis.Species;
using Landis.Util;
using System.Collections.Generic;
using System.Text;

namespace Landis.Fuels
{
    /// <summary>
    /// A parser that reads the plug-in's parameters from text input.
    /// </summary>
    public class ParametersParser
        : Landis.TextParser<IParameters>
    {
        public static IDataset SpeciesDataset = null;

        //---------------------------------------------------------------------

        public override string LandisDataValue
        {
            get {
                return "Fuels 2006";
            }
        }

        //---------------------------------------------------------------------

        public ParametersParser()
        {
            RegisterForInputValues();
        }

        //---------------------------------------------------------------------

        protected override IParameters Parse()
        {
            ReadLandisDataVar();

            EditableParameters parameters = new EditableParameters(SpeciesDataset.Count);

            InputVar<int> timestep = new InputVar<int>("Timestep");
            ReadVar(timestep);
            parameters.Timestep = timestep.Value;

            // Table of Fuel coefficients
            InputVar<string> speciesName = new InputVar<string>("Species");
            InputVar<double> FuelCoeff = new InputVar<double>("Fuel Coefficient");

            Dictionary <string, int> lineNumbers = new Dictionary<string, int>();

            //const string ConiferNames = "ConiferSpeciesList";
            const string HardwoodLabel = "HardwoodMaximum";

            while (! AtEndOfInput && CurrentName != HardwoodLabel) { //ConiferNames) {
                StringReader currentLine = new StringReader(CurrentLine);

                ReadValue(speciesName, currentLine);
                ISpecies species = GetSpecies(speciesName.Value);
                CheckForRepeatedName(speciesName.Value, "species", lineNumbers);

                ReadValue(FuelCoeff, currentLine);
                parameters.FuelCoefficients[species.Index] = FuelCoeff.Value;

                CheckNoDataAfter(string.Format("the {0} column", FuelCoeff.Name),
                                 currentLine);
                GetNextLine();
            }

            const string FuelTypeNames = "FuelTypes";


            // ------------------------------------------------------
            // List of Conifer Species
            /*

            ReadName(ConiferNames);

            const string DecidNames = "DeciduousSpeciesList";

            //  Read the species for the fuel type:
            List<string> speciesNames = new List<string>();

            while (! AtEndOfInput && CurrentName != DecidNames) {
                StringReader currentLine = new StringReader(CurrentLine);

                TextReader.SkipWhitespace(currentLine);
                while (currentLine.Peek() != -1) 
                {
                
                    ReadValue(speciesName, currentLine);
                    string name = speciesName.Value.Actual;
 
                    ISpecies species = GetSpecies(new InputValue<string>(name, speciesName.Value.String));
                    if (speciesNames.Contains(species.Name))
                        throw NewParseException("The species {0} appears more than once.", species.Name);
                    speciesNames.Add(species.Name);

                    parameters.ConiferIndex[species.Index] = true;

                    TextReader.SkipWhitespace(currentLine);
                }

                GetNextLine();
            }

            // ------------------------------------------------------
            // List of Deciduous Species

            ReadName(DecidNames);

            const string HardwoodLabel = "HardwoodMaximum";
            const string FuelTypeNames = "FuelTypes";

            //  Read the species for the fuel type:
            List<string> speciesNamesD = new List<string>();

            while (! AtEndOfInput && CurrentName != HardwoodLabel) {
                StringReader currentLine = new StringReader(CurrentLine);

                TextReader.SkipWhitespace(currentLine);
                while (currentLine.Peek() != -1) 
                {
                
                    ReadValue(speciesName, currentLine);
                    string name = speciesName.Value.Actual;
 
                    ISpecies species = GetSpecies(new InputValue<string>(name, speciesName.Value.String));
                    if (speciesNames.Contains(species.Name))
                        throw NewParseException("The species {0} appears more than once.", species.Name);
                    speciesNamesD.Add(species.Name);

                    parameters.DecidIndex[species.Index] = true;

                    TextReader.SkipWhitespace(currentLine);
                }

                GetNextLine();
            }*/


            //------------------------------------------------------------
            InputVar<int> hardwoodMax = new InputVar<int>("HardwoodMaximum");
            ReadVar(hardwoodMax);
            parameters.HardwoodMax = hardwoodMax.Value;
            
            InputVar<int> deadFirMaxAge = new InputVar<int>("DeadFirMaxAge");
            ReadVar(deadFirMaxAge);
            parameters.DeadFirMaxAge = deadFirMaxAge.Value;
            
            
            //------------------------------------------------------------
            //  Read definitions of Fuel maps
            
            UI.WriteLine("   Reading in the Fuel Assignment table...");
            ReadName(FuelTypeNames);
            
            List<string> speciesNames = new List<string>();

            //InputVar<FuelTypeCode> ft = new InputVar<FuelTypeCode>("CFS Fuel Type");
            InputVar<int> fi = new InputVar<int>("Fuel Index:  MATCHES TABLE in FIRE INPUT");
            InputVar<BaseFuelType> bft = new InputVar<BaseFuelType>("Base Fuel Type");
            InputVar<int> minAge = new InputVar<int>("Min Age");
            InputVar<int> maxAge = new InputVar<int>("Max Age");

            lineNumbers.Clear();
            Dictionary <int, int> FuelTypeLineNumbers = new Dictionary<int, int>();

            const string DisturbanceConversionTable = "DisturbanceConversionTable";

            while (! AtEndOfInput && CurrentName != DisturbanceConversionTable) {
                StringReader currentLine = new StringReader(CurrentLine);

                //ReadValue(ft, currentLine);
                //CheckForRepeatedName(ft.Value.ToString(), "fuel type", FuelTypeLineNumbers);
                ReadValue(fi, currentLine);
                CheckForRepeatedIndex(fi.Value, "fuel type", FuelTypeLineNumbers);

                IEditableFuelType currentFuelType = new EditableFuelType(SpeciesDataset.Count);
                parameters.FuelTypes.Add(currentFuelType);
                
                //currentFuelType.Name = ft.Value;
                currentFuelType.FuelIndex = fi.Value;
              
                ReadValue(bft, currentLine);
                currentFuelType.BaseFuel = bft.Value;

                // Read the age ranges for the species:
                ReadValue(minAge, currentLine);
                currentFuelType.MinAge = minAge.Value;
                
                TextReader.SkipWhitespace(currentLine);
                string word = TextReader.ReadWord(currentLine);
                if (word != "to") {
                    StringBuilder message = new StringBuilder();
                    message.AppendFormat("Expected \"to\" after the minimum age ({0})",
                                         minAge.Value.String);
                    if (word.Length > 0)
                        message.AppendFormat(", but found \"{0}\" instead", word);
                    throw NewParseException(message.ToString());
                }

                ReadValue(maxAge, currentLine);
                currentFuelType.MaxAge = maxAge.Value;

                //  Read the species for the fuel type:
                //List<string> 
                speciesNames = new List<string>();

                TextReader.SkipWhitespace(currentLine);
                while (currentLine.Peek() != -1) 
                {
                    ReadValue(speciesName, currentLine);
                    string name = speciesName.Value.Actual;
                    bool negativeMultiplier = name.StartsWith("-");
                    if (negativeMultiplier) 
                    {
                        name = name.Substring(1);
                        if (name.Length == 0)
                            throw new InputValueException(speciesName.Value.String,
                                "No species name after \"-\"");
                    }
                    ISpecies species = GetSpecies(new InputValue<string>(name, speciesName.Value.String));
                    if (speciesNames.Contains(species.Name))
                        throw NewParseException("The species {0} appears more than once.", species.Name);
                    speciesNames.Add(species.Name);

                    currentFuelType[species.Index] = negativeMultiplier ? -1 : 1;

                    TextReader.SkipWhitespace(currentLine);
                }
                if (speciesNames.Count == 0)
                    throw NewParseException("At least one species is required.");

                GetNextLine();
            }

            //------------------------------------------------------------
            //  Read definitions of Slash Types
            UI.WriteLine("   Reading in the Harvest Conversion table...");
            ReadName(DisturbanceConversionTable);

            //InputVar<FuelTypeCode> fts = new InputVar<FuelTypeCode>("CFS Fuel Type");
            InputVar<int> fti = new InputVar<int>("Fuel Index");
            InputVar<int> maxAgeS = new InputVar<int>("Max Age");
            InputVar<string> prescriptionName = new InputVar<string>("Prescription");

            lineNumbers.Clear();
            Dictionary <int, int> SlashTypeLineNumbers = new Dictionary<int, int>();

            const string MapFileNames = "MapFileNames";
            const string PctConiferFileName = "PctConiferFileName";
            const string PctDeadFirFileName = "PctDeadFirFileName";

            while (! AtEndOfInput && CurrentName != MapFileNames) {
                StringReader currentLine = new StringReader(CurrentLine);

                //ReadValue(fts, currentLine);
                //CheckForRepeatedName(fts.Value.ToString(), "slash type", SlashTypeLineNumbers);

                ReadValue(fti, currentLine);
                //CheckForRepeatedIndex(fti.Value, "slash type", SlashTypeLineNumbers);

                IEditableSlashType currentSlashType = new EditableSlashType();
                parameters.SlashTypes.Add(currentSlashType);
                
                //currentSlashType.Name = fts.Value;
                currentSlashType.FuelIndex = fti.Value;

                ReadValue(maxAgeS, currentLine);
                currentSlashType.MaxAge = maxAgeS.Value;

                List<string> prescriptionNames = new List<string>();

                TextReader.SkipWhitespace(currentLine);
                while (currentLine.Peek() != -1) {
                    ReadValue(prescriptionName, currentLine);
                    prescriptionNames.Add(prescriptionName.Value);

                    TextReader.SkipWhitespace(currentLine);
                }
                if (prescriptionNames.Count == 0)
                    throw NewParseException("At least one prescription is required.");
                    
                currentSlashType.PrescriptionNames = prescriptionNames;

                GetNextLine();
            }
            //------------------------------------------------------------
            // Template for filenames of Fuel maps

            UI.WriteLine("   Reading in map names...");

            InputVar<string> mapFileNames = new InputVar<string>(MapFileNames);
            ReadVar(mapFileNames);
            parameters.MapFileNames = mapFileNames.Value;
            
            //
            //GetNextLine();
            //------------------------------------------------------------
            // Template for filenames of percent conifer maps

            InputVar<string> pctConiferFileName = new InputVar<string>(PctConiferFileName);
            ReadVar(pctConiferFileName);
            parameters.PctConiferFileName = pctConiferFileName.Value;

            //GetNextLine();
            //------------------------------------------------------------
            // Template for filenames of percent dead fir maps

            InputVar<string> pctDeadFirFileName = new InputVar<string>(PctDeadFirFileName);
            ReadVar(pctDeadFirFileName);
            parameters.PctDeadFirFileName = pctDeadFirFileName.Value;

            CheckNoDataAfter(string.Format("the {0} parameter", PctDeadFirFileName));

            return parameters.GetComplete();
        }

        //---------------------------------------------------------------------

        protected ISpecies GetSpecies(InputValue<string> name)
        {
            ISpecies species = SpeciesDataset[name.Actual];
            if (species == null)
                throw new InputValueException(name.String,
                                              "{0} is not a species name.",
                                              name.String);
            return species;
        }
        //---------------------------------------------------------------------

        private void CheckForRepeatedIndex(int index,
                                          string      description,
                                          Dictionary<int, int> lineNumbers)
        {
            int lineNumber;
            if (lineNumbers.TryGetValue(index, out lineNumber))
                throw new InputValueException(index.ToString(),
                                              "The {0} {1} was previously used on line {2}",
                                              description, index, lineNumber);
            lineNumbers[index] = LineNumber;
        }

        //---------------------------------------------------------------------

        private void CheckForRepeatedName(string      name,
                                          string      description,
                                          Dictionary<string, int> lineNumbers)
        {
            int lineNumber;
            if (lineNumbers.TryGetValue(name, out lineNumber))
                throw new InputValueException(name,
                                              "The {0} {1} was previously used on line {2}",
                                              description, name, lineNumber);
            lineNumbers[name] = LineNumber;
        }
        
        public static BaseFuelType BFTParse(string word)
        {
            if (word == "Conifer")
                return BaseFuelType.Conifer;
            else if (word == "ConiferPlantation")
                return BaseFuelType.ConiferPlantation;
            else if (word == "Deciduous")
                return BaseFuelType.Deciduous;
            else if (word == "Open")
                return BaseFuelType.Open;
            else if (word == "NoFuel")
                return BaseFuelType.NoFuel;
            else if (word == "Slash")
                return BaseFuelType.Slash;
                
            throw new System.FormatException("Valid Fuel Types: Conifer, ConiferPlantation, Deciduous, Open, NoFuel, Slash.");
        }


        //---------------------------------------------------------------------

        /// <summary>
        /// Registers the appropriate method for reading input values.
        /// </summary>
        public static void RegisterForInputValues()
        {
            Type.SetDescription<BaseFuelType>("BaseFuelType");
            InputValues.Register<BaseFuelType>(BFTParse);

        }
    }
}
