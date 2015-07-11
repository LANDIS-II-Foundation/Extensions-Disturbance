//  Copyright 2006-2008 USFS Northern Research Station, Conservation Biology Institute, University of Wisconsin
//  Authors:  Robert M. Scheller, Brian R. Miranda
//  License:  Available at
//  http://www.landis-ii.org/developers/LANDIS-IISourceCodeLicenseAgreement.pdf

using Troschuetz.Random;
using System.Data;
using System;
using System.IO;
using System.Collections.Generic;

namespace Landis.Fire
{

    public class Weather
    {

        //---------------------------------------------------------------------

        public static ISeasonParameters GenerateSeason(ISeasonParameters[] seasons)
        {
            double randNum = Util.Random.GenerateUniform();
            double bottom = 0.0;
            double top = 0.0;
            foreach (ISeasonParameters season in seasons)
            {
                top += season.FireProbability;
                if(randNum >= bottom && randNum <= top)
                    return season;
                bottom += season.FireProbability;
            }
            return null;
        }

        //---------------------------------------------------------------------

        public static int GenerateWindSpeed(System.Data.DataSet weatherDS)
        {
            int windSpeed = 0;

            foreach (DataRow myDataRow in weatherDS.Tables["Table"].Rows)
            {

                windSpeed = (int) Math.Round((double)myDataRow["WSV"]);
                //UI.WriteLine("   New Event WSV:  {0}", windSpeed.ToString());
            }

            return windSpeed;
        }
         //---------------------------------------------------------------------

        public static int GenerateWindSpeed(System.Data.DataRow weatherRow)
        {
            int windSpeed = (int) Math.Round((double)weatherRow["WSV"]);;

            return windSpeed;
        }

        //---------------------------------------------------------------------

        public static int GenerateWindDirection(System.Data.DataRow weatherRow)
        {
            int windDir = (int) weatherRow["WindDir"];

            return windDir;
        }
        //---------------------------------------------------------------------

        public static int GenerateFineFuelMoistureCode(System.Data.DataSet weatherDS)
        {
            int FFMC = 0;

            foreach (DataRow myDataRow in weatherDS.Tables["Table"].Rows)
            {
                FFMC = (int) Math.Round((double)myDataRow["FFMC"]);
                //UI.WriteLine("   New Event FFMC:  {0}", FFMC.ToString());
            }

            return FFMC;
        }
        //---------------------------------------------------------------------

        public static int GenerateFineFuelMoistureCode(System.Data.DataRow weatherRow)
        {
            int FFMC = (int) Math.Round((double)weatherRow["FFMC"]);

            return FFMC;
        }
        //---------------------------------------------------------------------

        public static int GenerateBuildUpIndex(System.Data.DataSet weatherDS)
        {
            int BUI = 0;

            foreach (DataRow myDataRow in weatherDS.Tables["Table"].Rows)
            {
                BUI = (int)Math.Round((double)myDataRow["BUI"]);
                //UI.WriteLine("   New Event BUI:  {0}", BUI.ToString());
            }

            return  BUI;
        }
         //---------------------------------------------------------------------

        public static int GenerateBuildUpIndex(System.Data.DataRow weatherRow)
        {
            int BUI = (int) Math.Round((double)weatherRow["BUI"]);

            return  BUI;
        }
        //---------------------------------------------------------------------

        public static double CalculateFuelMoistureEffect(double FFMC)
        {
            // FBP 45 and 46:
            double f_F = 0.0;
            double m = (147.2 * (101 - FFMC)) / (59.5 + FFMC);

            f_F = 91.9 * System.Math.Exp(-0.1386 * m) *
                            (1 + (System.Math.Pow(m, 5.31) / 49300000));
            return f_F;
        }
        //---------------------------------------------------------------------
        public static double CalculateWindEffect(double WSV)
        {
            // FBP 53 and 53a:
            double f_W = 0.0;
            if(WSV  <= 40)  //0.06 = convert back to km/hr
                f_W = System.Math.Exp(0.05039 * WSV);
            else
                f_W = 12 * (1 - System.Math.Exp(-0.0818 * (WSV - 28)));
            return f_W;
        }
        //---------------------------------------------------------------------
        public static double CalculateBackWindEffect(double WSV)
        {
            // FBP 75:
            double f_W = System.Math.Exp(-1 * 0.05039 * WSV);
            return f_W;
        }
        //---------------------------------------------------------------------

        private static double GenerateRandomNum(Distribution dist, double parameter1, double parameter2)
        {
            double randomNum = 0.0;
            if(dist == Distribution.normal)
            {
                NormalDistribution randVar = new NormalDistribution(RandomNumberGenerator.Singleton);
                randVar.Mu = parameter1;      // mean
                randVar.Sigma = parameter2;   // std dev
                randomNum = randVar.NextDouble();
            }
            if(dist == Distribution.lognormal)
            {
                LognormalDistribution randVar = new LognormalDistribution(RandomNumberGenerator.Singleton);
                randVar.Mu = parameter1;      // mean
                randVar.Sigma = parameter2;   // std dev
                randomNum = randVar.NextDouble();
            }
            if(dist == Distribution.gamma)
            {
                GammaDistribution randVar = new GammaDistribution(RandomNumberGenerator.Singleton);
                randVar.Alpha = parameter1;      // mean
                randVar.Theta = parameter2;   // std dev
                randomNum = randVar.NextDouble();
            }
            if(dist == Distribution.Weibull)
            {
                WeibullDistribution randVar = new WeibullDistribution(RandomNumberGenerator.Singleton);
                randVar.Alpha = parameter1;      // mean
                randVar.Lambda = parameter2;   // std dev
                randomNum = randVar.NextDouble();
            }
            return randomNum;
        }
        //---------------------------------------------------------------------

        public static DataRow GenerateDataRow(ISeasonParameters season, IFireRegion fire_region, int sizeBin)
        {

            int weatherRandomizer = PlugIn.WeatherRandomizer;
            string seasonName = season.NameOfSeason.ToString();
            string ecoName = fire_region.Name;
            int weatherBin = 0;

            if(weatherRandomizer == 0)
                weatherBin = sizeBin;
            else
            {
                // First, tally the available bins and assign a probability based on their size
                // (number of records).
                // Bins can only be 1 - 5.

                int minBin = (int) System.Math.Max(1, sizeBin - weatherRandomizer);
                int maxBin = (int) System.Math.Min(5, sizeBin + weatherRandomizer);

                double[] binProbability = new double[maxBin - minBin + 1];
                int[] binCount = new int[maxBin - minBin + 1];
                int binSum = 0;

                for(int bin = minBin; bin <= maxBin; bin++)
                {
                    string selectString = "FWIBin = '" + bin + "' AND Season = '" + seasonName + "' AND Ecoregion = '" + ecoName + "'";
                    DataRow[] rows = PlugIn.WeatherDataTable.Select(selectString);
                    if (rows.Length == 0)
                    {
                        selectString = "FWIBin = '" + bin + "' AND Season = '" + seasonName + "' AND Ecoregion = 'All'";
                        rows = PlugIn.WeatherDataTable.Select(selectString);
                    }

                    int numRecs = rows.Length;

                    binCount[bin - minBin] = numRecs;
                    binSum += numRecs;
                }

                for(int bin = minBin; bin <= maxBin; bin++)
                    binProbability[bin-minBin] = (double) binCount[bin-minBin] / (double) binSum;

                // Now randomly select from the available bins:
                double randomBinNum = Util.Random.GenerateUniform();

                double minProb = 0.0;
                double maxProb = 0.0;

                for(int bin = minBin; bin <= maxBin; bin++)
                {
                    maxProb += binProbability[bin-minBin];
                    if(randomBinNum >= minProb && randomBinNum < maxProb)
                    {
                        weatherBin = bin;
                        break;
                    }
                    else
                    {
                        minProb = binProbability[bin-minBin];
                    }
                }

            }
            if(weatherBin == 0)
                weatherBin = sizeBin;
                //throw new System.ApplicationException("No Weather Bin randomly selected. FireRegion = "+ecoName+", Season = "+seasonName+", sizeBin = "+sizeBin);

            int rowCount = 0;
            int loopCount = 0;
            int firstDir = 0;
            DataRow[] foundRows = null;

            if (Util.Random.GenerateUniform() >= 0.5)
                firstDir = 1;  // Direction (+ or -) to check first if target bin is not available (+1 or -1)
            else
                firstDir = -1;
            while (rowCount == 0)
            {
                string selectString = "FWIBin = '" + weatherBin + "' AND Season = '" + seasonName + "' AND Ecoregion = '" + ecoName + "'";
                foundRows = PlugIn.WeatherDataTable.Select(selectString);
                rowCount = foundRows.Length;

                if (rowCount == 0)
                {
                    selectString = "FWIBin = '" + weatherBin + "' AND Season = '" + seasonName + "' AND Ecoregion = 'All'";
                    foundRows = PlugIn.WeatherDataTable.Select(selectString);
                    rowCount = foundRows.Length;
                }

                if (rowCount == 0)
                {
                    //UI.WriteLine("   weatherBin "+weatherBin+" Not Found.  Using alternate weatherBin.");
                    if (sizeBin == 5)
                        weatherBin = weatherBin - 1;
                    else if (sizeBin == 1)
                        weatherBin = weatherBin + 1;
                    else
                    {
                        if (loopCount == 0)
                            weatherBin = sizeBin + firstDir;
                        else if (loopCount == 1)
                            weatherBin = sizeBin - firstDir;
                        else if (loopCount == 2)
                            weatherBin = sizeBin + (firstDir * 2);
                        else if (loopCount == 3)
                            weatherBin = sizeBin - (firstDir * 2);
                        else if (loopCount == 4)
                            weatherBin = sizeBin + (firstDir * 3);
                        else if (loopCount == 5)
                            weatherBin = sizeBin - (firstDir * 3);
                        else if (loopCount == 6)
                            weatherBin = sizeBin + (firstDir * 4);
                        else
                            weatherBin = sizeBin - (firstDir * 4);
                    }
                    loopCount ++;
                    if (loopCount > 100)
                    {

                        UI.WriteLine("   No Weather Rows Selected");
                        throw new System.ApplicationException("No Weather Row could be selected. Ecoregion = "+ecoName+", Season = "+seasonName+", sizeBin = "+sizeBin);

                    }

                }
            }
            int newRandNum = (int)(Math.Round(Util.Random.GenerateUniform() * (rowCount - 1)));
            DataRow weatherRow = foundRows[newRandNum];

            return weatherRow;
        }
        //---------------------------------------------------------------------

        public static int GenerateFMC(ISeasonParameters season, IFireRegion fire_region)
        {

            int FMC = 0;
            if (season.NameOfSeason == SeasonName.Spring)
            {
                if (Util.Random.GenerateUniform() < fire_region.SpringFMCHiProp)
                    FMC = fire_region.SpringFMCHi;
                else
                    FMC = fire_region.SpringFMCLo;
            }
            if (season.NameOfSeason == SeasonName.Summer)
            {
                if (Util.Random.GenerateUniform() < fire_region.SummerFMCHiProp)
                    FMC = fire_region.SummerFMCHi;
                else
                    FMC = fire_region.SummerFMCLo;
            }
            if (season.NameOfSeason == SeasonName.Fall)
            {
                if (Util.Random.GenerateUniform() < fire_region.FallFMCHiProp)
                    FMC = fire_region.FallFMCHi;
                else
                    FMC = fire_region.FallFMCLo;
            }
            return FMC;
        }
        //---------------------------------------------------------------------

        public static DataTable ReadWeatherFile(string path, List<IFireRegion> ecoDataSet, ISeasonParameters[] seasonParms)
        {
            UI.WriteLine("   Dynamic Fire: Loading Weather Data...");

            CSVParser weatherParser = new CSVParser();

            DataTable weatherTable = weatherParser.ParseToDataTable(path);

            int recordCount = 0;
            for (int i = 0; i <= 2; i++)
            {
                string seasName = seasonParms[i].NameOfSeason.ToString();


                foreach (IFireRegion fire_region in ecoDataSet)
                {
                    string ecoName = fire_region.Name;
                    //UI.WriteLine("Read Weather File:  Season={0}, FireRegion={1}.", seasName, ecoName);

                    string selectText = ("Ecoregion = '" + ecoName + "' AND Season = '" + seasName + "'");
                    //UI.WriteLine("Read Weather File SelectText = {0}.", selectText);

                    DataRow[] foundRows = weatherTable.Select(selectText);

                    if (foundRows.Length == 0)
                    {
                        selectText = ("Ecoregion = 'All' AND Season = '" + seasName + "'");
                        foundRows = weatherTable.Select(selectText);
                    }

                    if(foundRows.Length > 0)
                    {
                        //Input validation
                        double WSV, FFMC, BUI;
                        int WINDDIR, FWIBIN;
                        for(int j = 0; j < foundRows.Length; j ++) //weatherDataSet.Tables["Table"].Rows)
                        {
                            DataRow myDataRow = foundRows[j];

                            WSV = (double) myDataRow["WSV"];
                            //Console.WriteLine("WSV:  {0}", WSV);
                            if (WSV < 0.0)
                            {
                                throw new System.ApplicationException("Error: Wind Speed < 0:  FireRegion = " + ecoName + "; Season = " + seasName);
                            }
                            FFMC = (double) myDataRow["FFMC"];
                            //Console.WriteLine("FFMC:  {0}", FFMC);
                            if (FFMC < 0.0)
                            {
                                throw new System.ApplicationException("Error: FFMC < 0:  FireRegion = " + ecoName + "; Season = " + seasName);
                            }
                            else if (FFMC > 100.0)
                            {
                                throw new System.ApplicationException("Error: FFMC > 100:  FireRegion = " + ecoName + "; Season = " + seasName);
                            }
                            BUI = (double) myDataRow["BUI"];
                            //Console.WriteLine("BUI:  {0}", BUI);
                            if (BUI < 0.0)
                            {
                                throw new System.ApplicationException("Error: BUI < 0:  FireRegion = " + ecoName + "; Season = " + seasName);
                            }
                            WINDDIR = (int) myDataRow["WindDir"];
                            //Console.WriteLine("WindDir:  {0}", WINDDIR);
                            if (WINDDIR < 0)
                            {
                                throw new System.ApplicationException("Error: WINDDIR < 0:  FireRegion = " + ecoName + "; Season = " + seasName);
                            }
                            else if (WINDDIR > 360)
                            {
                                throw new System.ApplicationException("Error: WINDDIR > 360:  FireRegion = " + ecoName + "; Season = " + seasName);
                            }
                            FWIBIN = (int) myDataRow["FWIBin"];
                            //Console.WriteLine("FWIBIN:  {0}", FWIBIN);
                            if (FWIBIN < 1)
                            {
                                throw new System.ApplicationException("Error: FWIBIN < 1:  FireRegion = " + ecoName + "; Season = " + seasName);
                            }
                            else if (FWIBIN > 5)
                            {
                                throw new System.ApplicationException("Error: FWIBIN > 5:  FireRegion = " + ecoName + "; Season = " + seasName);
                            }
                        }

                    }

                    if ((foundRows.Length == 0) && (seasonParms[i].FireProbability > 0) && (fire_region.EcoIgnitionNum > 0))
                    {
                        throw new System.ApplicationException("Error: Ecoregion " + ecoName + ", Season " + seasName + " has fire probability > 0, but 0 weather records");
                    }

                    if(seasName == "Fall")
                    {
                    fire_region.FallRecords = recordCount;
                    }
                    else if (seasName == "Spring")
                    {
                        fire_region.SpringRecords = recordCount;
                    }
                    else if (seasName == "Summer")
                    {
                        fire_region.SummerRecords = recordCount;
                    }
                }
            }

            return weatherTable;
        }
    }
}
