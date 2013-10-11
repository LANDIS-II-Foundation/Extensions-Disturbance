using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Landis.Library.Metadata;
using Edu.Wisc.Forest.Flel.Util;
using Landis.Core;

namespace Landis.Extension.Insects
{
    public static class MetadataHandler
    {
        
        public static ExtensionMetadata Extension {get; set;}

        public static void InitializeMetadata(int Timestep, 
            string MapFileName, 
            string logFileName, 
            IEnumerable<IInsect> insects,
            ICore mCore)
        {
            ScenarioReplicationMetadata scenRep = new ScenarioReplicationMetadata() {
                //String outputFolder = OutputPath.ReplaceTemplateVars("", FINISH ME LATER);
                FolderName = System.IO.Directory.GetCurrentDirectory().Split("\\".ToCharArray()).Last(),//"Scen_?-rep_?", //we should probably add this to the extension/scenario input file or we might be leaving this out because the extensions do not need to know anything about the replication (the hirarchy of the scenario-replications and their extensions are defined by the convention of folder structures)
                RasterOutCellArea = PlugIn.ModelCore.CellArea,
                TimeMin = PlugIn.ModelCore.StartTime,
                TimeMax = PlugIn.ModelCore.EndTime,
                ProjectionFilePath = "Projection.?" //How do we get projections???
            };

            Extension = new ExtensionMetadata(mCore){
                Name = PlugIn.ExtensionName,
                TimeInterval = Timestep, //change this to PlugIn.TimeStep for other extensions
                ScenarioReplicationMetadata = scenRep
            };

            //---------------------------------------
            //          table outputs:   
            //---------------------------------------

             PlugIn.eventLog = new MetadataTable<EventsLog>(logFileName);

            OutputMetadata tblOut_events = new OutputMetadata()
            {
                Type = OutputType.Table,
                Name = "EventsLog",
                FilePath = PlugIn.eventLog.FilePath//,
            };
            tblOut_events.RetriveFields(typeof(EventsLog));
            Extension.OutputMetadatas.Add(tblOut_events);


            //---------------------------------------            
            //          map outputs:         
            //---------------------------------------

            foreach (IInsect insect in insects)
            {

                string mapPath = MapNames.ReplaceTemplateVarsMetadata(MapFileName, insect.Name);
                OutputMetadata mapOut_GrowthReduction = new OutputMetadata()
                {
                    Type = OutputType.Map,
                    Name = "Growth Reduction",
                    FilePath = @mapPath,
                    Map_DataType = MapDataType.Quantitative,
                    Map_Unit = "percentile",
                };
                Extension.OutputMetadatas.Add(mapOut_GrowthReduction);

                mapPath = MapNames.ReplaceTemplateVarsMetadata(MapFileName, ("InitialPatchMap-" + insect.Name));
                OutputMetadata mapOut_InitialPatchProb = new OutputMetadata()
                {
                    Type = OutputType.Map,
                    Name = "Initial Outbreak Probabilities",
                    FilePath = @mapPath,
                    Map_DataType = MapDataType.Quantitative,
                    Map_Unit = "percentile",
                };
                Extension.OutputMetadatas.Add(mapOut_InitialPatchProb);

                mapPath = MapNames.ReplaceTemplateVarsMetadata(MapFileName, ("BiomassRemoved-" + insect.Name));
                OutputMetadata mapOut_BiomassRemoved = new OutputMetadata()
                {
                    Type = OutputType.Map,
                    Name = "Biomass Mortality",
                    FilePath = @mapPath,
                    Map_DataType = MapDataType.Quantitative,
                    Map_Unit = FiledUnits.Mg_ha,
                };
                Extension.OutputMetadatas.Add(mapOut_BiomassRemoved);
            }
            //---------------------------------------
            MetadataProvider mp = new MetadataProvider(Extension);
            mp.WriteMetadataToXMLFile("Metadata", Extension.Name, Extension.Name);




        }
    }
}
