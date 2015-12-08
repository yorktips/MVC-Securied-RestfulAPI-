using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShawInterviewExercise.API.Controllers;
using ShawInterviewExercise.API.Models;

namespace ShawInterviewExercise.API.Tests.Models
{
    /// <summary>
    /// Summary description for ShowControllerTest
    /// </summary>
    [TestClass]
    public class CommonServicesTest
    {
        public CommonServicesTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }


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
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        //[DataSource(@"Provider=System.Data.EntityClient; Data Source=(LocalDB)\v11.0;attachdbfilename=|DataDirectory|\ShawInterviewDb.mdf;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework;", "Numbers")]
        [TestMethod]
        public void BinaryToBase64Test()
        {
            //
            // TODO: Add test logic here
            //
            string userid = "admin";
            string token = Authenticate.GetAuthToken(userid);
            Assert.IsNotNull(token);
            Assert.IsTrue(Authenticate.Auth(token));

            userid = "user";
            token = Authenticate.GetAuthToken(userid);
            Assert.IsNotNull(token);
            Assert.IsFalse(Authenticate.Auth(token));

        }
    }
}
