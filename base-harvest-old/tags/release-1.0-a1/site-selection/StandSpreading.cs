using Edu.Wisc.Forest.Flel.Util;

namespace Landis.Harvest
{
    /// <summary>
    /// A site-selection method that spreads to neighboring stands based on
    /// their rankings.
    /// </summary>
    public abstract class StandSpreading
    {
        private StandRanking[] rankings;

        //---------------------------------------------------------------------

        /// <summary>
        /// The rankings of all the stands in the management area that the
        /// site-selection method is currently being applied to.
        /// </summary>
        public StandRanking[] StandRankings
        {
            get {
                return rankings;
            }

            set {
                rankings = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Gets the ranking for an unharvested stand from among the whole set
        /// of stand rankings.
        /// </summary>
        public StandRanking GetRanking(Stand stand)
        {
            //  Search backward through the stand rankings because unharvested
            //  stands are at the end of the list.
            for (int i = rankings.Length - 1; i >= 0; i--) {
                if (rankings[i].Stand == stand)
                    return rankings[i];
            }
            throw new System.ApplicationException("ERROR: Stand not found in rankings");
        }

        //---------------------------------------------------------------------

        public static void ValidateTargetSize(InputValue<double> targetSize)
        {
            if (targetSize.Actual < 0)
                throw new InputValueException(targetSize.String,
                                              "Target harvest size cannot be negative");
        }
    }
}
