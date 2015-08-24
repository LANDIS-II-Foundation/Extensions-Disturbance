// This file is part of the Base Harvest extension for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/trunk/base-harvest/trunk/

namespace Landis.Extension.BaseHarvest
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

        //---------------------------------------------------------------------

        /// <summary>
        /// Does the range contain a particular age?
        /// </summary>
        public bool Contains(ushort age)
        {
            return (start <= age) && (age <= end);
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Does the range overlap another range?
        /// </summary>
        public bool Overlaps(AgeRange other)
        {
            return Contains(other.Start) || other.Contains(start);
        }
    }
}
