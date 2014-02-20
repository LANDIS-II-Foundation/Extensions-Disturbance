using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Landis.Library.Metadata;

namespace Landis.Extension.LeafBiomassHarvest
{
    public class SummaryLog
    {
        //summaryLog.WriteLine("Time,ManagementArea,Prescription,TotalDamagedSites,TotalCohortsDamaged,TotalCohortsKilled{0}", species_header_names);

        [DataFieldAttribute(Unit = FieldUnits.Year, Desc = "Simulation Year")]
        public int Time {set; get;}

        [DataFieldAttribute(Desc = "Management Area")]
        public uint ManagementArea { set; get; }

        [DataFieldAttribute(Desc = "Prescription Name")]
        public string Prescription { set; get; }

        [DataFieldAttribute(Unit = FieldUnits.Count, Desc = "Total Damaged Sites")]
        public int TotalDamagedSites { set; get; }

        [DataFieldAttribute(Unit = FieldUnits.Count, Desc = "Total Cohorts Killed")]
        public int TotalCohortsKilled { set; get; }

        [DataFieldAttribute(Unit = FieldUnits.Count, Desc = "Total Cohorts Damaged")]
        public int TotalCohortsDamaged { set; get; }

        [DataFieldAttribute(Unit = FieldUnits.Count, Desc = "Species Cohorts Killed", SppList = true)]
        public double[] CohortsKilledBy { set; get; }

        //[DataFieldAttribute(Unit = FiledUnits.None, Desc = "Initiation Row")]
        //public int InitRow { set; get; }

        //[DataFieldAttribute(Unit = FiledUnits.None, Desc = "Initiation Column")]
        //public int InitColumn { set; get; }

        //[DataFieldAttribute(Unit = FiledUnits.None, Desc = "Total Number of Sites in Event")]
        //public int TotalSites { set; get; }

        //[DataFieldAttribute(Unit = FiledUnits.None, Desc = "Number of Damaged Sites in Event")]
        //public int DamagedSites { set; get; }

        //[DataFieldAttribute(Unit = FiledUnits.None, Desc = "Number of Cohorts Killed")]
        //public int CohortsKilled { set; get; }

        //[DataFieldAttribute(Unit = FiledUnits.None, Desc = "Mean Severity (1-5)", Format="0.00")]
        //public double MeanSeverity { set; get; }

    }
}
