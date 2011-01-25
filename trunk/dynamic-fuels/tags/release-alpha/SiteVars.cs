//  Copyright 2006 University of Wisconsin
//  Authors:  
//      Robert M. Scheller
//      James B. Domingo
//  License:  Available at  
//  http://landis.forest.wisc.edu/developers/LANDIS-IISourceCodeLicenseAgreement.pdf

using Landis.AgeCohort;
using Landis.Landscape;
using System.Collections.Generic;

namespace Landis.Fuels
{

    ///<summary>
    /// Site Variables for a fuels plug-in.
    /// </summary>
    public static class SiteVars
    {
        private static ISiteVar<int> cfsFuelType;
        private static ISiteVar<int> decidFuelType;
        private static ISiteVar<int> percentConifer;
        private static ISiteVar<int> percentHardwood;
        private static ISiteVar<int> percentDeadFir;
        private static ISiteVar<string> harvestPrescriptionName;
        private static ISiteVar<int> timeOfLastHarvest;
        private static ISiteVar<int> harvestCohortsKilled;
        private static ISiteVar<int> timeOfLastFire;
        private static ISiteVar<byte> fireSeverity;
        private static ISiteVar<int> timeOfLastWind;
        private static ISiteVar<byte> windSeverity; 
        private static ISiteVar<int[]> numberDeadFirCohorts;
        //private static ISiteVar<int> timeOfLastBDA;

        //---------------------------------------------------------------------

        public static void Initialize()
        {

            cfsFuelType     = Model.Core.Landscape.NewSiteVar<int>();
            decidFuelType   = Model.Core.Landscape.NewSiteVar<int>();
            percentConifer  = Model.Core.Landscape.NewSiteVar<int>();
            percentHardwood = Model.Core.Landscape.NewSiteVar<int>();
            percentDeadFir  = Model.Core.Landscape.NewSiteVar<int>();
            
            harvestPrescriptionName = Model.Core.GetSiteVar<string>("Harvest.PrescriptionName");
            timeOfLastHarvest       = Model.Core.GetSiteVar<int>("Harvest.TimeOfLastEvent");
            harvestCohortsKilled    = Model.Core.GetSiteVar<int>("Harvest.CohortsKilled");
            timeOfLastFire          = Model.Core.GetSiteVar<int>("Fire.TimeOfLastEvent");
            fireSeverity            = Model.Core.GetSiteVar<byte>("Fire.Severity");
            timeOfLastWind          = Model.Core.GetSiteVar<int>("Wind.TimeOfLastEvent");
            windSeverity            = Model.Core.GetSiteVar<byte>("Wind.Severity");
            numberDeadFirCohorts    = Model.Core.GetSiteVar<int[]>("BDA.NumCFSConifers");
            //timeOfLastBDA           = Model.Core.GetSiteVar<int>("BDA.TimeOfLastEvent");

            Model.Core.RegisterSiteVar(SiteVars.CFSFuelType, "Fuels.CFSFuelType");
            Model.Core.RegisterSiteVar(SiteVars.DecidFuelType, "Fuels.DecidFuelType");
            Model.Core.RegisterSiteVar(SiteVars.PercentConifer, "Fuels.PercentConifer");
            Model.Core.RegisterSiteVar(SiteVars.PercentHardwood, "Fuels.PercentHardwood");
            Model.Core.RegisterSiteVar(SiteVars.PercentDeadFir, "Fuels.PercentDeadFir");
        }

        //---------------------------------------------------------------------

        public static ISiteVar<int> CFSFuelType
        {
            get {
                return cfsFuelType;
            }
            set {
                cfsFuelType = value;
            }
        }
        //---------------------------------------------------------------------

        public static ISiteVar<int> DecidFuelType
        {
            get {
                return decidFuelType;
            }
            set {
                decidFuelType = value;
            }
        }
        //---------------------------------------------------------------------

        public static ISiteVar<int> PercentConifer
        {
            get {
                return percentConifer;
            }
        }

        //---------------------------------------------------------------------

        public static ISiteVar<int> PercentHardwood
        {
            get {
                return percentHardwood;
            }
        }
        //---------------------------------------------------------------------

        public static ISiteVar<int> PercentDeadFir
        {
            get {
                return percentDeadFir;
            }
        }
        //---------------------------------------------------------------------

        public static ISiteVar<string> HarvestPrescriptionName
        {
            get {
                return harvestPrescriptionName;
            }
        }
        //---------------------------------------------------------------------

        public static ISiteVar<int> TimeOfLastHarvest
        {
            get {
                return timeOfLastHarvest;
            }
        }
        //---------------------------------------------------------------------

        public static ISiteVar<int> HarvestCohortsKilled
        {
            get {
                return harvestCohortsKilled;
            }
        }
        //---------------------------------------------------------------------
        public static ISiteVar<int> TimeOfLastFire
        {
            get
            {
                return timeOfLastFire;
            }
        }
        //---------------------------------------------------------------------

        public static ISiteVar<byte> FireSeverity
        {
            get
            {
                return fireSeverity;
            }
        }
        //---------------------------------------------------------------------
        public static ISiteVar<int> TimeOfLastWind
        {
            get
            {
                return timeOfLastWind;
            }
        }
        //---------------------------------------------------------------------

        public static ISiteVar<byte> WindSeverity
        {
            get
            {
                return windSeverity;
            }
        }
        //---------------------------------------------------------------------
        /*
        public static ISiteVar<int> TimeOfLastBDA
        {
            get {
                return timeOfLastBDA;
            }
        }*/
        //---------------------------------------------------------------------

        public static ISiteVar<int[]> NumberDeadFirCohorts
        {
            get {
                return numberDeadFirCohorts;
            }
        }
    }
}
