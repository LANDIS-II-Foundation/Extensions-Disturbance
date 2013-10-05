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

        [DataFieldAttribute(Unit = FiledUnits.Year, Desc = "Harvest Year")]
        public int Time {set; get;}

        [DataFieldAttribute(Unit = FiledUnits.None, Desc = "Management Area")]
        public uint ManagementArea { set; get; }

        [DataFieldAttribute(Unit = FiledUnits.None, Desc = "Prescription Name")]
        public string Prescription { set; get; }

        [DataFieldAttribute(Unit = FiledUnits.None, Desc = "Stand Map Code")]
        public uint StandMapCode { set; get; }

        [DataFieldAttribute(Unit = FiledUnits.None, Desc = "Event ID")]
        public int EventID { set; get; }

        [DataFieldAttribute(Unit = FiledUnits.Year, Desc = "Stand Age")]
        public int StandAge { set; get; }

        [DataFieldAttribute(Unit = FiledUnits.None, Desc = "Stand Rank", Format = "0.0")]
        public double StandRank { set; get; }

        [DataFieldAttribute(Unit = FiledUnits.Count, Desc = "Stand Site Count")]
        public int StandSiteCount { set; get; }

        [DataFieldAttribute(Unit = FiledUnits.Count, Desc = "Number of Damaged Sites")]
        public int DamagedSites { set; get; }

        [DataFieldAttribute(Unit = FiledUnits.None, Desc = "Biomass Removed (Mg)", Format = "0.00")]
        public double MgBiomassRemoved { set; get; }

        [DataFieldAttribute(Unit = FiledUnits.None, Desc = "Biomass Removed (Mg) per damaged hectare", Format = "0.00")]
        public double MgBioRemovedPerDamagedHa { set; get; }

        [DataFieldAttribute(Unit = FiledUnits.Count, Desc = "Number of Cohorts Damaged")]
        public int CohortsDamaged { set; get; }

        [DataFieldAttribute(Unit = FiledUnits.Count, Desc = "Number of Cohorts Killed")]
        public int CohortsKilled { set; get; }

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
