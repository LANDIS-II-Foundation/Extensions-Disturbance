using Edu.Wisc.Forest.Flel.Util;
using Landis.Harvest;
using NUnit.Framework;

using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Landis.Test.Harvest
{
    [TestFixture]
    public class ParametersParser_Test
    {
        private Species.IDataset speciesDataset;
        private const int startTime = 1950;
        private const int endTime =   2400;
        private ParametersParser parser;
        private LineReader reader;

        //---------------------------------------------------------------------

        [TestFixtureSetUp]
        public void Init()
        {
            Species.DatasetParser speciesParser = new Species.DatasetParser();
            reader = OpenFile("Species.txt");
            try {
                speciesDataset = speciesParser.Parse(reader);
            }
            finally {
                reader.Close();
            }

            parser = new ParametersParser(//speciesDataset,
                                          startTime,
                                          endTime);
        }

        //---------------------------------------------------------------------

        private FileLineReader OpenFile(string filename)
        {
            string path = Data.MakeInputPath(filename);
            return Landis.Data.OpenTextFile(path);
        }

        //---------------------------------------------------------------------

        private IParameters ParseFile(string filename)
        {
            try {
                reader = OpenFile(filename);
                return parser.Parse(reader);
            }
            finally {
                reader.Close();
            }
        }

        //---------------------------------------------------------------------

        private string GetFileAssociatedWithTest(StackFrame currentTest)
        {
            //  Get the name of the input file associated with the test that's
            //  currently running.  The file's name is "{test}.txt".
            MethodBase testMethod = currentTest.GetMethod();
            string testName = testMethod.Name;
            string filename = testName + ".txt";
            return filename;
        }

        //---------------------------------------------------------------------

        [Test]
        public void GoodFile()
        {
            string filename = GetFileAssociatedWithTest(new StackFrame(0));
            IParameters parameters = ParseFile(filename);
            Assert.IsNotNull(parameters);
        }

        //---------------------------------------------------------------------

        private void TryParse(string filename)
        {
            int? errorLineNum = Testing.FindErrorMarker(Data.MakeInputPath(filename));
            try {
                reader = OpenFile(filename);
                IParameters parameters = parser.Parse(reader);
            }
            catch (System.Exception e) {
                Data.Output.WriteLine();
                Data.Output.WriteLine(e.Message.Replace(Data.Directory, Data.DirPlaceholder));
                LineReaderException lrExc = e as LineReaderException;
                if (lrExc != null && errorLineNum.HasValue)
                    Assert.AreEqual(errorLineNum.Value, lrExc.LineNumber);
                throw;
            }
            finally {
                reader.Close();
            }
        }

        //---------------------------------------------------------------------

        private void TryParseFileAssociatedWithTest()
        {
            //  This line:
            //
            //  string filename = GetFileAssociatedWithTest(new StackFrame(1));
            //
            //  doesn't work with the release configuration.
            MethodBase testMethod = new StackFrame(1).GetMethod();
            string testName = testMethod.Name;
            string filename = testName + ".txt";
            TryParse(filename);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void LandisData_WrongValue()
        {
            TryParseFileAssociatedWithTest();
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void Timestep_Missing()
        {
            TryParseFileAssociatedWithTest();
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void Timestep_Negative()
        {
            TryParseFileAssociatedWithTest();
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void ManagementAreas_Missing()
        {
            TryParseFileAssociatedWithTest();
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void ManagementAreas_Empty()
        {
            TryParseFileAssociatedWithTest();
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void ManagementAreas_Whitespace()
        {
            TryParseFileAssociatedWithTest();
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void Stands_Missing()
        {
            TryParseFileAssociatedWithTest();
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void Stands_Empty()
        {
            TryParseFileAssociatedWithTest();
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void Stands_Whitespace()
        {
            TryParseFileAssociatedWithTest();
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void Prescription_NameMissing()
        {
            TryParseFileAssociatedWithTest();
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void Prescription_NameRepeated()
        {
            TryParseFileAssociatedWithTest();
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void StandRanking_Missing()
        {
            TryParseFileAssociatedWithTest();
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void StandRanking_Unknown()
        {
            TryParseFileAssociatedWithTest();
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void SiteSelection_Missing()
        {
            TryParseFileAssociatedWithTest();
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void SiteSelection_Complete_Extra()
        {
            TryParseFileAssociatedWithTest();
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void SiteSelection_CompleteSpread_Negative()
        {
            TryParseFileAssociatedWithTest();
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void CohortSelection_Missing()
        {
            TryParseFileAssociatedWithTest();
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void CohortSelection_Unknown()
        {
            TryParseFileAssociatedWithTest();
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void HarvestImpl_MgmtArea_CodeTooBig()
        {
            TryParseFileAssociatedWithTest();
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void HarvestImpl_MgmtArea_NegativeCode()
        {
            TryParseFileAssociatedWithTest();
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void HarvestImpl_Prescription_Missing()
        {
            TryParseFileAssociatedWithTest();
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void HarvestImpl_Prescription_AppliedTwice()
        {
            TryParseFileAssociatedWithTest();
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void HarvestImpl_Prescription_Unknown()
        {
            TryParseFileAssociatedWithTest();
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void HarvestImpl_Area_Missing()
        {
            TryParseFileAssociatedWithTest();
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void HarvestImpl_Area_Negative()
        {
            TryParseFileAssociatedWithTest();
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void HarvestImpl_Area_TooBig()
        {
            TryParseFileAssociatedWithTest();
        }
    }
}
