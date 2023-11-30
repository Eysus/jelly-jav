using System.Globalization;
using AngleSharp.Dom;
using FluentAssertions;
using JellyJav.Plugin;
using JellyJav.Plugin.Entity;
using JellyJav.Plugin.SearchResult;
using Moq;

namespace JellyJav.Test.Unit.Util.Parser
{
    [TestClass]
    public class JavLibraryParserTest
    {
        [TestMethod]
        public void JavLibraryParser_ParseSearchResult_Test()
        {
            List<string> videosList = new() { "TEST-1", "TEST-2", "TEST-3", "TEST-4" };

            JavLibraryParser parser = new();
            IDocument page = Mock.Of<IDocument>();

            IHtmlCollection<IElement> videos = new MockHtmlCollection<IElement>(videosList.Select(n => CreateResultList(n, n.ToLower() + "test")).ToList());
            Mock.Get(page).Setup(d => d.QuerySelectorAll(".video")).Returns(videos);

            IEnumerable<VideoResult> result = parser.ParseSearchResult(page);

            result.Should().NotBeNull().And.NotBeEmpty();
            result.Should().HaveCount(videosList.Count);
            for (int i = 0; i < videosList.Count; i++)
            {
                result.ElementAt(i).Code.Should().Be(videosList.ElementAt(i));
                result.ElementAt(i).Id.Should().Be(videosList.ElementAt(i).ToLower() + "test");
            }
        }

        [TestMethod]
        public void JavLibraryParser_ParseSearchResult_SingleResult_Test()
        {
            // Arrange
            IDocument page = Mock.Of<IDocument>();
            Mock.Get(page).Setup(m => m.QuerySelector("#video_title a").GetAttribute("href")).Returns("/en/?v=javmemdb5q");
            Mock.Get(page).Setup(m => m.QuerySelector("#video_id")).Returns(new Mock<IElement>().Object); // Just need to be not null
            Mock.Get(page).Setup(m => m.QuerySelector("#video_id .text").TextContent).Returns("CODE-123");

            JavLibraryParser parser = new();
            IEnumerable<VideoResult> result = parser.ParseSearchResult(page);

            result.Should().NotBeNull().And.NotBeEmpty().And.HaveCount(1);
            result.ElementAt(0).Code.Should().Be("CODE-123");
            result.ElementAt(0).Id.Should().Be("javmemdb5q");
        }

        [TestMethod]
        public void JavLibraryParser_ParseVideoPage_Test()
        {
            List<string> actressesList = new List<string>() { "Yuuri Maina", "Sasaki Natsuna", "Fujiki Sae", "Kashii Kaho" };
            List<string> genresList = new List<string>() { "Tag 1", "Tag 2", "Tag 3", "Tag 4" };
            // Arrange
            IDocument page = Mock.Of<IDocument>();
            Mock.Get(page).Setup(m => m.QuerySelector("#video_title a").GetAttribute("href")).Returns("/en/?v=javmemdb5q");
            Mock.Get(page).Setup(m => m.QuerySelector("#video_id .text").TextContent).Returns("CODE-123");
            Mock.Get(page).Setup(m => m.QuerySelector("#video_title a").TextContent).Returns("CODE-123 My title");
            Mock.Get(page).Setup(m => m.QuerySelector("#video_date .text").TextContent).Returns("2023-10-03");
            Mock.Get(page).Setup(m => m.QuerySelector("#video_maker .text").TextContent).Returns("Movie Maker");
            Mock.Get(page).Setup(m => m.QuerySelector("#video_jacket img").GetAttribute("src")).Returns("imagepl.jpg");
            Mock.Get(page).Setup(m => m.QuerySelector("#video_jacket img").GetAttribute("src")).Returns("imagepl.jpg");

            IHtmlCollection<IElement> actresses = new MockHtmlCollection<IElement>(actressesList.Select(n => CreateListItem(n, "a")).ToList());
            Mock.Get(page).Setup(d => d.QuerySelectorAll(".star a")).Returns(actresses);
            IHtmlCollection<IElement> genres = new MockHtmlCollection<IElement>(genresList.Select(n => CreateListItem(n, "a")).ToList());
            Mock.Get(page).Setup(d => d.QuerySelectorAll(".genre a")).Returns(genres);
            JavLibraryParser parser = new();

            // Act
            Video video = parser.ParseVideoPage(page);

            // Assert
            parser.Should().NotBeNull();
            video.Should().NotBeNull();
            video.Id.Should().Be("javmemdb5q");
            video.Code.Should().Be("CODE-123");
            video.Title.Should().Be("My title");
            video.Studio.Should().Be("Movie Maker");
            video.BoxArt.Should().Be("imagepl.jpg");
            video.Cover.Should().Be("imageps.jpg");
            video.ReleaseDate.Should().Be(DateTime.Parse("2023-10-03", CultureInfo.InvariantCulture));
            video.Actresses.Should().NotBeNullOrEmpty().And.HaveCount(actressesList.Count).And.Contain(actressesList);
            video.Genres.Should().NotBeNullOrEmpty().And.HaveCount(genresList.Count).And.Contain(genresList);
        }

        [TestMethod]
        public void JavLibraryParser_ParseVideoPage_ActressNameInTitle_Test()
        {
            List<string> actressesList = new List<string>() { "Yuuri Maina" };

            // Arrange
            IDocument page = Mock.Of<IDocument>();
            Mock.Get(page).Setup(m => m.QuerySelector("#video_title a").GetAttribute("href")).Returns("/en/?v=javmemdb5q");
            Mock.Get(page).Setup(m => m.QuerySelector("#video_id .text").TextContent).Returns("CODE-123");
            Mock.Get(page).Setup(m => m.QuerySelector("#video_title a").TextContent).Returns("CODE-123 Yuuri Maina My title");

            IHtmlCollection<IElement> actresses = new MockHtmlCollection<IElement>(actressesList.Select(n => CreateListItem(n, "a")).ToList());
            Mock.Get(page).Setup(d => d.QuerySelectorAll(".star a")).Returns(actresses);
            JavLibraryParser parser = new();

            // Act
            Video video = parser.ParseVideoPage(page);

            // Assert
            parser.Should().NotBeNull();
            video.Should().NotBeNull();
            video.Title.Should().Be("My title");
        }

        [TestMethod]
        public void JavLibraryParser_ParseVideoPage_MissingData_Test()
        {
            List<string> actressesList = new List<string>() { "Yuuri Maina", "Sasaki Natsuna", "Fujiki Sae", "Kashii Kaho" };
            List<string> genresList = new List<string>() { "Tag 1", "Tag 2", "Tag 3", "Tag 4" };
            // Arrange
            IDocument page = Mock.Of<IDocument>();
            Mock.Get(page).Setup(m => m.QuerySelector("#video_title a").GetAttribute("href")).Returns("/en/?v=javmemdb5q");
            Mock.Get(page).Setup(m => m.QuerySelector("#video_id .text").TextContent).Returns("CODE-123");
            Mock.Get(page).Setup(m => m.QuerySelector("#video_title a").TextContent).Returns("CODE-123 My title");

            JavLibraryParser parser = new();

            // Act
            Video video = parser.ParseVideoPage(page);

            // Assert
            parser.Should().NotBeNull();
            video.Should().NotBeNull();
            video.Id.Should().Be("javmemdb5q");
            video.Code.Should().Be("CODE-123");
            video.Title.Should().Be("My title");
            video.Studio.Should().BeNull();
            video.BoxArt.Should().BeNull();
            video.Cover.Should().BeNull();
            video.ReleaseDate.Should().BeNull();
            video.Actresses.Should().BeEmpty();
            video.Genres.Should().BeEmpty();
        }

        private IElement CreateListItem(string text, string id)
        {
            Mock<IElement> elementMock = new Mock<IElement>();
            elementMock.Setup(e => e.TextContent).Returns(text);
            elementMock.Setup(e => e.GetAttribute("href")).Returns($"vl_star.php?s={id}");
            return elementMock.Object;
        }

        private IElement CreateResultList(string code, string id)
        {
            Mock<IElement> elementMock = new Mock<IElement>();
            elementMock.Setup(e => e.QuerySelector(".id").TextContent).Returns(code);
            elementMock.Setup(e => e.QuerySelector("a").GetAttribute("href")).Returns($"./?v={id}");
            return elementMock.Object;
        }
    }
}
