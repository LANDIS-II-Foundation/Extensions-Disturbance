//  Copyright 2006-2011 University of Wisconsin, Portland State University
//  Authors:  Jane Foster, Robert M. Scheller

//using Landis.Extension.Succession.Biomass;
using Landis.Core;
using Landis.SpatialModeling;
using Landis.Library.LeafBiomassCohorts;
using System.Collections.Generic;

namespace Landis.Extension.Insects
{
    ///<summary>
    /// Site Variables for a disturbance plug-in that simulates Biological Agents.
    /// </summary>
    public static class SiteVars
    {
        private static ISiteVar<Outbreak> outbreakVariables;
        private static ISiteVar<int> timeOfLastEvent;
        private static ISiteVar<int> biomassRemoved;
        private static ISiteVar<double> initialOutbreakProb;
        private static ISiteVar<ISiteCohorts> cohorts;
        private static ISiteVar<int> cohortsPartiallyDamaged;
        private static ISiteVar<string> insectName; //AMK Brian M update
        private static ISiteVar<int> siteDefoliation; //Brian M update


        //---------------------------------------------------------------------

        public static void Initialize()
        {
            outbreakVariables       = PlugIn.ModelCore.Landscape.NewSiteVar<Outbreak>();
            timeOfLastEvent         = PlugIn.ModelCore.Landscape.NewSiteVar<int>();
            biomassRemoved          = PlugIn.ModelCore.Landscape.NewSiteVar<int>();
            initialOutbreakProb     = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            cohorts                 = PlugIn.ModelCore.GetSiteVar<ISiteCohorts>("Succession.LeafBiomassCohorts");
            cohortsPartiallyDamaged = PlugIn.ModelCore.Landscape.NewSiteVar<int>();
            insectName = PlugIn.ModelCore.Landscape.NewSiteVar<string>(); //AMK Brian M update
            siteDefoliation = PlugIn.ModelCore.Landscape.NewSiteVar<int>(); //Brian M update


            SiteVars.TimeOfLastEvent.ActiveSiteValues = -10000;
            SiteVars.InitialOutbreakProb.ActiveSiteValues = 0.0;
            SiteVars.InsectName.ActiveSiteValues = ""; //AMK Brian M update
            SiteVars.SiteDefoliation.ActiveSiteValues = 0;

            //Initialize outbreaks:
            foreach (ActiveSite site in PlugIn.ModelCore.Landscape)
            {
                SiteVars.OutbreakVars = null;
            }

            if (cohorts == null)
            {
                string mesg = string.Format("Cohorts are empty.  Please double-check that this extension is compatible with your chosen succession extension.");
                throw new System.ApplicationException(mesg);
            }

            PlugIn.ModelCore.RegisterSiteVar(SiteVars.TimeOfLastEvent, "BiomassInsects.TimeOfLastEvent");
            PlugIn.ModelCore.RegisterSiteVar(SiteVars.InsectName, "BiomassInsects.InsectName");
            PlugIn.ModelCore.RegisterSiteVar(SiteVars.SiteDefoliation, "BiomassInsects.PctDefoliation");

        }
        //---------------------------------------------------------------------

        public static ISiteVar<Outbreak> OutbreakVars
        {
            get {
                return outbreakVariables;
            }
            set {
                outbreakVariables = value;
            }

        }

        //---------------------------------------------------------------------

        public static ISiteVar<int> BiomassRemoved
        {
            get {
                return biomassRemoved;
            }
            set
            {
                biomassRemoved = value;
            }
        }

        //---------------------------------------------------------------------
        public static ISiteVar<double> InitialOutbreakProb
        {
            get {
                return initialOutbreakProb;
            }
        }
        //---------------------------------------------------------------------

        public static ISiteVar<int> CohortsPartiallyDamaged
        {
            get
            {
                return cohortsPartiallyDamaged;
            }
        }

        //---------------------------------------------------------------------
        public static ISiteVar<ISiteCohorts> Cohorts
        {
            get
            {
                return cohorts;
            }
        }

        //---------------------------------------------------------------------
        public static ISiteVar<int> TimeOfLastEvent
        {
            get {
                return timeOfLastEvent;
            }
        }
        //---------------------------------------------------------------------
        public static ISiteVar<string> InsectName  //AMK Brian M update
        {
            get
            {
                return insectName;
            }
        }
        //---------------------------------------------------------------------
        public static ISiteVar<int> SiteDefoliation  //Brian M update
        {
            get
            {
                return siteDefoliation;
            }
        }
        //---------------------------------------------------------------------
    }
}
