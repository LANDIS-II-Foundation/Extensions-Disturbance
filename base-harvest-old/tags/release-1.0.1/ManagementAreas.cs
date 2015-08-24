using Landis.Landscape;
using Landis.RasterIO;

using System.Collections.Generic;
using System.Text;

namespace Landis.Harvest
{
    /// <summary>
    /// Utility methods for management areas.
    /// </summary>
    public static class ManagementAreas
    {
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
            //UI.WriteLine("reading ma map!!\n\n");
            List<ushort> inactiveMgmtAreas = new List<ushort>();

            IInputRaster<MapCodePixel> map = Model.Core.OpenRaster<MapCodePixel>(path);
            using (map) {
                // TODO: make sure its dimensions match landscape's dimensions
                foreach (Site site in Model.Core.Landscape.AllSites) {
                    MapCodePixel pixel = map.ReadPixel();
                    if (site.IsActive) {
                        ushort mapCode = pixel.Band0;
                        ManagementArea mgmtArea = managementAreas.Find(mapCode);
                        if (mgmtArea == null) {
                            if (! inactiveMgmtAreas.Contains(mapCode))
                                inactiveMgmtAreas.Add(mapCode);
                        }
                        else {
                            mgmtArea.OnMap = true;
                            SiteVars.ManagementArea[site] = mgmtArea;
                        }
                    }
                }
            }

            // Inform user about non-active areas: those that don't have any
            // applied prescriptions.
            if (inactiveMgmtAreas.Count > 0) {
                UI.WriteLine("Inactive management areas: {0}",
                             MapCodesToString(inactiveMgmtAreas));
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
