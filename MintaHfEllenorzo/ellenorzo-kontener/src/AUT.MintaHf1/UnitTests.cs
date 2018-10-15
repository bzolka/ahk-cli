using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AUT.MintaHf1
{
    /// <summary>
    /// Minden ellenorizendo aspektus egy fuggetlen unit teszt.
    /// </summary>
    [TestClass]
    public class UnitTests
    {
        [TestMethod]
        public void NemUresFajl()
        {
            var str = System.IO.File.ReadAllText(Init.MegoldasFajl);
            Assert.IsNotNull(str);
            Assert.IsTrue(!string.IsNullOrEmpty(str), "Ures fajl");
        }

        [TestMethod]
        public void Megoldas42()
        {
            var str = System.IO.File.ReadAllText(Init.MegoldasFajl);
            Assert.IsTrue(str.Contains("42"), "helytelen megoldas");
        }
    }
}
