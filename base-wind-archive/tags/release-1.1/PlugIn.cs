//  Copyright 2005 University of Wisconsin
//  Authors:  Jimm Domingo, Robert M. Scheller
//  License:  Available at  
//  http://landis.forest.wisc.edu/developers/LANDIS-IISourceCodeLicenseAgreement.pdf

using Landis.AgeCohort;
using Landis.Landscape;
using Landis.RasterIO;
using Landis.PlugIns;
using Landis.Util;
using Landis.Ecoregions;
using System.Collections.Generic;
using System.IO;
using System;

namespace Landis.Wind
{
    ///<summary>
    /// A disturbance plug-in that simulates wind disturbance.
    /// </summary>

    public class PlugIn
        : PlugIns.PlugIn
    {
        public static readonly PlugInType Type = new PlugInType("disturbance:wind");
        private ILandscapeCohorts cohorts;
        private string mapNameTemplate;
        private StreamWriter log;

        //---------------------------------------------------------------------

        public PlugIn()
            : base("Base Wind", Type)
        {
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes the plug-in with a data file.
        /// </summary>
        /// <param name="dataFile">
        /// Path to the file with initialization data.
        /// </param>
        /// <param name="startTime">
        /// Initial timestep (year): the timestep that will be passed to the
        /// first call to the component's Run method.
        /// </param>
        public override void Initialize(string dataFile,
                                        PlugIns.ICore modelCore)
        {
            Model.Core = modelCore;

            ParameterParser.EcoregionsDataset = Model.Core.Ecoregions;
            ParameterParser parser = new ParameterParser();
            IParameters parameters = Data.Load<IParameters>(dataFile, parser);

            Timestep = parameters.Timestep;
            mapNameTemplate = parameters.MapNamesTemplate;

            cohorts = Model.Core.SuccessionCohorts as ILandscapeCohorts;
            if (cohorts == null)
                throw new ApplicationException("Error: Cohorts don't support age-cohort interface");

            SiteVars.Initialize();
            Model.Core.RegisterSiteVar(SiteVars.TimeOfLastEvent, "Wind.TimeOfLastEvent");
            Event.Initialize(parameters.EventParameters,
                             parameters.WindSeverities);

            UI.WriteLine("Opening wind log file \"{0}\" ...", parameters.LogFileName);
            log = Data.CreateTextFile(parameters.LogFileName);
            log.AutoFlush = true;
            log.WriteLine("Time,Initiation Site,Total Sites,Damaged Sites,Cohorts Killed,Mean Severity");
        }

        //---------------------------------------------------------------------

        ///<summary>
        /// Run the plug-in at a particular timestep.
        ///</summary>
        public override void Run()
        {
            UI.WriteLine("Processing landscape for wind events ...");

            SiteVars.Event.SiteValues = null;
            SiteVars.Severity.ActiveSiteValues = 0;

            int eventCount = 0;
            foreach (ActiveSite site in Model.Core.Landscape) {
                Event windEvent = Event.Initiate(site, Timestep);
                if (windEvent != null) {
                    LogEvent(Model.Core.CurrentTime, windEvent);
                    eventCount++;
                }
            }
            UI.WriteLine("  Wind events: {0}", eventCount);

            //  Write wind severity map
            IOutputRaster<SeverityPixel> map = CreateMap(Model.Core.CurrentTime);
            using (map) {
                SeverityPixel pixel = new SeverityPixel();
                foreach (Site site in Model.Core.Landscape.AllSites) {
                    if (site.IsActive) {
                        if (SiteVars.Disturbed[site])
                            pixel.Band0 = (byte) (SiteVars.Severity[site] + 1);
                        else
                            pixel.Band0 = 1;
                    }
                    else {
                        //  Inactive site
                        pixel.Band0 = 0;
                    }
                    map.WritePixel(pixel);
                }
            }
        }

        //---------------------------------------------------------------------

        private void LogEvent(int   currentTime,
                              Event windEvent)
        {
            log.WriteLine("{0},\"{1}\",{2},{3},{4},{5:0.0}",
                          currentTime,
                          windEvent.StartLocation,
                          windEvent.Size,
                          windEvent.SitesDamaged,
                          windEvent.CohortsKilled,
                          windEvent.Severity);
        }

        //---------------------------------------------------------------------

        private IOutputRaster<SeverityPixel> CreateMap(int currentTime)
        {
            string path = MapNames.ReplaceTemplateVars(mapNameTemplate, currentTime);
            UI.WriteLine("Writing wind severity map to {0} ...", path);
            return Model.Core.CreateRaster<SeverityPixel>(path,
                                                          Model.Core.Landscape.Dimensions,
                                                          Model.Core.LandscapeMapMetadata);
        }

        //---------------------------------------------------------------------

        public void CleanUp()
        {
            if (log != null) {
                log.Close();
                log = null;
            }
        }
    }
}
