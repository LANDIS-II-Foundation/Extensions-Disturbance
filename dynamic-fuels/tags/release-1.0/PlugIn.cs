//  Copyright 2007-2009 Portland State University, USFS Northern Research Station, University of Wisconsin
//  Authors:
//      Robert M. Scheller
//      Brian R. Miranda
//  License:  Available at
//  http://www.landis-ii.org/developers/LANDIS-IISourceCodeLicenseAgreement.pdf

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
        public static readonly PlugInType Type = new PlugInType("disturbance:fuels");
        private string mapNameTemplate;
        private string pctConiferMapNameTemplate;
        private string pctDeadFirMapNameTemplate;
        private IEnumerable<IFuelType> fuelTypes;
        private IEnumerable<IDisturbanceType> disturbanceTypes;
        private ILandscapeCohorts cohorts;
        private double[] fuelCoefs;
        private int hardwoodMax;
        private int deadFirMaxAge;


        //---------------------------------------------------------------------

        public PlugIn()
            : base("Dynamic Fuel System", Type)
        {
        }

        //---------------------------------------------------------------------

        public override void Initialize(string        dataFile,
                                        PlugIns.ICore modelCore)
        {
            Model.Core = modelCore;

            InputParametersParser.SpeciesDataset = Model.Core.Species;
            InputParametersParser parser = new InputParametersParser();
            IInputParameters parameters = Data.Load<IInputParameters>(dataFile, parser);

            if(parameters == null)
                UI.WriteLine("Parameters are not loading.");


            Timestep = parameters.Timestep;
            mapNameTemplate = parameters.MapFileNames;
            pctConiferMapNameTemplate = parameters.PctConiferFileName;
            pctDeadFirMapNameTemplate = parameters.PctDeadFirFileName;
            fuelTypes = parameters.FuelTypes;
            disturbanceTypes = parameters.DisturbanceTypes;
            fuelCoefs = parameters.FuelCoefficients;
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
            if (SiteVars.TimeOfLastFire == null)
                SiteVars.ReInitialize();

            SiteVars.FuelType.ActiveSiteValues = 0;
            SiteVars.DecidFuelType.ActiveSiteValues = 0;

            UI.WriteLine("  Calculating the Dynamic Fuel Type for all active cells...");
            foreach (ActiveSite site in Model.Core.Landscape)
            {
                CalcFuelType(site, fuelTypes, disturbanceTypes);
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
                        pixel.Band0 = (byte) ((int) SiteVars.FuelType[site] + 1);
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
                                        IEnumerable<IDisturbanceType> DisturbanceTypes)
        {

            double[] forTypValue = new double[100];  //Maximum of 100 fuel types
            double sumConifer = 0.0;
            double sumDecid = 0.0;

            //UI.WriteLine("      Calculating species values...");
            Species.IDataset SpeciesDataset = Model.Core.Species;
            foreach(ISpecies species in SpeciesDataset)
            {
                /*ushort maxSpeciesAge = 0;
                double sppValue = 0.0;
                maxSpeciesAge = AgeCohort.Util.GetMaxAge(cohorts[site][species]);

                if(maxSpeciesAge > 0)
                {
                    sppValue = (double) maxSpeciesAge /
                        (double) species.Longevity *
                        (double) fuelCoefs[species.Index];

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
                } */

                // This is the new algorithm, based on where a cohort is within it's age range.
                // This algorithm is less biased towards older cohorts.
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

                                // Use the one cohort with the largest value:
                                //sppValue += System.Math.Max(sppValue, cohortValue);  // A BUG, should be...
                                sppValue = System.Math.Max(sppValue, cohortValue);
                            }
                        }

                        if(ftype[species.Index] == -1)
                            forTypValue[ftype.FuelIndex] -= sppValue;
                        if(ftype[species.Index] == 1)
                            forTypValue[ftype.FuelIndex] += sppValue;
                    }
                }

            }

            int finalFuelType = 0;
            int decidFuelType = 0;
            //int coniferFuelType = 0;
            //int openFuelType = 0;
            //int slashFuelType = 0;
            double maxValue = 0.0;
            double maxDecidValue = 0.0;
            //double maxConiferValue = 0.0;
            //double maxConPlantValue = 0.0;
            //double maxOpenValue = 0.0;
            //double maxSlashValue = 0.0;

            //Set the PERCENT CONIFER DOMINANCE:
            int coniferDominance = 0;
            int hardwoodDominance = 0;


            //First accumulate data for the BASE fuel types:
            foreach(IFuelType ftype in FuelTypes)
            {
                if(ftype != null)
                {

                    if ((ftype.BaseFuel == BaseFuelType.Conifer || ftype.BaseFuel == BaseFuelType.ConiferPlantation)
                        && forTypValue[ftype.FuelIndex] > 0)
                    {
                        sumConifer += forTypValue[ftype.FuelIndex];
                    }

                    //This is calculated for the mixed types:
                    if ((ftype.BaseFuel == BaseFuelType.Deciduous)
                        && forTypValue[ftype.FuelIndex] > 0)
                    {
                        sumDecid += forTypValue[ftype.FuelIndex];
                    }

                    if(forTypValue[ftype.FuelIndex] > maxValue)
                    {
                        maxValue = forTypValue[ftype.FuelIndex];
                        finalFuelType = ftype.FuelIndex;
                    }

                    if(ftype.BaseFuel == BaseFuelType.Deciduous && forTypValue[ftype.FuelIndex] > maxDecidValue)
                    {
                        maxDecidValue = forTypValue[ftype.FuelIndex];
                        decidFuelType = ftype.FuelIndex;
                    }

                }
            }

            // Next, use rules to modify the conifer and deciduous dominance:


            foreach(IFuelType ftype in FuelTypes)
            {
                if(ftype != null)
                {

                    if(ftype.FuelIndex == finalFuelType && ftype.BaseFuel == BaseFuelType.ConiferPlantation)
                    {
                        decidFuelType = 0;
                        sumConifer = 100;
                        sumDecid = 0;
                    }

                    // a SLASH type
                    else if(ftype.FuelIndex == finalFuelType && ftype.BaseFuel == BaseFuelType.Slash)
                    {
                        //maxValue = maxSlashValue;
                        //finalFuelType = slashFuelType;
                        //decidFuelType = 0;
                        sumConifer = 0;
                        sumDecid = 0;
                    }

            // an OPEN type
                    else if(ftype.FuelIndex == finalFuelType && ftype.BaseFuel == BaseFuelType.Open)
                    {
                        //maxValue = maxOpenValue;
                        //finalFuelType = openFuelType;
                        //decidFuelType = 0;
                        sumConifer = 0;
                        sumDecid = 0;
                    }

                }
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
                }
                if (coniferDominance < hardwoodMax)
                {
                    coniferDominance = 0;
                    hardwoodDominance = 100;
                    finalFuelType = decidFuelType;
                }
            }

            //---------------------------------------------------------------------
            // Next check the disturbance types.  This will override any other existing fuel type.
            foreach(DisturbanceType slash in DisturbanceTypes)
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

            //Assign Percent Conifer:
            SiteVars.PercentConifer[site] = coniferDominance;
            SiteVars.PercentHardwood[site] = hardwoodDominance;

            SiteVars.FuelType[site] = finalFuelType;
            SiteVars.DecidFuelType[site] = decidFuelType;

            return finalFuelType;

        }

        // If BDA is running, then use that information to calculate the percent of all cohorts
        // that are dead fir cohorts.
        private int CalcPercentDeadFir(ActiveSite site)
        {

            int numDeadFir = 0;

            if(SiteVars.NumberDeadFirCohorts == null) // Is BDA even running?
                return 0;

            int minimumStartTime = System.Math.Max(0, SiteVars.TimeOfLastFire[site]);
            for(int i = minimumStartTime; i <= Model.Core.CurrentTime; i++)
            {
                if(Model.Core.CurrentTime - i <= deadFirMaxAge)

                    //if(SiteVars.NumberDeadFirCohorts[site][i] > 0)  // Only if a map actually exists
                    //    numDeadFir += SiteVars.NumberDeadFirCohorts[site][i];
                    if(SiteVars.NumberDeadFirCohorts[site].ContainsKey(i))
                        numDeadFir += SiteVars.NumberDeadFirCohorts[site][i];
            }

            int numSiteCohorts = 0;
            int percentDeadFir = 0;

            Species.IDataset SpeciesDataset = Model.Core.Species;

            foreach (ISpeciesCohorts speciesCohorts in cohorts[site])
                foreach (ICohort cohort in speciesCohorts)
                    numSiteCohorts++;


            percentDeadFir = (int) ( ((double) numDeadFir / (double) (numSiteCohorts + numDeadFir)) * 100.0 + 0.5);


            return System.Math.Min(percentDeadFir, 100);
        }

    }
}
