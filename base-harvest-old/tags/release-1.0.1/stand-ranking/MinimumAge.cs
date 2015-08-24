namespace Landis.Harvest
{
    /// <summary>
    /// A ranking requirement which requires a stand be at least a certain
    /// minimum age to be eligible for ranking.
    /// </summary>
    public class MinimumAge
        : IRequirement
    {
        private ushort minAge;

        //---------------------------------------------------------------------

        public MinimumAge(ushort age)
        {
            minAge = age;
        }

        //---------------------------------------------------------------------

        bool IRequirement.MetBy(Stand stand)
        {
            return minAge <= stand.Age;
        }
    }
}
