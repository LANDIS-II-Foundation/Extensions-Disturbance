// Copyright 2008-2010 Green Code LLC, Portland State University
// Authors:  James B. Domingo, Robert M. Scheller,  
 
using BaseHarvest = Landis.Extension.BaseHarvest;

namespace Landis.Extension.BiomassHarvest
{
    /// <summary>
    /// The parameters for biomass harvest.
    /// </summary>
    public class Parameters
        // : BaseHarvest.Parameters, IParameters
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
