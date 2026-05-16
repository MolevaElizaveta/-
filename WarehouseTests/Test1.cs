using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WarehouseTests
{
    [TestClass]
    public class Test1
    {
        [TestMethod]
        public void TestIsWorking()
        {
            Assert.IsTrue(true, "Тест работает");
        }

        [TestMethod]
        public void TestAddition()
        {
            int a = 2;
            int b = 3;
            int result = a + b;
            Assert.AreEqual(5, result);
        }

        [TestMethod]
        public void TestStringNotNull()
        {
            string test = "hello";
            Assert.IsNotNull(test);
        }

        [TestMethod]
        public void TestStringEmpty()
        {
            string test = "";
            Assert.AreEqual(0, test.Length);
        }
    }
}