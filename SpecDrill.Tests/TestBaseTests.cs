using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpecDrill.MsTest;

namespace SomeTests
{
    [TestClass]
    public class TestBaseTests : MsTestBase
    {
        [ClassInitialize]
        public static void ClassInitializer(TestContext testContext) => _ClassSetup(testContext);

        private int methodInitCount = 0;
        private int methodTearDownCount = 0;


        protected override void ScenarioSetup()
        {
            Assert.AreEqual(0, methodInitCount);
            methodInitCount++;
            Assert.AreEqual(1, methodInitCount);
        }

        protected override void ScenarioTeardown()
        {
            Assert.AreEqual(0, methodTearDownCount);
            methodTearDownCount++;
            Assert.AreEqual(1, methodTearDownCount);
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
