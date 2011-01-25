using Landis.AgeCohort;

namespace Landis.Harvest
{
    /// <summary>
    /// A range of cohort ages.
    /// </summary>
    public struct AgeRange
    {
        private ushort start;
        private ushort end;

        //---------------------------------------------------------------------

        /// <summary>
        /// The starting age of the range.
        /// </summary>
        public ushort Start
        {
            get {
                return start;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The ending age of the range.
        /// </summary>
        public ushort End
        {
            get {
                return end;
            }
        }

        //---------------------------------------------------------------------

    	public AgeRange(ushort start,
                        ushort end)
    	{
            this.start = start;
            this.end   = end;
    	}
    }
}
