using AngleSharp.Dom;
using JellyJav.Plugin.SearchResult;
using JellyJav.Plugin.Util;
using System;
using System.Globalization;
using System.Web;

namespace JellyJav.Plugin
{
    public class JavLibraryParser
    {
        public IEnumerable<VideoResult> ParseSearchResult(IDocument page)
        {
            // When there is only one result, the search redirect to the result page.
            if (page.QuerySelector("#video_id") != null)
            {
                string resultCode = page.QuerySelector("#video_id .text").TextContent;
                string id = page.QuerySelector("#video_title a")?.GetAttribute("href").Split("v=").Last();
                return new[] { new VideoResult(resultCode, id) };
            }

            List<VideoResult> result = new List<VideoResult>();
            foreach (IElement video in page.QuerySelectorAll(".video"))
            {
                string code = video.QuerySelector(".id").TextContent;
                string id = video.QuerySelector("a")?.GetAttribute("href").Split("v=").Last();
                result.Add(new VideoResult(code, id));
            }

            return result;
        }

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
