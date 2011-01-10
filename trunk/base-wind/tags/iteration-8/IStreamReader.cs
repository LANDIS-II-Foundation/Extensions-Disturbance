namespace Landis.Util
{
	public interface IStreamReader
	{
		bool SkipBlankLines {
			get;
			set;
		}

		//---------------------------------------------------------------------

		bool SkipCommentLines {
			get;
			set;
		}

		//---------------------------------------------------------------------

		bool TrimEolComments {
			get;
			set;
		}
	}
}
