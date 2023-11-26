using System.Web;
using AngleSharp;
using AngleSharp.Dom;
using JellyJav.Plugin.Entity;
using JellyJav.Plugin.SearchResult;
using JellyJav.Plugin.Util;

namespace JellyJav.Plugin.Client
{
    /// <summary>A web scraping client for javlibrary.com.</summary>
    public class JavLibraryClient : ClientBase
    {
        private const string BASE_URL = "https://www.javlibrary.com";

        /// <summary>
        ///     Searches by the specified identifier.
        /// </summary>
        /// <param name="identifier">The identifier to search for.</param>
        /// <returns>An array of tuples representing each video returned during the search.</returns>
        public async Task<IEnumerable<VideoResult>> SearchVideos(string identifier)
        {
            IDocument doc = await LoadPage($"{BASE_URL}/en/vl_searchbyid.php?keyword={identifier}").ConfigureAwait(false);

            // When there is only one result, the search redirect to the result page.
            if (doc.QuerySelector("#video_id") != null)
            {
                string? resultCode = doc.QuerySelector("#video_id .text")?.TextContent;
                if (resultCode is null) return Array.Empty<VideoResult>();

                string url = BASE_URL + doc.QuerySelector("#video_title a")?.GetAttribute("href");
                string? id = HttpUtility.ParseQueryString(new Uri(url).Query)?.Get("v");
                if (id is null) return Array.Empty<VideoResult>();

                return new[] { new VideoResult(resultCode, id) };
            }

            List<VideoResult> videos = new List<VideoResult>();

            foreach (IElement n in doc.QuerySelectorAll(".video"))
            {
                string? code = n.QuerySelector(".id")?.TextContent;
                if (code is null) continue;

                Uri url = new Uri($"{BASE_URL}/en/" + n.QuerySelector("a")?.GetAttribute("href"));
                string? id = HttpUtility.ParseQueryString(url.Query)?.Get("v");
                if (id is null) continue;

                videos.Add(new VideoResult(code, id));
            }

            return videos;
        }

        /// <summary>Loads a specific JAV by url.</summary>
        /// <param name="url">The JAV url.</param>
        /// <returns>The parsed video, or null if no video at <c>url</c> exists.</returns>
        public async Task<Video?> LoadVideo(Uri url)
        {
            HttpResponseMessage response = await httpClient.GetAsync(url).ConfigureAwait(false);
            string html = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            IDocument doc = await context.OpenAsync(req => req.Content(html)).ConfigureAwait(false);
            return ParseVideoPage(doc);
        }

        /// <summary>Searches for a specific JAV code, and returns the first result.</summary>
        /// <param name="code">The JAV to search for.</param>
        /// <returns>The first result of the search, or null if nothing was found.</returns>
        public async Task<Video?> SearchVideo(string code)
        {
            IDocument doc = await LoadPage($"{BASE_URL}/en/vl_searchbyid.php?keyword={code}").ConfigureAwait(false);

            if (doc.QuerySelector("p em")?.TextContent == "Search returned no result." ||
                doc.QuerySelector("#badalert td")?.TextContent == "The search term you entered is invalid. Please try a different term.")
            {
                return null;
            }

            // if only one result was found, and so we were taken directly to the video page.
            if (doc.QuerySelector("#video_id") != null) return ParseVideoPage(doc);

            return await LoadVideo(new Uri($"{BASE_URL}/en/" + doc.QuerySelector(".video a")?.GetAttribute("href"))).ConfigureAwait(false);
        }

        /// <summary>Loads a specific JAV by id.</summary>
        /// <param name="id">The JavLibrary spcific JAV identifier.</param>
        /// <returns>The parsed video, or null if no video with <c>id</c> exists.</returns>
        public async Task<Video?> LoadVideo(string id)
        {
            return await LoadVideo(new Uri($"{BASE_URL}/en/?v=" + id)).ConfigureAwait(false);
        }

        private async Task<IDocument> LoadPage(string url)
        {
            HttpResponseMessage response = await httpClient.GetAsync(url).ConfigureAwait(false);
            string html = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return await context.OpenAsync(req => req.Content(html)).ConfigureAwait(false);
        }

        private Video? ParseVideoPage(IDocument doc)
        {
            string? id = HttpUtility.ParseQueryString(
                new Uri(BASE_URL + doc.QuerySelector("#video_title a")?.GetAttribute("href")).Query)["v"];
            string? code = doc.QuerySelector("#video_id .text")?.TextContent;
            if (id is null || code is null)
            {
                return null;
            }

            IEnumerable<string> actresses = doc.QuerySelectorAll(".star a").Select(n => n.TextContent);
            string title = doc.QuerySelector("#video_title a")
                           ?.TextContent
                           .Replace(code, string.Empty)
                           .TrimStart(' ')
                           .Trim(actresses.FirstOrDefault())
                           .Trim(ReverseName(actresses.FirstOrDefault() ?? string.Empty))
                           .Trim() ?? string.Empty;

            IEnumerable<string> genres = doc.QuerySelectorAll(".genre a").Select(n => n.TextContent);
            string? studio = doc.QuerySelector("#video_maker a")?.TextContent;
            string? boxArt = doc.QuerySelector("#video_jacket_img")?.GetAttribute("src");

            if (boxArt != null && !boxArt.StartsWith("https:"))
            {
                boxArt = "https:" + boxArt;
            }

            string? cover = boxArt?.Replace("pl.jpg", "ps.jpg");

            return new Video(
                id: id,
                code: code,
                title: title,
                actresses: actresses,
                genres: genres,
                studio: studio,
                boxArt: boxArt,
                cover: cover,
                releaseDate: null); // TODO
        }
    }
}
