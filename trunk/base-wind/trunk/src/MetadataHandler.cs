using System;
using System.Collections.Generic;
using System.Linq;
//using System.Data;
using System.Text;
using Landis.Library.Metadata;
using Edu.Wisc.Forest.Flel.Util;

namespace Landis.Extension.BaseWind
{
    public static class MetadataHandler
    {
        
        public static ExtensionMetadata Extension {get; set;}

        public static void InitializeMetadata(int Timestep, string MapFileName)
        {
            ScenarioReplicationMetadata scenRep = new ScenarioReplicationMetadata() {
                //String outputFolder = OutputPath.ReplaceTemplateVars("", FINISH ME LATER);
                FolderName = System.IO.Directory.GetCurrentDirectory().Split("\\".ToCharArray()).Last(),//"Scen_?-rep_?", //we should probably add this to the extension/scenario input file or we might be leaving this out because the extensions do not need to know anything about the replication (the hirarchy of the scenario-replications and their extensions are defined by the convention of folder structures)
                RasterOutCellArea = PlugIn.ModelCore.CellArea,
                TimeMin = PlugIn.ModelCore.StartTime,
                TimeMax = PlugIn.ModelCore.EndTime,
                ProjectionFilePath = "Projection.?" //How do we get projections???
            };

            Extension = new ExtensionMetadata(){
                Name = PlugIn.ExtensionName,
                TimeInterval = Timestep, //change this to PlugIn.TimeStep for other extensions
                ScenarioReplicationMetadata = scenRep
            };

            //---------------------------------------
            //          table outputs:   
            //---------------------------------------

            OutputMetadata tblOut_events = new OutputMetadata()
            {
                Type = OutputType.Table,
                Name = "WindLog",
                FilePath = PlugIn.eventLog.FilePath//,
                //MetadataFilePath = @"Base-Wind\EventLog.xml"
            };
            tblOut_events.RetriveFields(typeof(EventsLog));
            Extension.OutputMetadatas.Add(tblOut_events);


            //---------------------------------------            
            //          map outputs:         
            //---------------------------------------

            OutputMetadata mapOut_Severity = new OutputMetadata()
            {
                Type = OutputType.Map,
                Name = "severity",
                FilePath = @MapFileName,
                Map_DataType = MapDataType.Nominal,
                Map_Unit = "categorical",
            };
            Extension.OutputMetadatas.Add(mapOut_Severity);

            //---------------------------------------
            MetadataProvider mp = new MetadataProvider(Extension);
            mp.WriteMetadataToXMLFile("Metadata", Extension.Name, Extension.Name);




        }
    }
}
