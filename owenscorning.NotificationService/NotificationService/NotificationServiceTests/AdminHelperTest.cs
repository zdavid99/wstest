using OwensCorning.NotificationService;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace NotificationServiceTests
{
    
    
    /// <summary>
    ///This is a test class for AdminHelperTest and is intended
    ///to contain all AdminHelperTest Unit Tests
    ///</summary>
    [TestClass()]
    public class AdminHelperTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for GetBatchesForSite
        ///</summary>
        [TestMethod()]
        public void GetBatchesForSiteTest()
        {
            string site = Constants.site;
            List<Batch> expected = null; // TODO: Initialize to an appropriate value
            List<Batch> actual;
            actual = AdminHelper.GetBatchesForSite(site);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetAllSitesWithAvailableData
        ///</summary>
        [TestMethod()]
        public void GetAllSitesWithAvailableDataTest()
        {
            List<string> expected = null; // TODO: Initialize to an appropriate value
            List<string> actual;
            actual = AdminHelper.GetAllSitesWithAvailableData();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
