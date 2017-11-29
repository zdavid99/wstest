using OwensCorning.NotificationService;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace NotificationServiceTests
{
    
    
    /// <summary>
    ///This is a test class for EmailServiceTest and is intended
    ///to contain all EmailServiceTest Unit Tests
    ///</summary>
    [TestClass()]
    public class EmailServiceTest
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
        ///A test for GetSentInfoBySiteAndBatchId
        ///</summary>
        [TestMethod()]
        public void GetSentInfoBySiteAndBatchIdTest()
        {
            EmailService target = new EmailService();
            string site = Constants.site;
            int batchId = 1;
            int expectedCount = 2;
            List<SentInfo> actual;
            actual = target.GetSentInfoBySiteAndBatchId(site, batchId);
            Assert.AreEqual(expectedCount, actual.Count);
        }

        /// <summary>
        ///A test for ResendEmail
        ///</summary>
        [TestMethod()]
        public void ResendEmailTest()
        {
            EmailService target = new EmailService();
            string site = Constants.site;
            string email = Constants.testEmail;
            int batchId = 1;
            bool actual;
            actual = target.ResendEmail(site, email, batchId);
            Assert.IsTrue(actual);
        }

        /// <summary>
        ///A test for SendEmailToRecipients
        ///</summary>
        [TestMethod()]
        public void SendEmailToRecipientsTest()
        {
            EmailService target = new EmailService(); 
            string site = Constants.site; 
            int batchId = 5;
            target.SendEmailToRecipients(site, batchId);
            Assert.IsTrue(true);
        }
    }
}
