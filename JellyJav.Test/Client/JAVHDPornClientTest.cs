using FluentAssertions;
using JellyJav.Plugin.Client;
using JellyJav.Plugin.Entity;

namespace JellyJav.Test.Client
{
    [TestClass]
    public class JAVHDPornClientTest
    {
        private readonly JAVHDPornClient client;

        public JAVHDPornClientTest()
        {
            client = new();
        }

        [TestMethod]
        public async Task SearchVideos_SingleResult()
        {
            IEnumerable<Plugin.SearchResult.VideoResult> results = await client.SearchVideos("FC2 PPV 525098").ConfigureAwait(false);

            Assert.AreEqual("FC2-PPV-525098", results.ElementAt(0).Code);
            Assert.AreEqual("fc2-ppv-525098", results.ElementAt(0).Id);
        }

        [TestMethod]
        public async Task SearchVideo_Empty()
        {
            Video? result = await client.SearchVideo("vsdgvergresg").ConfigureAwait(false);

            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public async Task SearchVideo_Result()
        {
            Video? result = await client.SearchVideo("fc2-ppv-1834271").ConfigureAwait(false);

            Video correct = new Video(
                id: "fc2-ppv-1834271",
                code: "FC2-PPV-1834271",
                title: "Harajuku G Cup 18-year-old charismatic president Gonzo back video leaked",
                actresses: new List<string>(),
                genres: new List<string>(),
                studio: null,
                boxArt: null,
                cover: null,
                releaseDate: null); // TODO

            Assert.AreEqual(correct.Id, result.Id);
            Assert.AreEqual(correct.Code, result.Code);
            Assert.AreEqual(correct.Title, result.Title);
        }

        [TestMethod]
        public async Task SearchVideo_ResultNoFC2()
        {
            Video? result = await client.SearchVideo("KTKL-007").ConfigureAwait(false);

            Video correct = new Video(
                id: "ktkl-007",
                code: "KTKL-007",
                title: "Meet Our Beloved Daughter. Because We Can’t Make A Living With Her Doing Sexy Costume",
                actresses: new List<string>(),
                genres: new List<string>(),
                studio: null,
                boxArt: null,
                cover: null,
                releaseDate: null); // TODO

            Assert.AreEqual(correct.Id, result.Id);
            Assert.AreEqual(correct.Code, result.Code);
            Assert.AreEqual(correct.Title, result.Title);
        }
    }
}
