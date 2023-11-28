using AngleSharp.Dom;
using JellyJav.Plugin.Util;
using System.Globalization;

namespace JellyJav.Plugin
{
    public class JavLibraryParser
    {
        public Entity.Video? ParseVideoPage(IDocument page)
        {
            string id = page.QuerySelector("#video_title a").GetAttribute("href").Split("v=").Last();

            string code = page.QuerySelector("#video_id .text").TextContent.Trim();

            IEnumerable<string> actresses = page.QuerySelectorAll(".star a")?.Select(t => t.TextContent).ToList() ?? new();
            string title = page.QuerySelector("#video_title a")
                .TextContent
                .Replace(code, "").Trim()
                .Trim(actresses.FirstOrDefault()).Trim()
                .Trim(Utility.ReverseName(actresses.FirstOrDefault() ?? string.Empty))
                .Trim();
            string? studio = page.QuerySelector("#video_maker .text")?.TextContent.Trim();
            string? boxArt = page.QuerySelector("#video_jacket img")?.GetAttribute("src").Trim();
            string? releaseDate = page.QuerySelector("#video_date .text")?.TextContent.Trim();
            string? cover = boxArt?.Replace("pl.jpg", "ps.jpg").Trim();

            IEnumerable<string> genres = page.QuerySelectorAll(".genre a")?.Select(t => t.TextContent).ToList() ?? new();

            return new Entity.Video(
                id: id,
                code: code,
                title: title,
                actresses: actresses,
                genres: genres,
                studio: studio,
                boxArt: boxArt,
                cover: cover,
                releaseDate: releaseDate != null ? DateTime.Parse(releaseDate, CultureInfo.InvariantCulture) : null);
        }
    }
}
