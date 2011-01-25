//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:  Robert M. Scheller, James B. Domingo

using Landis.SpatialModeling;

namespace Landis.Extension.BaseHarvest
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
            using (IOutputRaster<BytePixel> outputRaster = PlugIn.ModelCore.CreateRaster<BytePixel>(path, PlugIn.ModelCore.Landscape.Dimensions))
            {
                BytePixel pixel = outputRaster.BufferPixel;
            
                foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                {
                    if (site.IsActive) {
                        Prescription prescription = SiteVars.Prescription[site];
                        if (prescription == null)
                            pixel.MapCode.Value = 1;
                        else
                            pixel.MapCode.Value = (byte)(prescription.Number + 1);
                    }
                    else {
                        //  Inactive site
                        pixel.MapCode.Value = 0;
                    }
                    outputRaster.WriteBufferPixel();
                }
            }
        }

        //---------------------------------------------------------------------
        /*
        private IOutputRaster<BytePixel> CreateMap(string path)
        {
            PlugIn.ModelCore.Log.WriteLine("   Writing Prescription map to {0} ...", path);
            return PlugIn.ModelCore.CreateRaster<BytePixel>(path,
                                                          PlugIn.ModelCore.Landscape.Dimensions);
        }*/
    }
}
