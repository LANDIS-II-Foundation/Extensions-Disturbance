using Landis.Harvest;
using NUnit.Framework;

namespace Landis.Test.Harvest
{
    [TestFixture]
    public class AgeRange_Test
    {
        [Test]
        public void Overlap_R1BeforeR2()
        {
            AgeRange range1 = new AgeRange(10, 20);
            AgeRange range2 = new AgeRange(      21, 50);
            Assert.IsFalse(range1.Overlaps(range2));
        }

        //---------------------------------------------------------------------

        [Test]
        public void Overlap_E1InR2()
        {
            AgeRange range1 = new AgeRange(10,   20);
            AgeRange range2 = new AgeRange(   15,   50);
            Assert.IsTrue(range1.Overlaps(range2));
        }

        //---------------------------------------------------------------------

        [Test]
        public void Overlap_R1InR2()
        {
            AgeRange range1 = new AgeRange(   10, 20);
            AgeRange range2 = new AgeRange(5,        50);
            Assert.IsTrue(range1.Overlaps(range2));
        }

        //---------------------------------------------------------------------

        [Test]
        public void Overlap_S1InR2()
        {
            AgeRange range1 = new AgeRange(   10,    20);
            AgeRange range2 = new AgeRange(5,     15);
            Assert.IsTrue(range1.Overlaps(range2));
        }

        //---------------------------------------------------------------------

        [Test]
        public void Overlap_R1AfterR2()
        {
            AgeRange range1 = new AgeRange(       100, 200);
            AgeRange range2 = new AgeRange(15, 50);
            Assert.IsFalse(range1.Overlaps(range2));
        }

        //---------------------------------------------------------------------

        [Test]
        public void Overlap_R1IncludesR2()
        {
            AgeRange range1 = new AgeRange(10,       200);
            AgeRange range2 = new AgeRange(   15, 50);
            Assert.IsTrue(range1.Overlaps(range2));
        }

        //---------------------------------------------------------------------

        [Test]
        public void Overlap_E1IsS2()
        {
            AgeRange range1 = new AgeRange(10, 20);
            AgeRange range2 = new AgeRange(    20, 50);
            Assert.IsTrue(range1.Overlaps(range2));
        }

        //---------------------------------------------------------------------

        [Test]
        public void Overlap_S1IsE2()
        {
            AgeRange range1 = new AgeRange(   10, 20);
            AgeRange range2 = new AgeRange(3, 10);
            Assert.IsTrue(range1.Overlaps(range2));
        }
    }
}
