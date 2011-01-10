using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Landis
{
	public abstract class ParameterSet
	{
		public static T Load<T>(string path)
			where T : ParameterSet, new()
		{
			T parmSet;
			if (/* path's extension is ".xml"? or ".landis" */ false) {
				//  Deserialize parameter set from the file:
				//  Binary serialization:
				IFormatter formatter = new BinaryFormatter();
				Stream stream = new FileStream(path, FileMode.Open,
 											   FileAccess.Read, FileShare.Read);
				parmSet = (T) formatter.Deserialize(stream);
				stream.Close();
			}
			else {
				parmSet = new T();
				//  Open the file specified by path
				Util.IStreamReader stream = new Util.FileStreamReader(path);
				stream.SkipBlankLines = true;
				stream.SkipCommentLines = true;
				stream.TrimEolComments = true;
				parmSet.ReadValues(stream);
			}
			return parmSet;
		}

		//---------------------------------------------------------------------

		public abstract void ReadValues(Util.IStreamReader stream);
	}
}
