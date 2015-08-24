using Edu.Wisc.Forest.Flel.Util;

namespace Landis.Harvest
{
    /// <summary>
    /// A site-selection method that selects small non-contiguous collections
    /// of sites within a stand.
    /// </summary>
    public class PatchCutting
    {
        public static void ValidatePercentage(InputValue<Percentage> percentage)
        {
            if (percentage.Actual < 0 || percentage.Actual > 1.0)
                throw new InputValueException(percentage.String,
                                              percentage.String + " is not between 0% and 100%");
        }

        //---------------------------------------------------------------------

        public static void ValidateSize(InputValue<double> size)
        {
            if (size.Actual < 0)
                throw new InputValueException(size.String,
                                              "Patch size cannot be negative");
        }
    }
}
