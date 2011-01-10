using Landis.Core;
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
		private Settings settings;
		private int nextTimeToRun;

		private SiteVariable<int> timeLastWind;
		private SiteVariable<bool> disturbed;

		//---------------------------------------------------------------------

		///<summary>
		/// Create a new plug-in.
		///</summary>
		public PlugIn()
		{
			settings = null;

			timeLastWind = new SiteVariable<int>("timeOfLastWind");
			disturbed = new SiteVariable<bool>("disturbed");
		}

		//---------------------------------------------------------------------

		///<summary>
		/// The plug-in's settings.
		///</summary>
		public Settings Settings
		{
			get {
				return settings;
			}
		}

		//---------------------------------------------------------------------

		///<summary>
		/// Initialize the plug-in.  Read data from text file.
		///</summary>
		public void Initialize(string settingsURI)
		{
			settings = Settings.Load(settingsURI);
			nextTimeToRun = settings.Timestep;

			Framework.Landscape.Add(disturbed);
			Framework.Landscape.Add(timeLastWind);

			Event.Initialize(settings.EventParms);
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
			nextTimeToRun += settings.Timestep;

			//create new landscape, windDisturbMap, with one int: windClass
			//initialize windDisturbMap:  non-active = 0, active = 1;
			Landscape.Landscape windClassMap = new Landscape.Landscape(Framework.Landscape);
			SiteVariable<byte> windClass = new SiteVariable<byte>("windClass");
			windClassMap.Add(windClass);
			windClass.SetInactiveValue(0);
			windClass.SetActiveValues(1);

			foreach (ActiveSite site in Framework.Landscape) {
				Event windEvent = Event.Initiate(site);
				if (windEvent != null) {
					windEvent.Spread();
					windClass[site] = windEvent.WindClass;
					windEvent.WriteLogEntry(Framework.Log);
					timeLastWind[site] = currentTimestep;
					disturbed[site] = true;
				}
			}

			//  Write wind class map
			//  First, create filename for map using current timestep
			Macros macros = new Macros();
			macros.Add("timestep", System.Convert.ToString(currentTimestep));
			string path = macros.Replace(settings.PathTemplate);

			//  Output map has just one band whose data type = byte.
			System.Type[] bandTypes = new System.Type[] { typeof(byte) };
			IOutputRaster raster = Framework.RasterFactory.Create(
			                               path,
			                               Framework.LandscapeRasterDimensions,
			                               bandTypes);
			IOutputBand<byte> band = raster.GetBand<byte>(1);

			foreach (Site site in windClassMap.AllSites)
				band[site] = windClass[site] + 1;
			raster.Close();
		}
	}
}
