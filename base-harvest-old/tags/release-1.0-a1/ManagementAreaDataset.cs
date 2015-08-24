using Edu.Wisc.Forest.Flel.Util;

using System.Collections;
using System.Collections.Generic;

namespace Landis.Harvest
{
    /// <summary>
    /// A collection of management areas.
    /// </summary>
    public class ManagementAreaDataset
        : IManagementAreaDataset
    {
        private Dictionary<ushort, ManagementArea> mgmtAreas;

        //---------------------------------------------------------------------

        public ManagementAreaDataset()
        {
            mgmtAreas = new Dictionary<ushort, ManagementArea>();
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Adds a new management area to the dataset.
        /// </summary>
        public void Add(ManagementArea mgmtArea)
        {
            Require.ArgumentNotNull(mgmtArea);
            mgmtAreas[mgmtArea.MapCode] = mgmtArea;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Finds a management area by its map code.
        /// </summary>
        /// <returns>
        /// null if there is no management area with the specified map code.
        /// </returns>
        public ManagementArea Find(ushort mapCode)
        {
            ManagementArea mgmtArea;
            if (mgmtAreas.TryGetValue(mapCode, out mgmtArea))
                return mgmtArea;
            return null;
        }

        //---------------------------------------------------------------------

        IEnumerator<ManagementArea> IEnumerable<ManagementArea>.GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        //---------------------------------------------------------------------

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<ManagementArea>) this).GetEnumerator();
        }
    }
}
