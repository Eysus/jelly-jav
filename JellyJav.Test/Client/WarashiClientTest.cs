using JellyJav.Plugin.Client;
using JellyJav.Plugin.Entity;

namespace JellyJav.Test.Client
{
    [TestClass]
    public class WarashiClientTest
    {
        private WarashiClient client;

        public WarashiClientTest()
        {
            client = new();
        }

        [TestMethod]
        public async Task SearchForActresses_LastFirst()
        {
            IEnumerable<Plugin.SearchResult.ActressResult> results = await client.SearchForActresses("Sasaki Aki").ConfigureAwait(false);
            Assert.AreEqual("Aki Sasaki", results.ElementAt(0).Name);
            Assert.AreEqual("s-2-0/2714", results.ElementAt(0).Id);
            Assert.AreEqual(
                new Uri("http://warashi-asian-pornstars.fr/WAPdB-img/pornostars-f/a/k/2714/aki-sasaki/preview/mini/wapdb-aki-sasaki-pornostar-asiatique.warashi-asian-pornstars.fr.jpg"),
                results.ElementAt(0).Cover);
        }

        [TestMethod]
        public async Task SearchForActresses_FirstLast()
        {
            IEnumerable<Plugin.SearchResult.ActressResult> results = await client.SearchForActresses("Maria Nagai").ConfigureAwait(false);
            Assert.AreEqual("Maria Nagai", results.ElementAt(0).Name);
            Assert.AreEqual("s-2-0/3743", results.ElementAt(0).Id);
            Assert.AreEqual(
                new Uri("http://warashi-asian-pornstars.fr/WAPdB-img/pornostars-f/m/a/3743/maria-nagai/preview/mini/wapdb-maria-nagai-pornostar-asiatique.warashi-asian-pornstars.fr.jpg"),
                results.ElementAt(0).Cover);
        }

        [TestMethod]
        public async Task LoadActress_Ok()
        {
            Actress? result = await client.LoadActress("s-2-0/2714").ConfigureAwait(false);

            Actress expected = new Actress(
                id: "s-2-0/2714",
                name: "Aki Sasaki",
                birthdate: DateTime.Parse("December 24, 1979"),
                birthplace: "Japan, Saitama prefecture",
                cover: "http://warashi-asian-pornstars.fr/WAPdB-img/pornostars-f/a/k/2714/aki-sasaki/profil-0/large/wapdb-aki-sasaki-pornostar-asiatique.warashi-asian-pornstars.fr.jpg");

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public async Task LoadActress_Invalid()
        {
            Actress? result = await client.LoadActress("invalid").ConfigureAwait(false);
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task SearchForActress_Ok()
        {
            Actress? result = await client.SearchForActress("Hiyori Yoshioka").ConfigureAwait(false);

            Actress expected = new Actress(
                id: "s-2-0/3806",
                name: "Hiyori Yoshioka",
                birthdate: DateTime.Parse("August 08, 1999"),
                birthplace: "Japan",
                cover: "http://warashi-asian-pornstars.fr/WAPdB-img/pornostars-f/h/i/3806/hiyori-yoshioka/profil-0/large/wapdb-hiyori-yoshioka-pornostar-asiatique.warashi-asian-pornstars.fr.jpg");

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public async Task SearchForActress_FemalePornstar()
        {
            Actress? result = await client.SearchForActress("Ruka Aoi").ConfigureAwait(false);

            // Parsing for female-pornstar results isn't done yet.
            Actress? expected = new Actress(
                id: "s-4-1/14028",
                name: "Ruka Aoi - 藍井る加",
                birthdate: null,
                birthplace: null,
                cover: "http://warashi-asian-pornstars.fr/WAPdB-img/pornostars-f/r/u/786/ruka-aoi/preview/mini/wapdb-ruka-aoi-pornostar-asiatique.warashi-asian-pornstars.fr.jpg");

            Assert.AreEqual(expected, result);
        }
    }
}
