namespace Landis.Harvest
{
    /// <summary>
    /// A ranking requirement which requires a stand be no more than a certain
    /// maximum age to be eligible for ranking.
    /// </summary>
    public class MaximumAge
        : IRankingRequirement
    {
        private ushort maxAge;

        //---------------------------------------------------------------------

        public MaximumAge(ushort age)
        {
            maxAge = age;
        }

        //---------------------------------------------------------------------

        bool IRankingRequirement.MetBy(Stand stand)
        {
            return stand.Age <= maxAge;
        }
    }
}
