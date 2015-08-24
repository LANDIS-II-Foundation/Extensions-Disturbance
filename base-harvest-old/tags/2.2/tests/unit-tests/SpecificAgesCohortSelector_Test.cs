using Landis.AgeCohort;
using Landis.Harvest;
using Landis.Species;
using NUnit.Framework;
using System.Collections.Generic;

namespace Landis.Test.Harvest
{
    [TestFixture]
    public class SpecificAgesCohortSelector_Test
    {
        private ISpecies abiebals;
        private SpecificAgesCohortSelector selector_25_50_75_100to200_300to500;
        private SpecificAgesCohortSelector selector_2to99_250;
        private SpeciesCohortBoolArray isHarvested;

        //---------------------------------------------------------------------

        [TestFixtureSetUp]
        public void Init()
        {
            abiebals = TestUtil.Species.SampleDataset["abiebals"];

            selector_25_50_75_100to200_300to500 = CreateSelector(25, 50, 75, null,
                                                                 100, 200,
                                                                 300, 500);
            selector_2to99_250 = CreateSelector(250, null,
                                                2, 99);

            isHarvested = new SpeciesCohortBoolArray();
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Creates a cohort selector based on a list of individual ages and
        /// age ranges.
        /// </summary>
        /// <param name="ages">
        /// Individual ages followed by a null then the age ranges.  Each range
        /// is represented by two parameters: start and end.
        /// </param>
        private SpecificAgesCohortSelector CreateSelector(params ushort?[] ages)
        {
            List<ushort> individualAges = new List<ushort>();
            int i = 0;
            for (i = 0; i < ages.Length; i++) {
                ushort? age = ages[i];
                if (age.HasValue)
                    individualAges.Add(age.Value);
                else
                    break;
            }
            i++;
            int countRemaining = ages.Length - i;
            Assert.IsTrue(countRemaining % 2 == 0);

            List<AgeRange> ranges = new List<AgeRange>();
            for (; i < ages.Length; i += 2) {
                ushort? start = ages[i];
                Assert.IsTrue(start.HasValue);
                ushort? end = ages[i+1];
                Assert.IsTrue(end.HasValue);
                ranges.Add(new AgeRange(start.Value, end.Value));
            }

            return new SpecificAgesCohortSelector(individualAges, ranges);
        }

        //---------------------------------------------------------------------

        private ISpeciesCohorts CreateCohorts(params ushort[] ages)
        {
            return TestUtil.AgeCohort.SpeciesCohorts.Create(abiebals, ages);
        }

        //---------------------------------------------------------------------

        private void SelectCohorts(ISpeciesCohorts            cohorts,
                                   SpecificAgesCohortSelector selector)
        {
            isHarvested.SetAllFalse(cohorts.Count);
            selector.SelectCohorts(cohorts, isHarvested);
        }

        //---------------------------------------------------------------------

        private void CheckIsHarvested(params bool[] expectedValues)
        {
            Assert.IsTrue(TestUtil.AgeCohort.SpeciesCohortBoolArray.Equals(isHarvested, expectedValues));
        }

        //---------------------------------------------------------------------

        [Test]
        public void NoCohorts()
        {
            ISpeciesCohorts cohorts = CreateCohorts();
            SelectCohorts(cohorts, selector_25_50_75_100to200_300to500);
            CheckIsHarvested();
        }

        //---------------------------------------------------------------------

        [Test]
        public void ThreeCohorts()
        {
            ISpeciesCohorts cohorts = CreateCohorts(110, 50, 30);
            SelectCohorts(cohorts, selector_25_50_75_100to200_300to500);
            CheckIsHarvested(true, true, false);
        }

        //---------------------------------------------------------------------

        [Test]
        public void ManyCohorts()
        {
            ISpeciesCohorts cohorts = CreateCohorts(501, 500, 300, 299, 201, 200, 155, 99, 75, 56, 50, 20);
            SelectCohorts(cohorts, selector_25_50_75_100to200_300to500);
            CheckIsHarvested(false, true, true, false, false, true, true, false, true, false, true, false);
        }

        //---------------------------------------------------------------------

        [Test]
        public void OneCohort_False()
        {
            ISpeciesCohorts cohorts = CreateCohorts(1);
            SelectCohorts(cohorts, selector_2to99_250);
            CheckIsHarvested(false);
        }

        //---------------------------------------------------------------------

        [Test]
        public void OneCohort_True()
        {
            ISpeciesCohorts cohorts = CreateCohorts(77);
            SelectCohorts(cohorts, selector_2to99_250);
            CheckIsHarvested(true);
        }

        //---------------------------------------------------------------------

        [Test]
        public void FourCohorts_False()
        {
            ISpeciesCohorts cohorts = CreateCohorts(251, 249, 100, 1);
            SelectCohorts(cohorts, selector_2to99_250);
            CheckIsHarvested(false, false, false, false);
        }

        //---------------------------------------------------------------------

        [Test]
        public void FourCohorts_True()
        {
            ISpeciesCohorts cohorts = CreateCohorts(250, 99, 50, 2);
            SelectCohorts(cohorts, selector_2to99_250);
            CheckIsHarvested(true, true, true, true);
        }
    }
}
