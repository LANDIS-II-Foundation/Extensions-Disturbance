using Landis.Landscape;
using Landis.Raster;
using Landis.Util;
using System.Collections.Generic;

namespace Landis.Wind
{
	///<summary>
	/// A sample disturbance plug-in that simulates wind disturbance.
	/// Copyright University of Wisconsin-Madison 2004
	/// </summary>
	public class PlugIn
		: Landis.Disturbance.IPlugIn
	{
//		private Settings settings;
		private Parameters parameters;
		private int nextTimeToRun;

		//---------------------------------------------------------------------

		///<summary>
		/// Create a new plug-in.
		///</summary>
		public PlugIn()
		{
//			settings = null;
		}

		//---------------------------------------------------------------------
/*
		///<summary>
		/// The plug-in's settings.
		///</summary>
		public Settings Settings
		{
			get {
				return settings;
			}
		}
*/
		//---------------------------------------------------------------------

		///<summary>
		/// Initialize the plug-in.  Read data from text file.
		///</summary>
		public void Initialize(string settingsURI)
		{
//			settings = Settings.Load(settingsURI);
			parameters = ParameterSet.Load<Parameters>(settingsURI);
			nextTimeToRun = parameters.Timestep;

			SiteVars.Initialize();
			Event.Initialize(parameters.EventParms);
		}

		//---------------------------------------------------------------------

		///<summary>
		/// The next model timestep that the plug-in should be run.
		///</summary>
		public int NextTimeToRun {
			get {
				return nextTimeToRun;
			}
		}

		//---------------------------------------------------------------------

		///<summary>
		/// Run the plug-in at a particular timestep.
		///</summary>
		public void Run(int currentTimestep)
		{
			nextTimeToRun += parameters.Timestep;

			//create new landscape, windDisturbMap, with one int: windClass
			//initialize windDisturbMap:  non-active = 0, active = 1;
//			Landscape.Landscape windClassMap = new Landscape.Landscape(Model.Landscape);
//			SiteVariable<byte> windClass = new SiteVariable<byte>("windClass");
//			windClassMap.Add(windClass);
//			windClass.SetInactiveValue(0);
//			windClass.SetActiveValues(1);

			foreach (ActiveSite site in Model.Landscape) {
				Event windEvent = Event.Initiate(site);
				if (windEvent != null) {
					SiteVars.Severity[site] = windEvent.Severity;
					windEvent.WriteLogEntry(Model.Log);
					SiteVars.TimeLastEvent[site] = currentTimestep;
					Model.SiteVars.Disturbed[site] = true;
				}
			}

			//  Write wind class map
			//  First, create filename for map using current timestep
			Macros macros = new Macros();
			macros.Add("timestep", System.Convert.ToString(currentTimestep));
			string path = macros.Replace(parameters.PathTemplate);

			//  Output map has just one band whose data type = byte.
			System.Type[] bandTypes = new System.Type[] { typeof(byte) };
			IOutputRaster raster = Model.RasterFactory.Create(
			                               path,
			                               Model.LandscapeRasterDimensions,
			                               bandTypes);
			IOutputBand<byte> band = raster.GetBand<byte>(1);

			foreach (Site site in Model.Landscape.AllSites)
				band[site] = (byte) (SiteVars.Severity[site] + 1);
			raster.Close();
		}
	}
}
