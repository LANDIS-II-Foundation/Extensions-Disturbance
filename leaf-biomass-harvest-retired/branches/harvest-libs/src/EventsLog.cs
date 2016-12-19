using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Landis.Library.Metadata;
using Landis.Core;

namespace Landis.Extension.LeafBiomassHarvest
{
    public class EventsLog
    {
        //log.WriteLine("Time,ManagementArea,Prescription,StandMapCode,EventId,StandAge,StandRank,StandSiteCount,DamagedSites,MgBiomassRemoved,MgBioRemovedPerDamagedHa,CohortsDamaged,CohortsKilled{0}", species_header_names);

        [DataFieldAttribute(Unit = FieldUnits.Year, Desc = "Harvest Year")]
        public int Time {set; get;}

        [DataFieldAttribute(Desc = "Management Area")]
        public uint ManagementArea { set; get; }

        [DataFieldAttribute(Desc = "Prescription Name")]
        public string Prescription { set; get; }

        [DataFieldAttribute(Desc = "Stand Map Code")]
        public uint StandMapCode { set; get; }

        [DataFieldAttribute(Desc = "Event ID")]
        public int EventID { set; get; }

        [DataFieldAttribute(Unit = FieldUnits.Year, Desc = "Stand Age")]
        public int StandAge { set; get; }

        [DataFieldAttribute(Desc = "Stand Rank", Format = "0.0")]
        public double StandRank { set; get; }

        [DataFieldAttribute(Unit = FieldUnits.Count, Desc = "Stand Site Count")]
        public int StandSiteCount { set; get; }

        [DataFieldAttribute(Unit = FieldUnits.Count, Desc = "Number of Sites Harvested")]
        public int HarvestedSites { set; get; }

        [DataFieldAttribute(Unit = FieldUnits.Mg_ha, Desc = "Biomass Removed (Mg)", Format = "0.00")]
        public double MgBiomassRemoved { set; get; }

        [DataFieldAttribute(Unit = FieldUnits.Mg_ha, Desc = "Biomass Removed (Mg) per damaged hectare", Format = "0.00")]
        public double MgBioRemovedPerDamagedHa { set; get; }

        [DataFieldAttribute(Unit = FieldUnits.Count, Desc = "Number of Cohorts Partially Harvested")]
        public int CohortsHarvestedPartial { set; get; }

        [DataFieldAttribute(Unit = FieldUnits.Count, Desc = "Number of Cohorts Completely Harvested")]
        public int CohortsHarvestedComplete { set; get; }

        [DataFieldAttribute(Unit = FieldUnits.Count, Desc = "Species Cohorts Harvested by Species", SppList = true)]
        public int[] CohortsHarvested_ { set; get; }

        
        //[DataFieldAttribute(Unit = FiledUnits.None, Desc = "Initiation Row")]
        //public int InitRow { set; get; }

        //[DataFieldAttribute(Unit = FiledUnits.None, Desc = "Initiation Column")]
        //public int InitColumn { set; get; }

        //[DataFieldAttribute(Unit = FiledUnits.None, Desc = "Total Number of Sites in Event")]
        //public int TotalSites { set; get; }

        //[DataFieldAttribute(Unit = FiledUnits.None, Desc = "Number of Damaged Sites in Event")]
        //public int DamagedSites { set; get; }

        //[DataFieldAttribute(Unit = FiledUnits.None, Desc = "Mean Severity (1-5)", Format="0.00")]
        //public double MeanSeverity { set; get; }

//for (i = 0; i < modelCore.Species.Count; i++) {
        //    [DataFieldAttribute(Unit = FiledUnits.None, Desc = "Species cohorts killed")]
        //    public double Species.Name { set; get; }
        //        //species_header_names += "," + modelCore.Species[i].Name;
        //}


    }
}
