using System.Text.RegularExpressions;
using System.Web;
using AngleSharp;
using AngleSharp.Dom;
using JellyJav.Plugin.Entity;
using JellyJav.Plugin.SearchResult;
using JellyJav.Plugin.Util;

namespace JellyJav.Plugin.Client
{
    /// <summary>A web scraping client for javlibrary.com.</summary>
    public class JAVHDPornClient : ClientBase
    {
        private const string BASE_URL = "https://www.javhdporn.net";
        private const string BASE_SEARCH = "/search/";
        private const string BASE_VIDEO = "/video/";
        private readonly string[] CLEAN_TITLE = { "(English Subtitle)" };

        /// <summary>
        ///     Searches by the specified identifier.
        /// </summary>
        /// <param name="identifier">The identifier to search for.</param>
        /// <returns>An array of tuples representing each video returned during the search.</returns>
        public async Task<IEnumerable<VideoResult>> SearchVideos(string identifier)
        {
            IDocument doc = await LoadPage($"{BASE_URL}{BASE_SEARCH}{identifier}").ConfigureAwait(false);

            List<VideoResult> videos = new List<VideoResult>();

            foreach (IElement n in doc.QuerySelectorAll(".loop-video"))
            {
                string? code = Regex.Match(n.QuerySelector(".entry-header span")?.TextContent ?? "", "FC2[\\s-]?PPV[\\s-]?\\d+").Value.Replace(" ", "-");
                if (code is null) continue;

                string id = code.ToLower();
                Uri url = new Uri($"{BASE_URL}{BASE_VIDEO}" + id);

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
            IDocument doc = await LoadPage($"{BASE_URL}{BASE_SEARCH}{code}").ConfigureAwait(false);

            if (doc.QuerySelector("#main .widget-title")?.TextContent == "Nothing found")
            {
                return null;
            }

            Uri url = new Uri(doc.QuerySelector("#main .archive-entry")?.GetAttribute("href"));

            return await LoadVideo(url).ConfigureAwait(false);
        }

        /// <summary>Loads a specific JAV by id.</summary>
        /// <param name="id">The JavLibrary spcific JAV identifier.</param>
        /// <returns>The parsed video, or null if no video with <c>id</c> exists.</returns>
        public async Task<Video?> LoadVideo(string id)
        {
            return await LoadVideo(new Uri($"{BASE_URL}{BASE_VIDEO}" + id)).ConfigureAwait(false);
        }

        private async Task<IDocument> LoadPage(string url)
        {
            HttpResponseMessage response = await httpClient.GetAsync(url).ConfigureAwait(false);
            string html = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return await context.OpenAsync(req => req.Content(html)).ConfigureAwait(false);
        }

        private Video? ParseVideoPage(IDocument doc)
        {
            string? url = doc.QuerySelector(".sharing-buttons a")?.GetAttribute("href");
            if (url == null) return null;

            url = HttpUtility.ParseQueryString(new Uri(url).Query)["u"];

            string[]? segments = url.Trim("/").Split('/');
            string? id = segments?.Last();
            string? code = id.ToUpper();

            string rawTitle = doc.QuerySelector(".post h1").TextContent;
            string? title = Regex.Replace(rawTitle, "FC2[\\s-]?PPV[\\s-]?\\d+", "").Trim();
            title = Regex.Replace(title, "[A-Z]+[\\s-]?\\d+", "").Trim();
            foreach (string toRemove in CLEAN_TITLE)
            {
                title = title.Replace(toRemove, "").Trim();
            }

            title = title.TrimRegex("[^A-Za-z0-9]");

            if (id is null || code is null)
            {
                return null;
            }

            string? cover = doc.QuerySelector("#video-player img")?.GetAttribute("src");

            return new Video(
                id: id,
                code: code,
                title: title ?? "NOT FOUND",
                actresses: new List<string>(),
                genres: new List<string>(),
                studio: null,
                boxArt: null,
                cover: cover,
                releaseDate: null); // TODO
        }
    }
}
