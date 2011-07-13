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
        //private static readonly bool isDebugEnabled = false;
        public static readonly ExtensionType Type = new ExtensionType("disturbance:stress");
        public static readonly string ExtensionName = "Stress Mortality";

        private string mapNameTemplate;
        private StreamWriter log;
        private static IInputParameters parameters;
        private static ICore modelCore;

        //Landis.Extension.Succession.Biomass.PlugIn.

        //---------------------------------------------------------------------
        public PlugIn()
            : base(ExtensionName, Type)
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
        public override void LoadParameters(string dataFile,
                                            ICore mCore)
        {
            modelCore = mCore;
            InputParametersParser parser = new InputParametersParser();
            parameters = modelCore.Load<IInputParameters>(dataFile, parser);
        }

        //---------------------------------------------------------------------
        public override void Initialize()
        {

            Timestep = parameters.Timestep;
            mapNameTemplate = parameters.MapNamesTemplate;

            SiteVars.Initialize();
            PartialDisturbance.Initialize();
            SpeciesData.Initialize(parameters);

            modelCore.Log.WriteLine("   Opening and Initializing Stress Mortality log file \"{0}\"...", parameters.LogFileName);
            try
            {
                log = modelCore.CreateTextFile(parameters.LogFileName);
            }
            catch (Exception err)
            {
                string mesg = string.Format("{0}", err.Message);
                throw new System.ApplicationException(mesg);
            }
            log.AutoFlush = true;

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
            //foreach (ISpecies species in PlugIn.ModelCore.Species)
            //{
            //    log.Write("ExtraRem_{0},", species.Name);
            //}
            log.WriteLine("");
        }

        //---------------------------------------------------------------------
        ///<summary>
        /// Run the plug-in at a particular timestep.
        ///</summary>
        public override void Run()
        {
            modelCore.Log.WriteLine("   Processing Stress Mortality ...");


            //-------------------------------------------------------------------
            // HOW TO USE DATA STRUCTURE FOR AGE CLASSES

            /*foreach (ISpecies species in modelCore.Species)
            {
                modelCore.Log.WriteLine("   calculating age ranges for {0}", species.Name);
                foreach (AgeClass ageclass in SpeciesData.PartialMortalityTable[species])
                {
                    ushort lwr_age = ageclass.LwrAge;
                    ushort upr_age = ageclass.UprAge;
                    double mortality_fraction = ageclass.MortalityFraction;
                }

            }*/
            
            foreach (ActiveSite site in PlugIn.ModelCore.Landscape)
            {
                IEcoregion ecoregion = PlugIn.ModelCore.Ecoregion[site];

                foreach (ISpecies species in modelCore.Species)
                {
                        if (SpeciesData.IsOnsetYear(modelCore.CurrentTime, species, ecoregion))
                        {   
                            PartialMortality(site, species);
                        }
                }
                PartialDisturbance.ReduceCohortBiomass(site);
            }
        }

        //---------------------------------------------------------------------
        public static void PartialMortality(ActiveSite site, ISpecies species)
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


    }
}