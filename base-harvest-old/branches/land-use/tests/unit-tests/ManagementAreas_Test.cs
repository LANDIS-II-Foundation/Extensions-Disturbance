using Landis.Harvest;
using NUnit.Framework;
using System.Collections.Generic;

namespace Landis.Test.Harvest
{
    [TestFixture]
    public class ManagementAreas_Test
    {
        [Test]
        public void MapCodesAsString_Null()
        {
            Assert.AreEqual("", ManagementAreas.MapCodesToString(null));
        }

        //---------------------------------------------------------------------

        private List<ushort> MakeUShortList(params int[] mapCodes)
        {
            List<ushort> list = new List<ushort>(mapCodes == null
                                                 ? 0
                                                 : mapCodes.Length);
            foreach (int mapCode in mapCodes)
                list.Add((ushort) mapCode);
            return list;
        }

        //---------------------------------------------------------------------

        [Test]
        public void MapCodesAsString_Zero()
        {
            List<ushort> mapCodes = MakeUShortList();
            Assert.AreEqual("", ManagementAreas.MapCodesToString(mapCodes));
        }

        //---------------------------------------------------------------------

        [Test]
        public void MapCodesAsString_One()
        {
            List<ushort> mapCodes = MakeUShortList(123);
            Assert.AreEqual("123", ManagementAreas.MapCodesToString(mapCodes));
        }

        //---------------------------------------------------------------------

        [Test]
        public void MapCodesAsString_TwoSingles()
        {
            List<ushort> mapCodes = MakeUShortList(123, 8);
            Assert.AreEqual("8, 123", ManagementAreas.MapCodesToString(mapCodes));
        }

        //---------------------------------------------------------------------

        [Test]
        public void MapCodesAsString_TwoConsecutive()
        {
            List<ushort> mapCodes = MakeUShortList(78, 79);
            Assert.AreEqual("78, 79", ManagementAreas.MapCodesToString(mapCodes));
        }

        //---------------------------------------------------------------------

        [Test]
        public void MapCodesAsString_OneRange()
        {
            List<ushort> mapCodes = MakeUShortList(99, 101, 100);
            Assert.AreEqual("99-101", ManagementAreas.MapCodesToString(mapCodes));
        }

        //---------------------------------------------------------------------

        [Test]
        public void MapCodesAsString_Many()
        {
            List<ushort> mapCodes = MakeUShortList(124, 120, 122, 123, 121, 0,
                                                   5, 6, 8, 44, 43, 42, 40);
            Assert.AreEqual("0, 5, 6, 8, 40, 42-44, 120-124", ManagementAreas.MapCodesToString(mapCodes));
        }
    }
}
