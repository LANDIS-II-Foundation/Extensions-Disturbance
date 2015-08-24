using Wisc.Flel.GeospatialModeling.Landscapes;
using Wisc.Flel.GeospatialModeling.RasterIO;

namespace Landis.Harvest
{
    /// <summary>
    /// Utility class for prescription maps.
    /// </summary>
    public class PrescriptionMaps
    {
        private string nameTemplate;

        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="nameTemplate">
        /// The template for the pathnames to the maps.
        /// </param>
        public PrescriptionMaps(string nameTemplate)
        {
            this.nameTemplate = nameTemplate;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Writes an output map of prescriptions that harvested each active
        /// site.
        /// </summary>
        /// <param name="timestep">
        /// Timestep to use in the map's name.
        /// </param>
        public void WriteMap(int timestep)
        {
            string path = MapNames.ReplaceTemplateVars(nameTemplate, timestep);
            using (IOutputRaster<PrescriptionPixel> map = CreateMap(path)) {
                PrescriptionPixel pixel = new PrescriptionPixel();
                foreach (Site site in Model.Core.Landscape.AllSites) {
                    if (site.IsActive) {
                        Prescription prescription = SiteVars.Prescription[site];
                        if (prescription == null)
                            pixel.Band0 = 1;
                        else
                            pixel.Band0 = (byte) (prescription.Number + 1);
                    }
                    else {
                        //  Inactive site
                        pixel.Band0 = 0;
                    }
                    map.WritePixel(pixel);
                }
            }
        }

        //---------------------------------------------------------------------

        private IOutputRaster<PrescriptionPixel> CreateMap(string path)
        {
            UI.WriteLine("Writing prescription map to {0} ...", path);
            return Model.Core.CreateRaster<PrescriptionPixel>(path,
                                                              Model.Core.Landscape.Dimensions,
                                                              Model.Core.LandscapeMapMetadata);
        }
    }
}
