//  Copyright 2005 University of Wisconsin-Madison
//  Authors:  Robert Scheller, Jimm Domingo
//  License:  Available at  
//  http://landis.forest.wisc.edu/developers/LANDIS-IISourceCodeLicenseAgreement.pdf

using Edu.Wisc.Forest.Flel.Util;

namespace Landis.Fuels
{
    /// <summary>
    /// Editable set of fuel coefficients for species.
    /// </summary>
    public class EditableConiferIndex
        : IEditable<bool[]>
    {
        private bool[] coniferIndex;

        //---------------------------------------------------------------------

        /// <summary>
        /// Conifer Index for a species
        /// </summary>
        public bool this[int speciesIndex]
        {
            get {
                return coniferIndex[speciesIndex];
            }

            set {
                coniferIndex[speciesIndex] = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Initialize a new instance.
        /// </summary>
        /// <remarks>
        /// All the conifer indices are initially FALSE.
        /// </remarks>
        public EditableConiferIndex(int speciesCount)
        {
            coniferIndex = new bool[speciesCount];
        }

        //---------------------------------------------------------------------

        public bool IsComplete
        {
            get {
                return true;
            }
        }

        //---------------------------------------------------------------------

        public bool[] GetComplete()
        {
            return coniferIndex;
        }
    }
}
