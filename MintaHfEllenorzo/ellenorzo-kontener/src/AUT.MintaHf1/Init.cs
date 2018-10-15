using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AUT.MintaHf1
{
    [TestClass]
    public class Init
    {
        public static string MegoldasFajl;

        /// <summary>
        /// A legeloszor lefuto kod.
        /// Pelda: ellenorzi, hogy egy darab megoldast adott be a hallgato.
        /// </summary>
        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context)
        {
            // Docker kontenerben ebbe a mappaba kerul a hallgato megoldasa

            var megoldasFajlok = System.IO.Directory.GetFiles("/megoldas", "*.txt");
            if (megoldasFajlok.Length == 0)
                throw new Exception("Nem letezik a megoldasokat tartalmazo TXT fajl");
            if (megoldasFajlok.Length > 1)
                throw new Exception("Tul sok megoldasokat tartalmazo TXT fajl talalhato");

            // A hallgato megoldasat tartalmazo fajl. A unit tesztek ezt hasznaljak.
            MegoldasFajl = megoldasFajlok.First();
        }
    }
}
