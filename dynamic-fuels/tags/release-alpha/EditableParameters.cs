//  Copyright 2006 University of Wisconsin-Madison
//  Authors:  Robert Scheller, Jimm Domingo
//  License:  Available at  
//  http://landis.forest.wisc.edu/developers/LANDIS-IISourceCodeLicenseAgreement.pdf

using Edu.Wisc.Forest.Flel.Util;

namespace Landis.Fuels
{
    /// <summary>
    /// Editable set of parameters for the plug-in.
    /// </summary>
    public class EditableParameters
        : IEditable<IParameters>
    {
        private InputValue<int> timestep;
        private InputValue<int> hardwoodMax;
        private InputValue<int> deadFirMaxAge;
        private EditableCoefficients coefficients;
        //private EditableConiferIndex coniferIndex;
        //private EditableDeciduousIndex decidIndex;
        private ListOfEditable<IFuelType> fuelTypes;
        private ListOfEditable<ISlashType> slashTypes;
        private InputValue<string> mapFileNames;
        private InputValue<string> pctConFileName;
        private InputValue<string> pctDeadFirFileName;

        //---------------------------------------------------------------------

        /// <summary>
        /// Timestep (years)
        /// </summary>
        public InputValue<int> Timestep
        {
            get {
                return timestep;
            }

            set {
                if (value != null) {
                    if (value.Actual < 0)
                        throw new InputValueException(value.String,
                                                      "Value must be = or > 0.");
                }
                timestep = value;
            }
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// Hardwood maximum (percent)
        /// </summary>
        public InputValue<int> HardwoodMax
        {
            get {
                return hardwoodMax;
            }

            set {
                if (value != null) {
                    if (value.Actual < 0 || value.Actual > 100)
                        throw new InputValueException(value.String,
                                                      "Value must be >= 0 and <= 100.");
                }
                hardwoodMax = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Maximum age that the dead fir cohort count will contribute to the percent conifer calculation
        /// </summary>
        public InputValue<int> DeadFirMaxAge
        {
            get {
                return deadFirMaxAge;
            }

            set {
                if (value != null) {
                    if (value.Actual < 0 || value.Actual > 100)
                        throw new InputValueException(value.String,
                                                      "Value must be >= 0 and <= 100.");
                }
                deadFirMaxAge = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Fuel coefficients for species 
        /// </summary>
        public EditableCoefficients FuelCoefficients
        {
            get {
                return coefficients;
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Conifer index for species 
        /// </summary>
        /*
        public EditableConiferIndex ConiferIndex
        {
            get {
                return coniferIndex;
            }
        }
        
        //---------------------------------------------------------------------
        /// <summary>
        /// Deciduous index for species 
        /// </summary>
        public EditableDeciduousIndex DecidIndex
        {
            get {
                return decidIndex;
            }
        }*/
        //---------------------------------------------------------------------

        /// <summary>
        /// Fuel types
        /// </summary>
        public ListOfEditable<IFuelType> FuelTypes
        {
            get {
                return fuelTypes;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Slash types
        /// </summary>
        public ListOfEditable<ISlashType> SlashTypes
        {
            get {
                return slashTypes;
            }
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// Template for the filenames for fuel maps.
        /// </summary>
        public InputValue<string> MapFileNames
        {
            get {
                return mapFileNames;
            }

            set {
                if (value != null) {
                    MapNames.CheckTemplateVars(value.Actual);
                }
                mapFileNames = value;
            }
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// Template for the filenames for percent conifers.
        /// </summary>
        public InputValue<string> PctConiferFileName
        {
            get
            {
                return pctConFileName;
            }

            set
            {
                if (value != null)
                {
                    MapNames.CheckTemplateVars(value.Actual);
                }
                pctConFileName = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Template for the filenames for percent dead fir maps.
        /// </summary>
        public InputValue<string> PctDeadFirFileName
        {
            get
            {
                return pctDeadFirFileName;
            }

            set
            {
                if (value != null)
                {
                    MapNames.CheckTemplateVars(value.Actual);
                }
                pctDeadFirFileName = value;
            }
        }

        //---------------------------------------------------------------------

        public EditableParameters(int speciesCount)
        {
            coefficients = new EditableCoefficients(speciesCount);
            //coniferIndex = new EditableConiferIndex(speciesCount);
            //decidIndex = new EditableDeciduousIndex(speciesCount);
            fuelTypes = new ListOfEditable<IFuelType>();
            slashTypes = new ListOfEditable<ISlashType>();
            
        }

        //---------------------------------------------------------------------

        public bool IsComplete
        {
            get {
                foreach (object parameter in new object[]{  timestep,
                                                            hardwoodMax, 
                                                            deadFirMaxAge,
                                                            mapFileNames, 
                                                            pctConFileName, 
                                                            pctDeadFirFileName 
                                                          }) {
                    if (parameter == null)
                    {
                        UI.WriteLine("One of the non-table variables didn't load correctly.");
                        return false;
                    }
                }
                return coefficients.IsComplete && 
                    //coniferIndex.IsComplete && 
                    //decidIndex.IsComplete &&
                    fuelTypes.IsEachItemComplete &&
                    slashTypes.IsEachItemComplete;
            }
        }

        //---------------------------------------------------------------------

        public IParameters GetComplete()
        {
            if (IsComplete)
                return new Parameters(timestep.Actual,
                                      coefficients.GetComplete(),
                                      //coniferIndex.GetComplete(),
                                      //decidIndex.GetComplete(),
                                      hardwoodMax.Actual,
                                      deadFirMaxAge.Actual,
                                      fuelTypes.GetComplete(),
                                      slashTypes.GetComplete(),
                                      mapFileNames.Actual,
                                      pctConFileName.Actual,
                                      pctDeadFirFileName.Actual);
            else
            {
                UI.WriteLine("IsComplete Failed.");
                return null;
            }
        }
    }
}
