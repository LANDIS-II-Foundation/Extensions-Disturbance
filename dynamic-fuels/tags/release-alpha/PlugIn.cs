//  Copyright 2006 University of Wisconsin-Madison
//  Authors:  Robert Scheller, Jimm Domingo
//  License:  Available at  
//  http://landis.forest.wisc.edu/developers/LANDIS-IISourceCodeLicenseAgreement.pdf

using Landis.AgeCohort;
using Landis.Landscape;
using Landis.RasterIO;
using Landis.Species;
using Landis.PlugIns;


using System.Collections.Generic;
using System;

namespace Landis.Fuels
{
    public class PlugIn
        : PlugIns.PlugIn
    {
        private string mapNameTemplate;
        private string pctConiferMapNameTemplate;
        private string pctDeadFirMapNameTemplate;
        private IEnumerable<IFuelType> fuelTypes;
        private IEnumerable<ISlashType> slashTypes;
        private ILandscapeCohorts cohorts;
        private double[] fuelCoefs;
        //private bool[] coniferIndex;
        //private bool[] decidIndex;
        private int hardwoodMax;
        private int deadFirMaxAge;


        //---------------------------------------------------------------------

        public PlugIn()
            : base("Fuels 2006", new PlugIns.PlugInType("output"))
        {
        }

        //---------------------------------------------------------------------

        public override void Initialize(string        dataFile,
                                        PlugIns.ICore modelCore)
        {
            Model.Core = modelCore;

            ParametersParser.SpeciesDataset = Model.Core.Species;
            ParametersParser parser = new ParametersParser();
            IParameters parameters = Data.Load<IParameters>(dataFile,
                                                            parser);

            if(parameters == null)
                UI.WriteLine("Parameters are not loading.");
                                                            

            Timestep = parameters.Timestep;
            mapNameTemplate = parameters.MapFileNames;
            pctConiferMapNameTemplate = parameters.PctConiferFileName;
            pctDeadFirMapNameTemplate = parameters.PctDeadFirFileName;
            fuelTypes = parameters.FuelTypes;
            slashTypes = parameters.SlashTypes;
            fuelCoefs = parameters.FuelCoefficients;
            //coniferIndex = parameters.ConiferIndex;
            //decidIndex = parameters.DecidIndex;
            hardwoodMax = parameters.HardwoodMax;
            deadFirMaxAge = parameters.DeadFirMaxAge;



            cohorts = Model.Core.SuccessionCohorts as ILandscapeCohorts;
            if (cohorts == null)
                throw new ApplicationException("Error: Cohorts don't support age-cohort interface");

            SiteVars.Initialize();

        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Runs the component for a particular timestep.
        /// </summary>
        /// <param name="currentTime">
        /// The current model timestep.
        /// </param>
        public override void Run()
        {
        
            UI.WriteLine("  Re-initializing all values to zero...");
            SiteVars.PercentDeadFir.ActiveSiteValues = 0;
            SiteVars.CFSFuelType.ActiveSiteValues = 0;
            SiteVars.DecidFuelType.ActiveSiteValues = 0;//(int) FuelTypeCode.NoFuel;
            
            UI.WriteLine("  Calculating the CFS Fuel Type for all active cells...");
            foreach (ActiveSite site in Model.Core.Landscape)  //ActiveSites
            {
                CalcFuelType(site, fuelTypes, slashTypes);
                SiteVars.PercentDeadFir[site] = CalcPercentDeadFir(site);
            }

            IOutputRaster<ClassPixel> newmap = CreateMap();
            string path = newmap.Path;
            using (newmap) 
            {
                ClassPixel pixel = new ClassPixel();
                foreach (Site site in Model.Core.Landscape.AllSites) 
                {
                    if (site.IsActive)
                        pixel.Band0 = (byte) ((int) SiteVars.CFSFuelType[site] + 1); 
                    else
                        pixel.Band0 = 0;
                    newmap.WritePixel(pixel);
                }
            }

            IOutputRaster<ClassPixel> conmap = CreateConMap();
            string conpath = conmap.Path;
            using (conmap)
            {
                ClassPixel pixel = new ClassPixel();
                foreach (Site site in Model.Core.Landscape.AllSites)
                {
                    if (site.IsActive)
                        pixel.Band0 = (byte)((int)SiteVars.PercentConifer[site]);
                    else
                        pixel.Band0 = 0;
                    conmap.WritePixel(pixel);
                }
            }
            IOutputRaster<ClassPixel> firmap = CreateFirMap();
            string firpath = firmap.Path;
            using (firmap)
            {
                ClassPixel pixel = new ClassPixel();
                foreach (Site site in Model.Core.Landscape.AllSites)
                {
                    if (site.IsActive)
                        pixel.Band0 = (byte)((int)SiteVars.PercentDeadFir[site]);
                    else
                        pixel.Band0 = 0;
                    firmap.WritePixel(pixel);
                }
            }
        }

        //---------------------------------------------------------------------

        private IOutputRaster<ClassPixel> CreateMap()
        {
            string path = MapNames.ReplaceTemplateVars(mapNameTemplate, Model.Core.CurrentTime);
            UI.WriteLine("Writing Fuel map to {0} ...", path);
            return Model.Core.CreateRaster<ClassPixel>(path,
                                                    Model.Core.Landscape.Dimensions,
                                                    Model.Core.LandscapeMapMetadata);
        }

        //---------------------------------------------------------------------

        private IOutputRaster<ClassPixel> CreateConMap()
        {
            string conpath = MapNames.ReplaceTemplateVars(pctConiferMapNameTemplate, Model.Core.CurrentTime);
            UI.WriteLine("Writing % Conifer map to {0} ...", conpath);
            return Model.Core.CreateRaster<ClassPixel>(conpath,
                                                    Model.Core.Landscape.Dimensions,
                                                    Model.Core.LandscapeMapMetadata);
        }

        //---------------------------------------------------------------------
        
        private IOutputRaster<ClassPixel> CreateFirMap()
        {
            string firpath = MapNames.ReplaceTemplateVars(pctDeadFirMapNameTemplate, Model.Core.CurrentTime);
            UI.WriteLine("Writing % Dead Fir map to {0} ...", firpath);
            return Model.Core.CreateRaster<ClassPixel>(firpath,
                                                    Model.Core.Landscape.Dimensions,
                                                    Model.Core.LandscapeMapMetadata);
        }

        //---------------------------------------------------------------------

        private int CalcFuelType(Site site, 
                                        IEnumerable<IFuelType> FuelTypes, 
                                        IEnumerable<ISlashType> SlashTypes)
        {

            double[] forTypValue = new double[100];  //Maximum of 50 fuel types
            double sumConifer = 0.0;
            double sumDecid = 0.0;
            //double totalSppValue = 0.0;
            
            //UI.WriteLine("      Calculating species values...");
            Species.IDataset SpeciesDataset = Model.Core.Species;
            foreach(ISpecies species in SpeciesDataset)
            {
                /* ushort maxSpeciesAge = 0;
                
                double sppValue = 0.0;

                maxSpeciesAge = AgeCohort.Util.GetMaxAge(cohorts[site][species]);

                if(maxSpeciesAge > 0)
                {
                    sppValue = (double) maxSpeciesAge /
                        (double) species.Longevity * 
                        fuelCoefs[species.Index];
                        
                    //if(coniferIndex[species.Index])
                    //    sumConifer += sppValue;
                                
                    //if(decidIndex[species.Index])
                    //    sumDecid += sppValue;

                    //UI.WriteLine("      accumulating species values...");
                    foreach(IFuelType ftype in FuelTypes)
                    {
                        if(maxSpeciesAge >= ftype.MinAge && maxSpeciesAge <= ftype.MaxAge && sppValue > 0)
                        {
                            if(ftype[species.Index] != 0)
                            {
                                if(ftype[species.Index] == -1)
                                    forTypValue[ftype.FuelIndex] -= sppValue;
                                if(ftype[species.Index] == 1)
                                    forTypValue[ftype.FuelIndex] += sppValue;
                             }
                             
                                
                        }
                    }
                }*/
                
              
                ISpeciesCohorts speciesCohorts = cohorts[site][species];
                
                if(speciesCohorts == null)
                    continue;

                foreach(IFuelType ftype in FuelTypes)
                {

                    if(ftype[species.Index] != 0)
                    {
                        double sppValue = 0.0;

                        foreach(ICohort cohort in speciesCohorts)
                        {
                            double cohortValue =0.0;
                            
                        
                            if(cohort.Age >= ftype.MinAge && cohort.Age <= ftype.MaxAge)
                            {
                                // Adjust max range age to the spp longevity
                                double maxAge = System.Math.Min(ftype.MaxAge, (double) species.Longevity);
                                
                                // The fuel type range must be at least 5 years:
                                double ftypeRange = System.Math.Max(1.0, maxAge - (double) ftype.MinAge);
                                
                                // The cohort age relative to the fuel type range:
                                double relativeCohortAge = System.Math.Max(1.0, (double) cohort.Age - ftype.MinAge);
                                
                                cohortValue = relativeCohortAge / ftypeRange * fuelCoefs[species.Index];
                                
                                // Add a weight, dependent upon the size of the range.  Smaller ranges = smaller 
                                // weight because cohorts will more quickly advance to the end of small ranges.
                                //cohortValue *= ftypeRange / 100.0;
                            
                                sppValue += System.Math.Max(sppValue, cohortValue);
                            }
                        }
                        
                        if(ftype[species.Index] == -1)
                            forTypValue[ftype.FuelIndex] -= sppValue;
                        if(ftype[species.Index] == 1)
                            forTypValue[ftype.FuelIndex] += sppValue;
                    }
                } 
                
            }
            
            
            
            //UI.WriteLine("      Determining CFS fuel type...");
            int finalFuelType = 0;
            int decidFuelType = 0;
            int coniferFuelType = 0;
            int openFuelType = 0;
            int slashFuelType = 0;
            double maxValue = 0.0;
            double maxDecidValue = 0.0;
            double maxConiferValue = 0.0;
            double maxConPlantValue = 0.0;
            double maxOpenValue = 0.0;
            double maxSlashValue = 0.0;

            //Set the PERCENT CONIFER DOMINANCE:
            int coniferDominance = 0;
            int hardwoodDominance = 0;

            
            //First assign the CONIFER and DECIDUOUS types:
            foreach(IFuelType ftype in FuelTypes)
            {
                if(ftype != null)
                {

                    if ((ftype.BaseFuel == BaseFuelType.Conifer || ftype.BaseFuel == BaseFuelType.ConiferPlantation) 
                        && forTypValue[ftype.FuelIndex] > 0)
                    {
                        sumConifer += forTypValue[ftype.FuelIndex];
                        //maxOtherValue = forTypValue[ftype.FuelIndex];
                        //otherFuelType = ftype.FuelIndex;
                    }
                    
                    //This is calculated for the mixed types:
                    if ((ftype.BaseFuel == BaseFuelType.Deciduous) 
                        && forTypValue[ftype.FuelIndex] > 0)
                    {
                        sumDecid += forTypValue[ftype.FuelIndex];
                        //maxDecidValue = forTypValue[ftype.FuelIndex];
                        //decidFuelType = ftype.FuelIndex;
                    }

                    // CONIFER
                    if(forTypValue[ftype.FuelIndex] > maxConiferValue && ftype.BaseFuel == BaseFuelType.Conifer)
                    {
                        
                        maxConiferValue = forTypValue[ftype.FuelIndex];
                        if(maxConiferValue > maxConPlantValue)
                        coniferFuelType = ftype.FuelIndex;
                    }
                    // CONIFER PLANTATION
                    if (forTypValue[ftype.FuelIndex] > maxConPlantValue && ftype.BaseFuel == BaseFuelType.ConiferPlantation)
                    {

                        maxConPlantValue = forTypValue[ftype.FuelIndex];
                        if(maxConPlantValue > maxConiferValue)
                        coniferFuelType = ftype.FuelIndex;
                    }

                    // OPEN
                    if (forTypValue[ftype.FuelIndex] > maxOpenValue && ftype.BaseFuel == BaseFuelType.Open)
                    {

                        maxOpenValue = forTypValue[ftype.FuelIndex];
                        openFuelType = ftype.FuelIndex;
                    }
                    
                    // SLASH
                    if (forTypValue[ftype.FuelIndex] > maxSlashValue && ftype.BaseFuel == BaseFuelType.Slash)
                    {

                        maxSlashValue = forTypValue[ftype.FuelIndex];
                        slashFuelType = ftype.FuelIndex;
                    }

                    // DECIDUOUS
                    if(forTypValue[ftype.FuelIndex] > maxDecidValue && ftype.BaseFuel == BaseFuelType.Deciduous)
                    {
                        
                        maxDecidValue = forTypValue[ftype.FuelIndex];
                        decidFuelType = ftype.FuelIndex;
                    }

                }
            }

            if (maxConiferValue >= maxConPlantValue && maxConiferValue >= maxDecidValue && maxConiferValue >= maxOpenValue && maxConiferValue >= maxSlashValue)
            {
                maxValue = maxConiferValue;
            }
            else if (maxDecidValue >= maxConiferValue && maxDecidValue >= maxConPlantValue && maxDecidValue >= maxOpenValue && maxDecidValue >= maxSlashValue)
            {
                maxValue = maxDecidValue;
            }
            else if (maxConPlantValue >= maxConiferValue && maxConPlantValue >= maxDecidValue && maxConPlantValue >= maxOpenValue && maxConPlantValue >= maxSlashValue)
            {
                maxValue = maxConPlantValue;
                finalFuelType = coniferFuelType;
                decidFuelType = 0;
                sumConifer = 100;
                sumDecid = 0;
            }
            else if (maxSlashValue >= maxConiferValue && maxSlashValue >= maxConPlantValue && maxSlashValue >= maxDecidValue && maxSlashValue >= maxOpenValue)
            {
                maxValue = maxSlashValue;
                finalFuelType = slashFuelType;
                decidFuelType = 0;
                sumConifer = 0;
                sumDecid = 0;
            }
            else if (maxOpenValue >= maxConiferValue && maxOpenValue >= maxConPlantValue && maxOpenValue >= maxDecidValue && maxOpenValue >= maxSlashValue)
            {
                maxValue = maxOpenValue;
                finalFuelType = openFuelType;
                decidFuelType = 0;
                sumConifer = 0;
                sumDecid = 0;
            }
            
            
            //Set the PERCENT DOMINANCE values:
            if (sumConifer > 0 || sumDecid > 0)
            {
                coniferDominance = (int)((sumConifer / (sumConifer + sumDecid) * 100) + 0.5);
                hardwoodDominance = (int)((sumDecid / (sumConifer + sumDecid) * 100) + 0.5);
                if (hardwoodDominance < hardwoodMax)
                {
                    coniferDominance = 100;
                    hardwoodDominance = 0;
                    finalFuelType = coniferFuelType;
                    decidFuelType = 0;
                }
                if (coniferDominance < hardwoodMax)
                {
                    coniferDominance = 0;
                    hardwoodDominance = 100;
                    finalFuelType = decidFuelType;
                    decidFuelType = 0;
                }
                if (hardwoodDominance > hardwoodMax && coniferDominance > hardwoodMax)
                    finalFuelType = coniferFuelType;
            }
            

            
            //---------------------------------------------------------------------
            // Next check the disturbance types.  This will override any other existing fuel type.
            foreach(SlashType slash in SlashTypes)
            {
                if (SiteVars.HarvestCohortsKilled != null && SiteVars.HarvestCohortsKilled[site] > 0)
                {
                    if (SiteVars.TimeOfLastHarvest != null &&
                        (Model.Core.CurrentTime - SiteVars.TimeOfLastHarvest[site] <= slash.MaxAge))
                    {
                        foreach (string pName in slash.PrescriptionNames)
                        {
                            if (SiteVars.HarvestPrescriptionName != null && SiteVars.HarvestPrescriptionName[site].Trim() == pName.Trim())
                            {
                                finalFuelType = slash.FuelIndex; //Name;
                                decidFuelType = 0;
                                coniferDominance = 0;
                                hardwoodDominance = 0;
                            }
                        }
                    }
                }
                //Check for fire severity effects of fuel type
                if (SiteVars.FireSeverity != null && SiteVars.FireSeverity[site] > 0)
                {
                    if (SiteVars.TimeOfLastFire != null &&
                        (Model.Core.CurrentTime - SiteVars.TimeOfLastFire[site] <= slash.MaxAge))
                    {
                        foreach (string pName in slash.PrescriptionNames)
                        {
                            if (pName.StartsWith("FireSeverity"))
                            {
                                if((pName.Substring((pName.Length - 1), 1)).ToString() == SiteVars.FireSeverity[site].ToString())
                                {
                                    finalFuelType = slash.FuelIndex; //Name;
                                    decidFuelType = 0;
                                    coniferDominance = 0;
                                    hardwoodDominance = 0;
                                }
                            }
                        }
                    }
                }
                //Check for wind severity effects of fuel type
                if (SiteVars.WindSeverity != null && SiteVars.WindSeverity[site] > 0)
                {
                    if (SiteVars.TimeOfLastWind != null &&
                        (Model.Core.CurrentTime - SiteVars.TimeOfLastWind[site] <= slash.MaxAge))
                    {
                        foreach (string pName in slash.PrescriptionNames)
                        {
                            if (pName.StartsWith("WindSeverity"))
                            {
                                if ((pName.Substring((pName.Length - 1), 1)).ToString() == SiteVars.WindSeverity[site].ToString())
                                {
                                    finalFuelType = slash.FuelIndex; //Name;
                                    decidFuelType = 0;
                                    coniferDominance = 0;
                                    hardwoodDominance = 0;
                                }
                            }
                        }
                    }
                }
            }

            
            /*
            //If NEITHER conifer nor deciduous, then set those dominances values to zero:
            foreach(IFuelType ftype in FuelTypes)
                if (ftype.FuelIndex == finalFuelType && ftype.BaseFuel != BaseFuelType.Conifer 
                    && ftype.BaseFuel != BaseFuelType.ConiferPlantation 
                    && ftype.BaseFuel != BaseFuelType.Deciduous)
                {
                    coniferDominance = 0;
                    hardwoodDominance = 0;
                }
            */
            
            //if(decidFuelType == 0) hardwoodDominance = 0;
            
            //Assign Percent Conifer:
            SiteVars.PercentConifer[site] = coniferDominance;
            SiteVars.PercentHardwood[site] = hardwoodDominance;

            SiteVars.CFSFuelType[site] = finalFuelType;
            SiteVars.DecidFuelType[site] = decidFuelType;
            
            return finalFuelType;

        }
        
        private int CalcPercentDeadFir(ActiveSite site)
        {
        
            int numDeadFir = 0;
        
            if(SiteVars.NumberDeadFirCohorts == null)
            {
                //UI.WriteLine("No BDA data registered properly");
                return 0;
            }
            
            
            int[] deadfirs = SiteVars.NumberDeadFirCohorts[site];
            
            int minimumStartTime = Math.Max(0, SiteVars.TimeOfLastFire[site]);
            
            for(int i = minimumStartTime; i <= Model.Core.CurrentTime; i++)
            {
                if(Model.Core.CurrentTime - i <= deadFirMaxAge)
                {
                    numDeadFir += deadfirs[i];
                    //if (numDeadFir>0) UI.WriteLine("Num Dead Fir = {0}, i={1}.", numDeadFir,i);
                }
            }
            
            int numSiteCohorts = 0;
            int percentDeadFir = 0;
            
            Species.IDataset SpeciesDataset = Model.Core.Species;
            
            ILandscapeCohorts cohorts = Model.Core.SuccessionCohorts as ILandscapeCohorts;
            if (cohorts == null)
                throw new ApplicationException("Error: Cohorts don't support age-cohort interface");

            foreach (ISpeciesCohorts speciesCohorts in cohorts[site])
                foreach (ICohort cohort in speciesCohorts)
                    numSiteCohorts++;

            
            if(numDeadFir > 0)
            percentDeadFir = (int) ( (double) numDeadFir / ((double) (numSiteCohorts + numDeadFir)) * 100.0 + 0.5);
            
            
            return System.Math.Min(percentDeadFir, 100);
        }

    }
}
