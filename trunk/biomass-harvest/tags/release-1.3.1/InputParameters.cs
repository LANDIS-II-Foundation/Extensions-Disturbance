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

using BaseHarvest = Landis.Harvest;

namespace Landis.Extension.BiomassHarvest
{
    /// <summary>
    /// The parameters for biomass harvest.
    /// </summary>
    public class Parameters
        : BaseHarvest.InputParameters, IParameters
    {
        private string biomassMapNamesTemplate;

        //---------------------------------------------------------------------
        /// <summary>
        /// Template for pathnames for biomass-removed maps.
        /// </summary>
        public string BiomassMapNames
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
                    BaseHarvest.MapNames.CheckTemplateVars(value);
                }
                biomassMapNamesTemplate = value;
            }
        }

        public Parameters()
        {
        }

        //---------------------------------------------------------------------
/*
        public Parameters(int                                timestep,
                          string                             managementAreaMap,
                          BaseHarvest.IManagementAreaDataset managementAreas,
                          string                             standMap,
                          string                             prescriptionMapNamesTemplate,
                          string                             biomassMapNamesTemplate,
                          string                             eventLog,
                          string                             summaryLog)
            : base(timestep,
                   managementAreaMap,
                   managementAreas,
                   standMap,
                   prescriptionMapNamesTemplate,
                   eventLog,
                   summaryLog)
        {
            this.biomassMapNamesTemplate = biomassMapNamesTemplate;
        }*/
    }
}
