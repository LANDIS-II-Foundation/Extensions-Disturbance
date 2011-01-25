using Edu.Wisc.Forest.Flel.Util;
using System.Collections.Generic;
using System.Reflection;

using log4net;

namespace Landis.Harvest
{
    /// <summary>
    /// Management area is a collection of stands to which specific harvesting
    /// prescriptions are applied.
    /// </summary>
    public class ManagementArea
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly bool isDebugEnabled = log.IsDebugEnabled;

        private ushort mapCode;
        private List<Stand> stands;
        private double area;
        private List<AppliedPrescription> prescriptions;
        private bool onMap;

        //---------------------------------------------------------------------

        /// <summary>
        /// The code that represents the area in the management area input map.
        /// </summary>
        public ushort MapCode
        {
            get {
                return mapCode;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The number of stands in the management area.
        /// </summary>
        public int StandCount
        {
            get {
                return stands.Count;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The area covered by the management area (units: hectares).
        /// </summary>
        public double Area
        {
            get {
                return area;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Was the management area's map code used in the map of management
        /// areas?
        /// </summary>
        public bool OnMap
        {
            get {
                return onMap;
            }

            set {
                onMap = value;
            }
        }

        //---------------------------------------------------------------------

        public ManagementArea(ushort mapCode)
        {
            this.mapCode = mapCode;
            stands = new List<Stand>();
            area = 0.0;
            // TODO: when the MA's stands are assigned, total their areas to
            //       determine the MA's area
            prescriptions = new List<AppliedPrescription>();
            onMap = false;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Has a particular prescription been applied to the managment area?
        /// </summary>
        public bool IsApplied(Prescription prescription)
        {
            foreach (AppliedPrescription appliedPrescription in prescriptions)
                if (appliedPrescription.Prescription == prescription)
                    return true;
            return false;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Adds a prescription to be applied to the management area.
        /// </summary>
        public void ApplyPrescription(Prescription prescription,
                                      Percentage   percentageToHarvest,
                                      int          startTime,
                                      int          endTime)
        {
            prescriptions.Add(new AppliedPrescription(prescription,
                                                      this,
                                                      percentageToHarvest,
                                                      startTime,
                                                      endTime));
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Adds a stand to the management area.
        /// </summary>
        public void Add(Stand stand)
        {
            stands.Add(stand);
            stand.ManagementArea = this;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Finish initializing the management area after reading the stand
        /// map.
        /// </summary>
        /// <remarks>
        /// This phase of initialization includes computing the total area for
        /// the management area, and finishing the initialization of its
        /// applied prescriptions.
        /// </remarks>
        public void FinishInitialization()
        {
            //  Update the total area of the management area after adding all
            //  its stands.
            area = 0;
            foreach (Stand stand in stands)
                area += stand.ActiveArea;

            foreach (AppliedPrescription prescription in prescriptions)
                prescription.FinishInitialization(StandCount);
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Harvest the area's stands according to its prescriptions.
        /// </summary>
        public void HarvestStands()
        {
            if (isDebugEnabled)
                log.DebugFormat("Harvesting management area {0} ...", mapCode);

            foreach (Stand stand in stands)
                stand.InitializeForHarvesting();

            //  Determine which are prescriptions are active.
            List<AppliedPrescription> activePrescriptions = new List<AppliedPrescription>();
            foreach (AppliedPrescription prescription in prescriptions) {
                if (prescription.BeginTime <= Model.Core.CurrentTime &&
                        Model.Core.CurrentTime <= prescription.EndTime) {
                    if (isDebugEnabled)
                        log.DebugFormat("  Initializing prescription {0} ...", prescription.Prescription.Name);
                    prescription.InitializeForHarvest(stands);
                    if (prescription.AnyUnharvestedStandsRankedAbove0)
                        activePrescriptions.Add(prescription);
                }
            }

            if (isDebugEnabled) {
                log.DebugFormat("  # of active prescriptions: {0}",
                                activePrescriptions.Count);
                for (int i = 0; i < activePrescriptions.Count; i++)
                    log.DebugFormat("    {0})  {1}", i+1,
                                    activePrescriptions[i].Prescription.Name);
            }

            //  Loop while there are still active prescriptions that haven't
            //  reached their target harvest areas and that still have
            //  at least one unharvested stand ranked above 0.

            double[] endProbability = new double[activePrescriptions.Count];
            while (activePrescriptions.Count > 0) {
                //  Assign a part of the probability interval [0, 1) to each
                //  prescription based on how the ratio of the area remaining
                //  to be harvested to the total area to be harvested
                double ratioTotal = 0.0;
                foreach (AppliedPrescription prescription in activePrescriptions)
                    ratioTotal += prescription.AreaRemainingRatio;

                for (int i = 0; i < activePrescriptions.Count; ++i) {
                    AppliedPrescription prescription = activePrescriptions[i];
                    if (i == activePrescriptions.Count - 1) {
                        //  last prescription
                        endProbability[i] = 1.0;
                    }
                    else {
                        double startProbability = endProbability[i-1];
                        double intervalWidth = prescription.AreaRemainingRatio / ratioTotal;
                        endProbability[i] = startProbability + intervalWidth;
                    }
                }

                //  Randomly select one of the active prescriptions and harvest
                //  the stand ranked highest by that prescription.
                AppliedPrescription selectedPrescription = null;
                double randomNum = Util.Random.GenerateUniform();
                for (int i = 0; i < activePrescriptions.Count; ++i) {
                    if (randomNum < endProbability[i]) {
                        selectedPrescription = activePrescriptions[i];
                        break;
                    }
                }
                if (isDebugEnabled)
                    log.DebugFormat("  Selected prescription: {0}", selectedPrescription.Prescription.Name);
                selectedPrescription.HarvestHighestRankedStand();

                //  Check each prescription to see if there's at least one
                //  unharvested stand that the prescription ranks higher than
                //  0.  The list is traversed in reverse order, so that the
                //  removal of items doesn't mess up the traversal.
                for (int i = activePrescriptions.Count - 1; i >= 0; --i) {
                    if (! activePrescriptions[i].AnyUnharvestedStandsRankedAbove0)
                        activePrescriptions.RemoveAt(i);
                }
            }
        }

        //---------------------------------------------------------------------

        public IEnumerator<Stand> GetEnumerator()
        {
            foreach (Stand stand in stands)
                yield return stand;
        }
    }
}
