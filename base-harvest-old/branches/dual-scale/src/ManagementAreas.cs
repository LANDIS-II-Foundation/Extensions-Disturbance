using Landis.DualScale;
using System.Collections.Generic;
using System.Text;
using Wisc.Flel.GeospatialModeling.Landscapes.DualScale;
using Wisc.Flel.GeospatialModeling.RasterIO;

namespace Landis.Harvest
{
    /// <summary>
    /// Utility methods for management areas.
    /// </summary>
    public static class ManagementAreas
    {
        private static List<ushort> inactiveMgmtAreas;
        private static IManagementAreaDataset mgmtAreas;
        //---------------------------------------------------------------------

        static ManagementAreas()
        {
            inactiveMgmtAreas = new List<ushort>();
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Reads the input map of management areas.
        /// </summary>
        /// <param name="path">
        /// Path to the map.
        /// </param>
        /// <param name="managementAreas">
        /// Management areas that have prescriptions applied to them.
        /// </param>
        public static void ReadMap(string                 path,
                                   IManagementAreaDataset managementAreas)
        {
            mgmtAreas = managementAreas;

            IInputRaster<MapCodePixel> map = Model.Core.OpenRaster<MapCodePixel>(path);
            InputMap.ReadWithMajorityRule(map, Model.Core.Landscape, AssignSiteToMgmtArea);

            // Inform user about non-active areas: those that don't have any
            // applied prescriptions.
            if (inactiveMgmtAreas.Count > 0) {
                UI.WriteLine("Inactive management areas: {0}",
                             MapCodesToString(inactiveMgmtAreas));
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Assigns an active site to a particular management area.
        /// </summary>
        /// <param name="activeSite">
        /// The active site that is being assigned to a management area.
        /// </param>
        /// <param name="mapCode">
        /// The map code of the management area that the site is being assigned
        /// to.
        /// </param>
        public static void AssignSiteToMgmtArea(ActiveSite activeSite,
                                                ushort     mapCode)
        {
            ManagementArea mgmtArea = mgmtAreas.Find(mapCode);
            if (mgmtArea == null) {
                if (! inactiveMgmtAreas.Contains(mapCode))
                    inactiveMgmtAreas.Add(mapCode);
            }
            else {
                mgmtArea.OnMap = true;
                SiteVars.ManagementArea[activeSite] = mgmtArea;
            }
        }

        //---------------------------------------------------------------------

        public static string MapCodesToString(List<ushort> mapCodes)
        {
            if (mapCodes == null || mapCodes.Count == 0)
                return "";

            mapCodes.Sort();
            List<Range> ranges = new List<Range>();
            Range currentRange = new Range(mapCodes[0]);
            for (int i = 1; i < mapCodes.Count; i++) {
                ushort mapCode = mapCodes[i];
                if (currentRange.End + 1 == mapCode)
                    currentRange.End = mapCode;
                else {
                    ranges.Add(currentRange);
                    currentRange = new Range(mapCode);
                }
            }
            ranges.Add(currentRange);

            StringBuilder listAsText = new StringBuilder(ranges[0].ToString());
            for (int i = 1; i < ranges.Count; i++) {
                listAsText.AppendFormat(", {0}", ranges[i]);
            }
            return listAsText.ToString();
        }

        //---------------------------------------------------------------------

        private struct Range
        {
            public ushort Start;
            public ushort End;

            //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

            public Range(ushort mapCode)
            {
                Start = mapCode;
                End = mapCode;
            }

            //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

            public override string ToString()
            {
                if (Start == End)
                    return Start.ToString();
                if (Start + 1 == End)
                    return string.Format("{0}, {1}", Start, End);
                return string.Format("{0}-{1}", Start, End);
            }
        }
    }
}
