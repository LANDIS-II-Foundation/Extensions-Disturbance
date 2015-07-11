using Landis.Core;
using Landis.SpatialModeling;
using Landis.Library.BiomassCohorts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Landis.Extension.Succession.Biomass;
using Edu.Wisc.Forest.Flel.Util;


namespace Landis.Extension.StressMortality
{
    public class PlugIn
        : ExtensionMain
    {
        public static readonly ExtensionType ExtType = new ExtensionType("disturbance:stress");
        public static readonly string ExtensionName = "Stress Mortality";

        private string mapNameTemplate;
        private StreamWriter log;
        private static IInputParameters parameters;
        private static ICore modelCore;
        public static int[] StressCohortsKilled; 

        //---------------------------------------------------------------------
        public PlugIn()
            : base(ExtensionName, ExtType)
        {
        }

        //---------------------------------------------------------------------
        public static ICore ModelCore
        {
            get
            {
                return modelCore;
            }
        }
        //---------------------------------------------------------------------
        public override void LoadParameters(string dataFile, ICore mCore)
        {
            modelCore = mCore;
            InputParametersParser parser = new InputParametersParser();
            parameters = Landis.Data.Load<IInputParameters>(dataFile, parser);
        }

        //---------------------------------------------------------------------
        public override void Initialize()
        {

            Timestep = 1; // parameters.Timestep;
            mapNameTemplate = parameters.MapNamesTemplate;

            SiteVars.Initialize();
            PartialDisturbance.Initialize();
            SpeciesData.Initialize(parameters);

            modelCore.UI.WriteLine("   Opening and Initializing Stress Mortality log file \"{0}\"...", parameters.LogFileName);
            try
            {
                log = Landis.Data.CreateTextFile(parameters.LogFileName);
            }
            catch (Exception err)
            {
                string mesg = string.Format("{0}", err.Message);
                throw new System.ApplicationException(mesg);
            }
            log.AutoFlush = true;

            log.Write("Year, Ecoregion,");
            foreach (ISpecies species in PlugIn.ModelCore.Species)
            {
                log.Write("BioRemoved_{0},", species.Name);
            }
            log.Write("TotalBioRemoved,");
            foreach (ISpecies species in PlugIn.ModelCore.Species)
            {
                log.Write("CohortsKilled_{0},", species.Name);
            }
            log.Write("TotalCohortsKilled,");
            log.WriteLine("");
        }

        //---------------------------------------------------------------------
        ///<summary>
        /// Run the plug-in at a particular timestep.
        ///</summary>
        public override void Run()
        {
            modelCore.UI.WriteLine("   Processing Stress Mortality ...");
            
            SiteVars.StressBioRemoved.ActiveSiteValues = 0;

            StressCohortsKilled = new int[modelCore.Ecoregions.Count];
            foreach (IEcoregion ecoregion in modelCore.Ecoregions)
            {
                foreach (ISpecies species in modelCore.Species)
                {
                    SpeciesData.SppBiomassRemoved[species][ecoregion] = 0;
                    SpeciesData.CohortsKilled[species][ecoregion] = 0;
                }
            }
            
            PartialDisturbance.Initialize();
            
            foreach (ActiveSite site in PlugIn.ModelCore.Landscape)
            {
                IEcoregion ecoregion = PlugIn.ModelCore.Ecoregion[site];

                foreach (ISpecies species in modelCore.Species)
                {
                    if (SpeciesData.IsOnsetYear(modelCore.CurrentTime, species, ecoregion))
                    {   
                        StressMortality(site, species);
                    }
                }
                PartialDisturbance.ReduceCohortBiomass(site);
            }

            WriteMapLogFile();
        }

        //---------------------------------------------------------------------
        public static void StressMortality(ActiveSite site, ISpecies species)
        {
            
            foreach (ISpeciesCohorts cohorts in SiteVars.Cohorts[site])
            {

                if (cohorts.Species != species)
                    continue;

                foreach(AgeClass ageclass in SpeciesData.PartialMortalityTable[cohorts.Species])
                {

                    ushort lwr_age = ageclass.LwrAge;
                    ushort upr_age = ageclass.UprAge;
                    
                    switch (ageclass.BinType)
                    {
                        case 1:
                        {// <
                            foreach (ICohort cohort in cohorts)
                            {
                                if (cohort.Age < upr_age)
                                {
                                    PartialDisturbance.RecordBiomassReduction(site, cohort, ageclass.MortalityFraction); 
                                }
                            }
                            break;
                        }

                        case 2:
                        {// Range - equivalent to (>= lwr_age and <upr_age)
                            foreach (ICohort cohort in cohorts)
                            {
                                if (cohort.Age >= lwr_age && cohort.Age < upr_age)
                                {
                                    //PlugIn.ModelCore.Log.WriteLine("ageclass mortality fraction = {0}.", ageclass.MortalityFraction);
                                    PartialDisturbance.RecordBiomassReduction(site, cohort, ageclass.MortalityFraction); 
                                }
                                else if (cohort.Age < lwr_age)
                                    break;//we can break here, since ages sorted descending order
                            }
                            break;
                        }

                        case 3:
                        {// >  (is this equivalent to >= ??)
                            foreach (ICohort cohort in cohorts)
                            {
                                if (cohort.Age >= lwr_age)
                                {
                                    PartialDisturbance.RecordBiomassReduction(site, cohort, ageclass.MortalityFraction); 
                                }
                                else
                                    break;//we can break here, since ages sorted descending order
                            }

                            break;
                        }
                        case 4:
                        {// Single value
                            foreach (ICohort cohort in cohorts)
                            {
                                if (cohort.Age == lwr_age)
                                {
                                    PartialDisturbance.RecordBiomassReduction(site, cohort, ageclass.MortalityFraction); 
                                }
                                else if (cohort.Age < lwr_age)
                                    break;//we can break here, since ages sorted descending order
                            }
                            break;
                        }

                        default:
                        {
                            throw new InputValueException("", "Unhandled binning type; this should never occur");
                        }
                    }
                }
            }

            return;
        }

        //---------------------------------------------------------------------
        private void WriteMapLogFile()
        {
            int[] stressBioRemoved = new int[modelCore.Ecoregions.Count];
            foreach (ActiveSite site in PlugIn.ModelCore.Landscape)
            {
                IEcoregion ecoregion = PlugIn.ModelCore.Ecoregion[site];
                stressBioRemoved[ecoregion.Index] += SiteVars.StressBioRemoved[site];
            }


            //  Write biomass removed map
            string path = MapNames.ReplaceTemplateVars(mapNameTemplate, PlugIn.modelCore.CurrentTime);
            using (IOutputRaster<IntPixel> outputRaster = modelCore.CreateRaster<IntPixel>(path, modelCore.Landscape.Dimensions))
            {
                IntPixel pixel = outputRaster.BufferPixel;
                foreach (Site site in modelCore.Landscape.AllSites)
                {
                    if (site.IsActive)
                    {
                        pixel.MapCode.Value = (int)(SiteVars.StressBioRemoved[site]);
                    }
                    else
                    {
                        //  Inactive site
                        pixel.MapCode.Value = 0;
                    }
                    outputRaster.WriteBufferPixel();
                }
            }


            foreach (IEcoregion ecoregion in modelCore.Ecoregions)
            {
                log.Write("{0},", PlugIn.ModelCore.CurrentTime);
                log.Write("{0},", ecoregion.Name);
                foreach (ISpecies species in PlugIn.ModelCore.Species)
                    log.Write("{0},", SpeciesData.SppBiomassRemoved[species][ecoregion]);
                log.Write("{0},", stressBioRemoved[ecoregion.Index]);
                    foreach (ISpecies species in PlugIn.ModelCore.Species)
                        log.Write("{0},", SpeciesData.CohortsKilled[species][ecoregion]);
                log.Write("{0}", StressCohortsKilled[ecoregion.Index]);
                log.WriteLine("");
            }
        }

    }
}