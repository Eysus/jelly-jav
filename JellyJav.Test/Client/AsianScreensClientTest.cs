using FluentAssertions;
using JellyJav.Plugin.Client;
using JellyJav.Plugin.Entity;
using JellyJav.Plugin.SearchResult;

namespace JellyJav.Test.Client
{
    [TestClass]
    public class AsianScreensClientTest
    {
        private AsianScreensClient client;

        public AsianScreensClientTest()
        {
            client = new();
        }

        [TestMethod]
        public async Task SearchForActresses_MultipleResults()
        {
            IEnumerable<ActressResult> result = await client.SearchForActresses("Ai Nanase").ConfigureAwait(false);
            result.Should().HaveCountGreaterThan(0);
        }

        [TestMethod]
        public async Task SearchForActresses_PartialName()
        {
            IEnumerable<ActressResult> result = await client.SearchForActresses("Ai").ConfigureAwait(false);
            result.Should().HaveCountGreaterThan(0);
        }

        [TestMethod]
        public async Task SearchForActresses_EmptyResult()
        {
            IEnumerable<ActressResult> result = await client.SearchForActresses("AZERTY").ConfigureAwait(false);
            result.Should().HaveCount(0);
        }

        [TestMethod]
        public async Task SearchForActresses_FindByLastnameFirst()
        {
            IEnumerable<ActressResult> result = await client.SearchForActresses("Sasaki Aki").ConfigureAwait(false);
            result.Should().HaveCount(1);
            result.First().Name.Should().Be("Aki Sasaki");
        }

        [TestMethod]
        public async Task SearchForActresses_FindByFirstnameFirst()
        {
            IEnumerable<ActressResult> result = await client.SearchForActresses("Aki Sasaki").ConfigureAwait(false);
            result.Should().HaveCount(1);
            result.First().Name.Should().Be("Aki Sasaki");
        }

        [TestMethod]
        public async Task SearchForActresses_InvalidQuery()
        {
            IEnumerable<ActressResult> result = await client.SearchForActresses("æŒç”°æ žé‡Œ").ConfigureAwait(false);
            result.Should().HaveCount(0);
        }

        [TestMethod]
        public async Task SearchForActress_Ok()
        {
            Actress? result = await client.SearchForActress("Ai Uehara").ConfigureAwait(false);

            Actress expected = new Actress(
                id: "ai_uehara2",
                name: "Ai Uehara",
                birthdate: DateTime.Parse("1992-11-22"),
                birthplace: null,
                cover: "https://www.asianscreens.com/products/400000/portraits/ai_uehara.jpg");

            result.Should().Be(expected);
        }

        [TestMethod]
        public async Task SearchForActress_Empty()
        {
            Actress? result = await client.SearchForActress("test").ConfigureAwait(false);
            result.Should().BeNull();
        }

        [TestMethod]
        public async Task LoadActress_Ok()
        {
            Actress? result = await this.client.LoadActress("koharu_suzuki2").ConfigureAwait(false);

            Actress expected = new Actress(
                id: "koharu_suzuki2",
                name: "Koharu Suzuki",
                birthdate: DateTime.Parse("1993-12-1"),
                birthplace: "Kanagawa",
                cover: "https://www.asianscreens.com/products/400000/portraits/koharu_suzuki.jpg");

            result.Should().Be(expected);
        }

        [TestMethod]
        public async Task LoadActress_MinimalMetadata()
        {
            Actress? result = await this.client.LoadActress("amika_tsuboi2").ConfigureAwait(false);

            Actress expected = new Actress(
                id: "amika_tsuboi2",
                name: "Amika Tsuboi",
                birthdate: null,
                birthplace: null,
                cover: null);

            result.Should().Be(expected);
        }
    }
}
