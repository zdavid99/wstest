using OwensCorning.NotificationService;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;

namespace NotificationServiceTests
{
    
    
    /// <summary>
    ///This is a test class for DocumentServiceTest and is intended
    ///to contain all DocumentServiceTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DocumentServiceTest
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

        ///// <summary>
        /////A test for GetPreviousDocumentsList
        /////</summary>
        //[TestMethod()]
        //public void GetPreviousDocumentsListTest1()
        //{
        //    DocumentService target = new DocumentService();
        //    int batchId = 1;
        //    int expectedCount = 30;
        //    List<UpdatedDoc> actual;
        //    actual = target.GetDocumentsList(Constants.site, batchId);
        //    Assert.AreEqual(expectedCount, actual.Count);
        //}

        ///// <summary>
        /////A test for GetPreviousDocumentsList
        /////</summary>
        //[TestMethod()]
        //public void GetPreviousDocumentsListTest()
        //{
        //    DocumentService target = new DocumentService();
        //    int expectedCount = 30;
        //    List<UpdatedDoc> actual;
        //    actual = target.GetDocumentsList(Constants.site);
        //    Assert.AreEqual(expectedCount, actual.Count);
        //}

        /// <summary>
        ///A test for GetLastBatchRun
        ///</summary>
        [TestMethod()]
        [DeploymentItem("OwensCorning.NotificationService.dll")]
        public void GetLastBatchRunTest()
        {
            DocumentService_Accessor target = new DocumentService_Accessor();
            Batch expected = new Batch();
            expected.batchId = 1;
            expected.startDate = DateTime.Now;
            expected.site = Constants.site;
            Batch actual;
            actual = target.GetLastBatchRun(Constants.site);
            Assert.AreEqual(expected.batchId, actual.batchId);
            Assert.AreEqual(expected.startDate.ToShortDateString(), actual.startDate.ToShortDateString());
            Assert.AreEqual(expected.site, actual.site);
        }

        /// <summary>
        ///A test for CreateNewBatch
        ///</summary>
        [TestMethod()]
        [DeploymentItem("OwensCorning.NotificationService.dll")]
        public void CreateNewBatchTest()
        {
            DocumentService_Accessor target = new DocumentService_Accessor();
            Batch expected = new Batch();
            expected.batchId = 1;
            expected.startDate = DateTime.Now;
            expected.site = Constants.site;
            Batch actual;
            actual = target.CreateNewBatch(Constants.site);
            Assert.AreEqual(expected.batchId, actual.batchId);
            Assert.AreEqual(expected.startDate.ToShortDateString(), actual.startDate.ToShortDateString());
            Assert.AreEqual(expected.site, actual.site);
        }

        /// <summary>
        ///A test for ProcessEktronDocuments
        ///</summary>
        [TestMethod()]
        public void ProcessEktronDocumentsTest()
        {
            throw new NotImplementedException("this is broken!");
            //DocumentService target = new DocumentService();
            //int expectedCount = 30;
            //List<UpdatedDoc> actual;
            //actual = target.ProcessEktronDocuments(Constants.site, "Document Categories");
            //Assert.AreEqual(expectedCount, actual.Count);
        }

        /// <summary>
        ///A test for GetLastBatchRunDate
        ///</summary>
        [TestMethod()]
        [DeploymentItem("OwensCorning.NotificationService.dll")]
        public void GetLastBatchRunDateTest()
        {
            DocumentService_Accessor target = new DocumentService_Accessor();
            DateTime expected = DateTime.Now;
            DateTime actual;
            actual = target.GetLastBatchRunDate(Constants.site);
            Assert.AreEqual(expected.ToShortDateString(), actual.ToShortDateString());
        }

        /// <summary>
        ///A test for GetDocumentsFromEktron
        ///</summary>
        [TestMethod()]
        [DeploymentItem("OwensCorning.NotificationService.dll")]
        public void GetDocumentsFromEktronTest()
        {
            DocumentService_Accessor target = new DocumentService_Accessor(); 
            string site = "commercial.owenscorning.com";
            DocumentProcessOptions processingOptions = new DocumentProcessOptions
                {
                    Site = site,
                    StartDate = DateTime.Now.AddDays(-90).Date,
                    EndDate = DateTime.Now.AddDays(-1).Date,
                    FileTypes = new List<string>{"pdf", "doc"},
                }; 
            //List<UpdatedDoc> expected = null; // TODO: Initialize to an appropriate value
            List<UpdatedDoc> actual;
            actual = target.GetDocumentsFromEktron(processingOptions);
            Assert.IsNotNull(actual);
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }



        ///// <summary>
        /////A test for CreateBatchForItems
        /////</summary>
        //[TestMethod()]
        //[DeploymentItem("OwensCorning.NotificationService.dll")]
        //public void CreateBatchForItemsTest()
        //{
        //    DocumentService_Accessor target = new DocumentService_Accessor(); // TODO: Initialize to an appropriate value
        //    string site = "commercial.owenscorning.com";
        //    DocumentProcessOptions processingOptions = new DocumentProcessOptions
        //    {
        //        StartDate = DateTime.Now.AddDays(-90).Date,
        //        EndDate = DateTime.Now.AddDays(-1).Date,
        //        FileTypes = new List<string> { "pdf", "doc" },
        //    };
        //    List<UpdatedDoc> itemsForBatch = target.GetDocumentsFromEktron(site, processingOptions);
        //    Batch actual;
        //    actual = target.CreateBatchForItems(site, itemsForBatch, processingOptions);
        //    Assert.IsNotNull(actual);
        //    //Assert.Inconclusive("Verify the correctness of this test method.");
        //}
    }
}
