using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpecDrill;
using SpecDrill.MsTest;

namespace SomeTests
{
    [TestClass]
    public class TestBaseTests : TestBase
    {
        
        private int methodInitCount = 0;
        private int methodTearDownCount = 0;

       
        public override void TestSetup()
        {
            base.TestSetup();
            Assert.AreEqual(0, methodInitCount);
            methodInitCount++;
            Assert.AreEqual(1, methodInitCount);
        }

        public override void TestCleanup()
        {
            Assert.AreEqual(0, methodTearDownCount);
            methodTearDownCount++;
            Assert.AreEqual(1, methodTearDownCount);
            base.TestCleanup();
        }

        [TestMethod]
        public void DummyTest1()
        {
            //Assert.IsTrue(classInit);
            Assert.AreEqual(1, methodInitCount);
            Assert.AreEqual(0, methodTearDownCount);
        }
        [TestMethod]
        public void DummyTest2()
        {
            Assert.AreEqual(1, methodInitCount);
            Assert.AreEqual(0, methodTearDownCount);
        }
        [TestMethod]
        public void DummyTest3()
        {
            Assert.AreEqual(1, methodInitCount);
            Assert.AreEqual(0, methodTearDownCount);
        }
    }
}
