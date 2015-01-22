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
    }
}
