namespace Landis.Harvest
{
    /// <summary>
    /// A ranking requirement which requires a stand be at least a certain
    /// minimum age to be eligible for ranking.
    /// </summary>
    public class MinimumAge
        : IRankingRequirement
    {
        private ushort minAge;

        //---------------------------------------------------------------------

        public MinimumAge(ushort age)
        {
            minAge = age;
        }

        //---------------------------------------------------------------------

        bool IRankingRequirement.MetBy(Stand stand)
        {
            return minAge <= stand.Age;
        }
    }
}
