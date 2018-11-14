using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AUT.MintaHf1
{
    /// <summary>
    /// A tesztek strukturalasa a feladattol fugg. Itt egy osztalyban van ket teszt, mert
    /// ugyanazt a feladatot ellenorzik, csak mas aspektusbol.
    /// </summary>
    [TestClass]
    public class UnitTests
    {
        public static string HallgatoMegoldasa;

        [ClassInitialize]
        public static void ClassInit(TestContext testContext)
        {
            // Elvarjuk, hogy itt legyen a megoldas. A /megoldas konyvar a Docker kontenerbe automatikusan belekerul.
            var megoldasFajl = "/megoldas/megoldas.txt";

            // Elozetes ellenorzesek. A tesztek (ebben az osztalyban) se le futnak, ha ez sikertelen.
            if(!System.IO.File.Exists(megoldasFajl))
                throw new Exception("Nem letezik a megoldasokat tartalmazo TXT fajl");

            HallgatoMegoldasa = System.IO.File.ReadAllText(megoldasFajl);
        }

        [TestMethod]
        public void NemUresFajl()
        {
            Assert.IsTrue(!string.IsNullOrEmpty(HallgatoMegoldasa), "Ures megoldas");
        }

        [TestMethod]
        public void Megoldas42()
        {
            Assert.IsTrue(HallgatoMegoldasa.Contains("42"), "Helytelen megoldas");
        }
    }
}
