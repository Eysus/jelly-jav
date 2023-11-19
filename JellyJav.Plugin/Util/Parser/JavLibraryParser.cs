using System.Web;
using AngleSharp.Dom;
using JellyJav.Plugin.Client;

namespace JellyJav.Plugin.Util.Parser
{
    public class JavLibraryParser
    {
        public JavLibraryParser(IDocument document)
        {
            Document = document;
            Parse();
        }

        public bool IsOk()
        {
            return !(Document.QuerySelector("p em")?.TextContent == "Search returned no result." ||
                Document.QuerySelector("#badalert td")?.TextContent == "The search term you entered is invalid. Please try a different term.");
        }

        private void Parse()
        {
            // Get ID

            Id = HttpUtility.ParseQueryString(
                new Uri(JavLibraryClient.BASE_URL + Document.QuerySelector("#video_title a")?.GetAttribute("href")).Query)["v"];

            // Video CODE
            Code = Document.QuerySelector("#video_id .text")?.TextContent;

            if (Id is null || Code is null)
            {
                return;
            }

            // ACTRESSES
            Actresses = Document.QuerySelectorAll(".star a").Select(n => n.TextContent);

            // TITLE
            Title = Document.QuerySelector("#video_title a")
                           ?.TextContent
                           .Replace(Code, string.Empty)
                           .TrimStart(' ')
                           .Trim(Actresses.FirstOrDefault())
                           .Trim(Utility.ReverseName(Actresses.FirstOrDefault() ?? string.Empty))
                           .Trim() ?? string.Empty;

            Genres = Document.QuerySelectorAll(".genre a").Select(n => n.TextContent);
            Studio = Document.QuerySelector("#video_maker a")?.TextContent;

            BoxArt = Document.QuerySelector("#video_jacket_img")?.GetAttribute("src");

            if (BoxArt != null && !BoxArt.StartsWith("https:"))
            {
                BoxArt = "https:" + BoxArt;
            }

            Cover = BoxArt?.Replace("pl.jpg", "ps.jpg");

            string releaseDateString = Document.QuerySelector("#video_date .text")
                           ?.TextContent
                           .TrimStart(' ')
                           .Trim() ?? string.Empty;
            ReleaseDate = DateTime.Parse(releaseDateString);
        }

        public IDocument Document { get; private set; }
        public string? Id { get; private set; }
        public string? Code { get; private set; }
        public string? Title { get; private set; }
        public IEnumerable<string>? Actresses { get; private set; }
        public IEnumerable<string>? Genres { get; private set; }
        public string? Studio { get; private set; }
        public string? BoxArt { get; private set; }
        public string? Cover { get; private set; }
        public DateTime ReleaseDate { get; private set; }

        public Entity.Video GetVideoData()
        {
            return new Entity.Video(
                id: Id,
                code: Code,
                title: Title,
                actresses: Actresses,
                genres: Genres,
                studio: Studio,
                boxArt: BoxArt,
                cover: Cover,
                releaseDate: ReleaseDate
            );
        }
    }
}
