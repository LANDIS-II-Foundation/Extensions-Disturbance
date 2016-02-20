using System;
using System.Collections.Generic;
using System.Linq;
//using System.Data;
using System.Text;
using Landis.Library.Metadata;
using Landis.Core;
using Edu.Wisc.Forest.Flel.Util;

namespace Landis.Extension.LeafBiomassHarvest
{
    public static class MetadataHandler
    {
        
        public static ExtensionMetadata Extension {get; set;}

        public static void InitializeMetadata(int Timestep, string MapFileName, string BiomassMapName, ICore mCore)
        {

            ScenarioReplicationMetadata scenRep = new ScenarioReplicationMetadata() {
                RasterOutCellArea = PlugIn.ModelCore.CellArea,
                TimeMin = PlugIn.ModelCore.StartTime,
                TimeMax = PlugIn.ModelCore.EndTime,
            };

            Extension = new ExtensionMetadata(PlugIn.ModelCore)
            //Extension = new ExtensionMetadata()
            {
                Name = PlugIn.ExtensionName,
                TimeInterval = Timestep, //change this to PlugIn.TimeStep for other extensions
                ScenarioReplicationMetadata = scenRep
            };

            //---------------------------------------
            //          table outputs:   
            //---------------------------------------

            PlugIn.eventLog = new MetadataTable<EventsLog>("harvest-events-log.csv");
            PlugIn.summaryLog = new MetadataTable<SummaryLog>("harvest-summary-log.csv");
            PlugIn.summaryLogShort = new MetadataTable<SummaryLogShort>("harvest-summary-short-log.csv");

            PlugIn.ModelCore.UI.WriteLine("   Generating event table...");
            OutputMetadata tblOut_events = new OutputMetadata()
            {
                Type = OutputType.Table,
                Name = "EventLog",
                FilePath = PlugIn.eventLog.FilePath,
                Visualize = false,
            };
            tblOut_events.RetriveFields(typeof(EventsLog));
            Extension.OutputMetadatas.Add(tblOut_events);

            PlugIn.ModelCore.UI.WriteLine("   Generating summary table...");
            OutputMetadata tblOut_summary = new OutputMetadata()
            {
                Type = OutputType.Table,
                Name = "SummaryLog",
                FilePath = PlugIn.summaryLog.FilePath,
                Visualize = false,
            };
            tblOut_summary.RetriveFields(typeof(SummaryLog));
            Extension.OutputMetadatas.Add(tblOut_summary);

            //PlugIn.ModelCore.UI.WriteLine("   Generating summary table...");
            OutputMetadata tblOut_summaryShort = new OutputMetadata()
            {
                Type = OutputType.Table,
                Name = "SummaryLog_Short",
                FilePath = PlugIn.summaryLogShort.FilePath,
                Visualize = true,
            };
            tblOut_summaryShort.RetriveFields(typeof(SummaryLogShort));
            Extension.OutputMetadatas.Add(tblOut_summaryShort);

            //---------------------------------------            
            //          map outputs:         
            //---------------------------------------

            OutputMetadata mapOut_BiomassRemoved = new OutputMetadata()
            {
                Type = OutputType.Map,
                Name = "biomass removed",
                FilePath = @BiomassMapName,
                Map_DataType = MapDataType.Continuous,
                Map_Unit = FieldUnits.Mg_ha,
                Visualize = true,
            };
            Extension.OutputMetadatas.Add(mapOut_BiomassRemoved);


            OutputMetadata mapOut_Prescription = new OutputMetadata()
            {
                Type = OutputType.Map,
                Name = "prescription",
                FilePath = @MapFileName,
                Map_DataType = MapDataType.Nominal,
                Visualize = true,
                //Map_Unit = "categorical",
            };
            Extension.OutputMetadatas.Add(mapOut_Prescription);

            //---------------------------------------
            MetadataProvider mp = new MetadataProvider(Extension);
            mp.WriteMetadataToXMLFile("Metadata", Extension.Name, Extension.Name);




        }
    }
}
