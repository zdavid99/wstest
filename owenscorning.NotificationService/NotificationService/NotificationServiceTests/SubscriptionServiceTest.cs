using OwensCorning.NotificationService;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace NotificationServiceTests
{
    
    
    /// <summary>
    ///This is a test class for SubscriptionServiceTest and is intended
    ///to contain all SubscriptionServiceTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SubscriptionServiceTest
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
        ///A test for Unsubscribe
        ///</summary>
        [TestMethod()]
        public void UnsubscribeTest()
        {
            string email = Constants.testEmail;
            string site = Constants.site;
            bool expected = true;
            bool actual;
            actual = SubscriptionService.Unsubscribe(email, site);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Subscribe
        ///</summary>
        [TestMethod()]
        public void SubscribeTest()
        {
            string email = Constants.testEmail;
            string site = Constants.site;
            bool expected = true;
            bool actual;
            actual = SubscriptionService.Subscribe(email, site);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for GetCurrentSubscribers
        ///</summary>
        [TestMethod()]
        public void GetCurrentSubscribersTest()
        {
            string site = Constants.site;
            int expectedCount = 1;
            List<Subscription> actual;
            actual = SubscriptionService.GetCurrentSubscribers(site);
            Assert.AreEqual(expectedCount, actual.Count);
        }

        /// <summary>
        ///A test for Subscribe
        ///</summary>
        [TestMethod()]
        public void SubscribeTest1()
        {
            string email = Constants.testEmail;
            string site = Constants.site;
            string firstName = Constants.testFirstName;
            string lastName = Constants.testLastName;
            bool expected = true;
            bool actual;
            actual = SubscriptionService.Subscribe(email, site, firstName, lastName);
            Assert.AreEqual(expected, actual);
        }
    }
}
