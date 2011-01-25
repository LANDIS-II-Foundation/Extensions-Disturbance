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

            prescriptions = new List<AppliedPrescription>();
            onMap = false;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Has a particular prescription been applied to the managment area?
        /// </summary>
        public bool IsApplied(string prescriptionName, int beginTime, int endTime)
        {
            //UI.WriteLine("prescriptionName = {0} beginTime = {1} endTime = {2}");
            //loop through prescriptions already applied to this management area
            //looking for one that matches this exactly.
            foreach (AppliedPrescription appliedPrescription in prescriptions) {
                //if this prescription matches
                if (appliedPrescription.Prescription.Name == prescriptionName && 
                    appliedPrescription.BeginTime == beginTime &&
                    appliedPrescription.EndTime == endTime) {
                    return true;
                }
            }
            //otherwise this exact prescription has not yet been applied
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
            area += stand.ActiveArea;
            //UI.WriteLine("ma {0} now has area {1}", mapCode, area);
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
            foreach (Stand stand in stands) {
                area += stand.ActiveArea;
            }

            foreach (AppliedPrescription prescription in prescriptions)
                prescription.FinishInitialization(StandCount, area);
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Harvest the area's stands according to its prescriptions.
        /// </summary>
        public void HarvestStands()
        {
            if (isDebugEnabled)
                log.DebugFormat("Harvesting management area {0} ...", mapCode);

            //initialize each stand for harvesting (setting harvested = false)
            foreach (Stand stand in stands) {
                stand.InitializeForHarvesting();
            }

            //  Determine which are prescriptions are active.
            List<AppliedPrescription> activePrescriptions = new List<AppliedPrescription>();
            foreach (AppliedPrescription prescription in prescriptions) {
                if (prescription.BeginTime <= Model.Core.CurrentTime &&
                        Model.Core.CurrentTime <= prescription.EndTime) {
                    if (isDebugEnabled)
                        log.DebugFormat("  Initializing prescription {0} ...", prescription.Prescription.Name);
                    
                    //set harvesting areas, rank stands (by user choice method)
                    prescription.InitializeForHarvest(stands);
                    
                    if (prescription.AnyUnharvestedStandsRankedAbove0) {
						//UI.WriteLine("adding {0}", prescription.Prescription.Name);
/*                         foreach (StandRanking sr in prescription.Rankings) {
							UI.WriteLine("stand {0} ranked {1}", sr.Stand.MapCode, sr.Rank);
						} */
						activePrescriptions.Add(prescription);
                    }
                }
            }

            if (isDebugEnabled) {
                log.DebugFormat("  # of active prescriptions: {0}",
                                activePrescriptions.Count);
                for (int i = 0; i < activePrescriptions.Count; i++)
                    log.DebugFormat("    {0})  {1}", i+1,
                                    activePrescriptions[i].Prescription.Name);
            }

            foreach (AppliedPrescription prescription in prescriptions) {
                if (prescription is AppliedRepeatHarvest) {
                    ((AppliedRepeatHarvest) prescription).HarvestReservedStands();
                }
            }

            //  Loop while there are still active prescriptions that haven't
            //  reached their target harvest areas and that still have
            //  at least one unharvested stand ranked above 0.

            while (activePrescriptions.Count > 0) {
                double[] endProbability = new double[activePrescriptions.Count + 1];

                //  Assign a part of the probability interval [0, 1) to each prescription based on how the ratio of the area remaining to be harvested to the total area to be harvested
                double ratioTotal = 0.0;
                
                foreach (AppliedPrescription prescription in activePrescriptions) {
                    ratioTotal += prescription.AreaRemainingRatio;
                }
                
                if (ratioTotal > 0) {
                    for (int i = 0; i < activePrescriptions.Count; ++i) {
                        AppliedPrescription prescription = activePrescriptions[i];
                        //first prescription, start at 0
                        if (i == 0) {
                            endProbability[i] = prescription.AreaRemainingRatio / ratioTotal;
                        }

                        //last prescription, end at 1.0                    
                        else if (i == activePrescriptions.Count - 1) {
                            endProbability[i] = 1.0;
                        }                    

                        //
                        else {
                            double startProbability = endProbability[i - 1];
                            double intervalWidth = prescription.AreaRemainingRatio / ratioTotal;
                            endProbability[i] = startProbability + intervalWidth;

                        }

                    }

                    //  Randomly select one of the active prescriptions and harvest the stand ranked highest by that prescription.
                    AppliedPrescription selectedPrescription = null;

                    double randomNum = Util.Random.GenerateUniform();
                    for (int i = 0; i < activePrescriptions.Count; ++i) {
                        if (randomNum < endProbability[i]) {
                            selectedPrescription = activePrescriptions[i];
							//UI.WriteLine("\nSELECTED PRESCRIPTION = {0}\n", selectedPrescription.Prescription.Name);
                            break;
                        }
                    }
					
                    //actually harvest the stands: starting with highest ranked
/* 					UI.WriteLine("area remaining for {0} = {1}", selectedPrescription.Prescription.Name, 
																selectedPrescription.AreaRemainingToHarvest); */
                    
					

					selectedPrescription.HarvestHighestRankedStand();
					Stand stand = selectedPrescription.HighestRankedStand;
					
					
					
					
					if (stand != null) {
						//if there was a stand-adjacency constraint on this stand, enforce:
			 			foreach (IRequirement r in selectedPrescription.Prescription.StandRankingMethod.Requirements) {
							//look for stand-adacency constraint in list r ranking methods
							if (r.ToString() == "Landis.Harvest.StandAdjacency") {
								StandAdjacency sa = (StandAdjacency) r;
								//set-aside every stand in this stand's neighbor-list for the specified number of years
								
								//IF siteselection = some type of spreading, freeze the spread-list of neighbors
								if (selectedPrescription.Prescription.SiteSelectionMethod.ToString() == "Landis.Harvest.CompleteStandSpreading" 								|| selectedPrescription.Prescription.SiteSelectionMethod.ToString() == "Landis.Harvest.PartialStandSpreading") {
									//freeze every stand in the neighbor list
									StandSpreading ss = (StandSpreading) selectedPrescription.Prescription.SiteSelectionMethod;
									//if it's a spreading, go through the UnharvestedNeighbors list that was built during the site-selection spread
									foreach (Stand n_stand in ss.UnharvestedNeighbors) {
											//UI.WriteLine("SPREAD setting aside {0}", n_stand.MapCode);
											n_stand.SetAsideUntil(Model.Core.CurrentTime + sa.SetAside);
									}
								}
								else {
									//if it's not a spreading, just take all of the stand's neighbors
									foreach (Stand n_stand in stand.Neighbors) {
											//UI.WriteLine("NON-SPREAD setting aside {0}", n_stand.MapCode);
											n_stand.SetAsideUntil(Model.Core.CurrentTime + sa.SetAside);
									}
								}
								//found and implemented the stand adjacency, so break out of the requirements list
								break;
							}
						}
					}
					else {
						//UI.WriteLine("returned a null stand");
					}
					
                    //  Check each prescription to see if there's at least one  unharvested stand that the prescription ranks higher than  0.  
					// The list is traversed in reverse order, so that the removal of items doesn't mess up the traversal.
                    for (int i = activePrescriptions.Count - 1; i >= 0; --i) {
                        if (! activePrescriptions[i].AnyUnharvestedStandsRankedAbove0) {
							//UI.WriteLine("removing1 {0}", activePrescriptions[i].Prescription.Name);
                            activePrescriptions.RemoveAt(i);
						}
                    }
					
                }
                else {
                    for (int i = activePrescriptions.Count - 1; i >= 0; --i) {
						//UI.WriteLine("removing2 {0}", activePrescriptions[i].Prescription.Name);
                        activePrescriptions.RemoveAt(i);
                    }
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