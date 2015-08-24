using Edu.Wisc.Forest.Flel.Util;

namespace Landis.Harvest
{
	/// <summary>
	/// Editable management area.
	/// </summary>
	public class EditableManagementArea
	    : IEditable<ManagementArea>
	{
	    private ushort mapCode;

		//---------------------------------------------------------------------

		public EditableManagementArea(ushort mapCode)
		{
		    this.mapCode = mapCode;
		}

		//---------------------------------------------------------------------

		public ushort MapCode
		{
		    get {
		        return mapCode;
		    }
		}

		//---------------------------------------------------------------------

		public bool IsComplete
		{
		    get {
		        // TODO:  Check if all editable applied prescriptions are complete.
		        return false;
		    }
		}

		//---------------------------------------------------------------------

		public ManagementArea GetComplete()
		{
		    return new ManagementArea(mapCode);
		}

		//---------------------------------------------------------------------

    	/// <summary>
    	/// List of editable management areas.
    	/// </summary>
    	public class List
    	    : ListOfEditable<EditableManagementArea, ManagementArea>
    	{
    		public List()
    		    : base()
    		{
    		}
    	}
	}
}
