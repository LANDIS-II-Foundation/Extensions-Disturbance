namespace Landis.Wind
{
	public class Settings
	{
		private int timestep;
		private string pathTemplate;

		//---------------------------------------------------------------------

		public static Settings Load(string path)
		{
			//  This method read plug-in's settings from the given file.
			//  The file contains wind event parameters per ecoregion.
			//  These parameters are in tabular format:
			//
			//		ecoregion   WEP  maxSize   meanSize   minSize
			//
			//  Before reading the table, we allocate an array of
			//  EventParameters with its length = Framework.Ecoregions.Count
			//  Then loop:
			//
			//		read ecoregion name
			//		lookup name in Framework.Ecoregions
			//			returns Ecoregion instance or null
			//		if null
			//			error: unknown ecoregion
			//		read event parameters: prob, maxSize, meanSize, minSize
			//		eventParms[ecoregion.Index] = new EventParameters(...);
			return null;
		}

		//---------------------------------------------------------------------

		public int Timestep
		{
			get {
				return timestep;
			}
		}

		//---------------------------------------------------------------------

		public string PathTemplate
		{
			get {
				return pathTemplate;
			}
		}

		//---------------------------------------------------------------------

		public EventParameters[] EventParms
		{
			get {
				return null;
			}
		}
	}
}
