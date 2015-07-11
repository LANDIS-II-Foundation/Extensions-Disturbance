using Landis.Landscape;
using Landis.RasterIO;
using Landis.Util;
using System.Collections.Generic;
using System.IO;

namespace Landis.Wind
{
	///<summary>
	/// A disturbance plug-in that simulates wind disturbance.
	/// </summary>
	public class PlugIn
		: Landis.PlugIns.IDisturbance, Landis.PlugIns.ICleanUp
	{
		private int timestep;
		private int nextTimeToRun;
		private string mapNameTemplate;
		private StreamWriter log;

		//---------------------------------------------------------------------

		/// <summary>
		/// The name that users refer to the plug-in by.
		/// </summary>
		public string Name
		{
			get {
				return "Wind";
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The next timestep when the plug-in should run.
		/// </summary>
		public int NextTimeToRun
		{
			get {
				return nextTimeToRun;
			}
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
		public void Initialize(string dataFile,
		                       int    startTime)
		{
			ParameterParser.EcoregionsDataset = Model.Ecoregions;
			ParameterParser parser = new ParameterParser();
			IParameters parameters = Data.Load<IParameters>(dataFile, parser);

			timestep = parameters.Timestep;
			nextTimeToRun = startTime - 1 + timestep;
			mapNameTemplate = parameters.MapNamesTemplate;

			SiteVars.Initialize();
			Model.SiteVars.RegisterVar(SiteVars.TimeOfLastEvent, "Wind.TimeOfLastEvent");
			Event.Initialize(parameters.EventParameters,
			                 parameters.WindSeverities,
			                 timestep);

			UI.WriteLine("Opening wind log file \"{0}\" ...", parameters.LogFileName);
			log = Data.CreateTextFile(parameters.LogFileName);
			log.AutoFlush = true;
			log.WriteLine("Time,Initiation Site,Total Sites,Damaged Sites,Cohorts Killed,Mean Severity");
		}

		//---------------------------------------------------------------------

		///<summary>
		/// Run the plug-in at a particular timestep.
		///</summary>
		public void Run(int currentTimestep)
		{
			UI.WriteLine("Processing landscape for wind events ...");

			nextTimeToRun += timestep;

			SiteVars.Event.SiteValues = null;
			SiteVars.Severity.ActiveSiteValues = 0;

			int eventCount = 0;
			foreach (ActiveSite site in Model.Landscape) {
				Event windEvent = Event.Initiate(site, currentTimestep);
				if (windEvent != null) {
					LogEvent(currentTimestep, windEvent);
					eventCount++;
				}
			}
			UI.WriteLine("  Wind events: {0}", eventCount);

			//  Write wind severity map
			IOutputRaster<SeverityPixel> map = CreateMap(currentTimestep);
			using (map) {
				SeverityPixel pixel = new SeverityPixel();
				foreach (Site site in Model.Landscape.AllSites) {
					if (site.IsActive) {
						if (Model.SiteVars.Disturbed[site])
							pixel.Band0 = (byte) (SiteVars.Severity[site] + 1);
						else
							pixel.Band0 = 1;
					}
					else {
						//	Inactive site
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
			return Util.Raster.Create<SeverityPixel>(path,
			                                         Model.LandscapeMapDims,
			                                         Model.LandscapeMapMetadata);
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
