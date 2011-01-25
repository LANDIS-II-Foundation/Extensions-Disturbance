/*
 * Copyright 2008 Green Code LLC
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using Edu.Wisc.Forest.Flel.Util;

using BaseHarvest = Landis.Harvest;

namespace Landis.Extensions.BiomassHarvest
{
    /// <summary>
    /// Editable set of parameters for biomass harvest.
    /// </summary>
    public class EditableParameters
        : BaseHarvest.EditableParameters, IEditable<IParameters>
    {
        private InputValue<string> biomassMapNamesTemplate;

        //---------------------------------------------------------------------

        /// <summary>
        /// Template for pathnames for biomass-reduced maps.
        /// </summary>
        public InputValue<string> BiomassMapNames
        {
            get {
                return biomassMapNamesTemplate;
            }

            set {
                if (value != null) {
                    // Since this template for biomass-reduced map names
                    // recognized just one template variable ("{timestep}")
                    // just like the template for prescription map names,
                    // we can use the MapNames class for validation.
                    // TO DO: update documentation for MapNames class.
                    BaseHarvest.MapNames.CheckTemplateVars(value.Actual);
                }
                biomassMapNamesTemplate = value;
            }
        }

        //---------------------------------------------------------------------
/*
 * Since this template is optional for now, we can just use the base class'
 * IsComplete property.
 *
        public new bool IsComplete
        {
            get {
                return base.IsComplete && (biomassMapNamesTemplate != null);
            }
        }
*/
        //---------------------------------------------------------------------

        public EditableParameters()
            : base()
        {
        }

        //---------------------------------------------------------------------

        public new IParameters GetComplete()
        {
            if (this.IsComplete) {
                BaseHarvest.IParameters baseHarvestParams = base.GetComplete();
                return new Parameters(baseHarvestParams.Timestep,
                                      baseHarvestParams.ManagementAreaMap,
                                      baseHarvestParams.ManagementAreas,
                                      baseHarvestParams.StandMap,
                                      baseHarvestParams.PrescriptionMapNames,
                                                        biomassMapNamesTemplate != null
                                                            ? biomassMapNamesTemplate.Actual
                                                            : null,
                                      baseHarvestParams.EventLog);
            }
            else
                return null;
        }
    }
}
