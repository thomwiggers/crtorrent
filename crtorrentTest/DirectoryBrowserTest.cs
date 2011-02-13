using crtorrent;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
namespace crtorrentTest
{
    
    
    /// <summary>
    ///This is a test class for DirectoryBrowserTest and is intended
    ///to contain all DirectoryBrowserTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DirectoryBrowserTest
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
        ///A test for DirectoryBrowser Constructor
        ///</summary>
        [TestMethod()]
        [DeploymentItem("TestFiles")]
        public void DirectoryBrowserConstructorTest()
        {
            string path = @"TestDirectory";
            DirectoryBrowser target = new DirectoryBrowser(path);
            
            Assert.AreEqual(Path.GetFullPath(path), target.RootDirectory.FullName);
        }

        /// <summary>
        ///A test for getSubDirectory
        ///</summary>
        [TestMethod()]
        [DeploymentItem("TestFiles")]
        public void getSubDirectoryTest()
        {
            string path = "TestDirectory"; 
            DirectoryBrowser target = new DirectoryBrowser(path);
            string subdirectory = "TestDirectory1";
            string expected = (new DirectoryBrowser(@"TestDirectory\TestDirectory1")).RootDirectory.FullName;
            string actual;
            actual = target.getSubDirectory(subdirectory).RootDirectory.FullName;
            Assert.AreEqual(expected, actual);  
        }

        /// <summary>
        ///A test for getSubDirectories
        ///</summary>
        [TestMethod()]
        [DeploymentItem("TestFiles")]
        public void getSubDirectoriesTest()
        {
            string path = @"TestDirectory"; // TODO: Initialize to an appropriate value
            DirectoryBrowser target = new DirectoryBrowser(path); // TODO: Initialize to an appropriate value
            string[] expected = Directory.GetDirectories(Path.GetFullPath(path)); // TODO: Initialize to an appropriate value
            List<string> actualList = new List<string>();
            foreach (DirectoryBrowser b in target.getSubDirectories())
            {
                actualList.Add(b.RootDirectory.FullName);
            }
            string[] actual = actualList.ToArray();
            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
