using FluentAssertions;
using JellyJav.Plugin.Client;
using JellyJav.Plugin.Entity;

namespace JellyJav.Test.Client
{
    [TestClass]
    public class JavlibraryClientTest
    {
        private readonly JavLibraryClient client;

        public JavlibraryClientTest()
        {
            client = new();
        }

        [TestMethod]
        public async Task SearchVideos_MultipleResults()
        {
            IEnumerable<Plugin.SearchResult.VideoResult> results = await client.SearchVideos("abp").ConfigureAwait(false);

            Assert.AreEqual(results.Count(), 20);
            Assert.AreEqual(results.ElementAt(5).Code, "ABP-006");
            Assert.AreEqual(results.ElementAt(5).Id, "javlijaqye");
        }

        [TestMethod]
        public async Task SearchVideos_SingleResult()
        {
            IEnumerable<Plugin.SearchResult.VideoResult> results = await client.SearchVideos("HND-723").ConfigureAwait(false);

            Assert.AreEqual(results.ElementAt(0).Code, "HND-723");
            Assert.AreEqual(results.ElementAt(0).Id, "javli6laqy");
        }

        [TestMethod]
        public async Task SearchVideos_SingleResultFourDigits()
        {
            IEnumerable<Plugin.SearchResult.VideoResult> results = await client.SearchVideos("GODR-1136").ConfigureAwait(false);

            Assert.AreEqual("GODR-1136", results.ElementAt(0).Code);
            Assert.AreEqual("javmemaqqa", results.ElementAt(0).Id);
        }

        [TestMethod]
        public async Task SearchVideo_Empty()
        {
            Video? result = await client.SearchVideo("AAA-111").ConfigureAwait(false);

            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public async Task SearchVideo_Invalid()
        {
            Video? result = await client.SearchVideo("259LUXU-1142").ConfigureAwait(false);

            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public async Task SearchVideo_Result()
        {
            Video? result = await client.SearchVideo("SSNI-230").ConfigureAwait(false);

            Video correct = new Video(
                id: "javli7bvzi",
                code: "SSNI-230",
                title: "Big Slap Brilliantly Seductive Ass Pub Miss",
                actresses: new[] { "Hoshino Nami" },
                genres: new[] { "Solowork", "Nasty, Hardcore", "Cowgirl", "Prostitutes", "Butt", "Risky Mosaic", "Huge Butt" },
                studio: "S1 NO.1 STYLE",
                boxArt: "https://pics.dmm.co.jp/mono/movie/adult/ssni230/ssni230pl.jpg",
                cover: "https://pics.dmm.co.jp/mono/movie/adult/ssni230/ssni230ps.jpg",
                releaseDate: null); // TODO

            Assert.AreEqual(correct, result);
        }

        [TestMethod]
        public async Task LoadVideo_NormalizeTitle()
        {
            Video? result = await client.LoadVideo("javli6lg24").ConfigureAwait(false);

            Video correct = new Video(
                id: "javli6lg24",
                code: "STARS-126",
                title: "A Big Ass Pantyhose Woman Who Is Exposed So Much That There Is No Plump",
                actresses: new[] { "Koizumi Hinata" },
                genres: new[] { "Cosplay", "Solowork", "Beautiful Girl", "Huge Butt" },
                studio: "SOD Create",
                boxArt: "https://pics.dmm.co.jp/mono/movie/adult/1stars126/1stars126pl.jpg",
                cover: "https://pics.dmm.co.jp/mono/movie/adult/1stars126/1stars126ps.jpg",
                releaseDate: DateTime.Parse("2019-09-26"));

            Assert.AreEqual(result, correct);
        }

        [TestMethod]
        public async Task LoadVideo_OneActress()
        {
            Video? result = await client.LoadVideo("javlio354u").ConfigureAwait(false);

            Video correct = new Video(
                id: "javlio354u",
                code: "ABP-002",
                title: "NEW TOKYO Style 01 Aika Phosphorus",
                actresses: new[] { "Aikarin" },
                genres: new[] { "Handjob", "Solowork", "Facials" },
                studio: "Prestige",
                boxArt: "https://pics.dmm.co.jp/mono/movie/adult/118abp002/118abp002pl.jpg",
                cover: "https://pics.dmm.co.jp/mono/movie/adult/118abp002/118abp002ps.jpg",
                releaseDate: null); // TODO

            Assert.AreEqual(result, correct);
        }

        [TestMethod]
        public async Task LoadVideo_ManyActresses()
        {
            Video? result = await client.LoadVideo("javli6bm5q").ConfigureAwait(false);

            Video correct = new Video(
                id: "javli6bm5q",
                code: "SDDE-592",
                title: "Room Boundaries-If It Were In This Way, I Would Like It!To",
                actresses: new[] { "Kurata Mao", "Mihara Honoka", "Kururugi Aoi" },
                genres: new[] { "Cosplay", "Planning", "Cum", "Hypnosis" },
                studio: "SOD Create",
                boxArt: "https://pics.dmm.co.jp/mono/movie/adult/1sdde592/1sdde592pl.jpg",
                cover: "https://pics.dmm.co.jp/mono/movie/adult/1sdde592/1sdde592ps.jpg",
                releaseDate: null); // TODO

            result.Should().Be(correct);
        }

        [TestMethod]
        public async Task LoadVideo_NoActresses()
        {
            Video? result = await client.LoadVideo("javliarg3u").ConfigureAwait(false);

            Video correct = new Video(
                id: "javliarg3u",
                code: "IPTD-041",
                title: "Goddesses Of The Speed Of Sound 01 RQ'S Cafe",
                actresses: Array.Empty<string>(),
                genres: new[] { "Mini Skirt", "Big Tits", "Slender", "Race Queen", "Digital Mosaic" },
                studio: "IDEA POCKET",
                boxArt: "https://pics.dmm.co.jp/mono/movie/adult/iptd041/iptd041pl.jpg",
                cover: "https://pics.dmm.co.jp/mono/movie/adult/iptd041/iptd041ps.jpg",
                releaseDate: null); // TODO

            Assert.AreEqual(result, correct);
        }
    }
}
