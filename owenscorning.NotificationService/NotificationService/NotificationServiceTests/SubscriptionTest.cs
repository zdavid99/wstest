using NotificationService;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace NotificationServiceTests
{
    
    
    /// <summary>
    ///This is a test class for SubscriptionTest and is intended
    ///to contain all SubscriptionTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SubscriptionTest
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
            string email = "matthew.oberlin@hansoninc.com";
            string site = "www.owenscorning.com";
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
            string email = "matthew.oberlin@hansoninc.com";
            string site = "www.owenscorning.com";
            bool expected = true;
            bool actual;
            actual = SubscriptionService.Subscribe(email, site);
            Assert.AreEqual(expected, actual);
        }
    }
}
