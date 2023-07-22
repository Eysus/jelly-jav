using System.Globalization;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using AngleSharp;
using AngleSharp.Dom;
using JellyJav.Plugin.Entity;
using JellyJav.Plugin.SearchResult;

namespace JellyJav.Plugin.Client
{
    public class AsianScreensClient : ClientBase
    {
        private const string BASE_URL = "https://www.asianscreens.com";
        private const string NO_PICTURE = "/products/400000/portraits/no_picture_available.gif";

        private static readonly Regex IdFromUrl = new Regex(@".*\/(.*)\.asp", RegexOptions.Compiled);

        public static bool Ping()
        {
            Ping ping = new();
            PingReply result = ping.Send(BASE_URL);
            return result.Status == IPStatus.Success;
        }

        /// <summary>
        /// Finds and parses an actress by id.
        /// </summary>
        /// <param name="id">The actress' asianscreens.com identifier.</param>
        /// <returns>The parsed actress.</returns>
        public async Task<Actress?> LoadActress(string id)
        {
            return await LoadActress(new Uri($"{BASE_URL}/{id}.asp")).ConfigureAwait(false);
        }

        /// <summary>Finds and parses an actress by url.</summary>
        /// <param name="url">The actress' asianscreens.com absolute url.</param>
        /// <returns>The parsed actress.</returns>
        public async Task<Actress?> LoadActress(Uri url)
        {
            HttpResponseMessage response = await httpClient.GetAsync(url).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            string html = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            IDocument doc = await context.OpenAsync(req => req.Content(html)).ConfigureAwait(false);

            string id, name;
            try
            {
                id = ExtractId(url) ?? throw new Exception("Id is null");
                name = ExtractCell(doc, "Name: ") ?? throw new Exception("Name is null");
            }
            catch
            {
                return null;
            }
            DateTime? birthdate = ExtractBirthdate(doc);
            string? birthplace = ExtractCell(doc, "Birthplace: ");
            string? cover = GetCover(doc);

            return new Actress(
                id: id,
                name: name,
                birthdate: birthdate,
                birthplace: birthplace,
                cover: cover);
        }

        /// <summary>
        /// Same as <see cref="Actress" />, but parses and returns the first found actress.
        /// </summary>
        /// <param name="searchName">The actress name to search for.</param>
        /// <returns>The first actress found.</returns>
        public async Task<Actress?> SearchForActress(string searchName)
        {
            IEnumerable<ActressResult> result = await SearchForActresses(searchName).ConfigureAwait(false);
            if (!result.Any() || result.First().Id == null) return null;
            return await LoadActress(result.First().Id!).ConfigureAwait(false);
        }

        /// <summary>
        /// Search actress results for a given name.
        /// </summary>
        /// <param name="actressName">Name of the actress, can be partial (only last/first name) or full, regardless order</param>
        /// <returns>A list of actresses who match the name</returns>
        public async Task<IEnumerable<ActressResult>> SearchForActresses(string actressName)
        {
            IEnumerable<ActressResult> result = await SearchHelper(actressName).ConfigureAwait(false);

            string reversedName = ReverseName(actressName);
            if (actressName != reversedName)
            {
                IEnumerable<ActressResult> reverseResult = await SearchHelper(reversedName).ConfigureAwait(false);
                result = result.Concat(reverseResult);
            }
            return result;
        }

        private static DateTime? ExtractBirthdate(IDocument doc)
        {
            string? birthdate = ExtractCell(doc, "DOB: ");
            return birthdate == null ? null : DateTime.Parse(birthdate, CultureInfo.InvariantCulture);
        }

        private static string? ExtractCell(IDocument doc, string cellName)
        {
            string? cell = doc.QuerySelectorAll("td")
                          ?.FirstOrDefault(n => n.TextContent == cellName)
                          ?.NextElementSibling
                          ?.TextContent;

            return cell == "n/a" ? null : cell;
        }

        private static string? ExtractId(Uri url)
        {
            Match match = IdFromUrl.Match(url.ToString());
            return match.Success ? match.Groups[1].Value : null;
        }

        private static Uri GenerateCoverUrl(string id)
        {
            char idEnd = id[^1];
            string picEnd = idEnd == '2' ? string.Empty : (char.GetNumericValue(idEnd) - 1).ToString();

            string url = string.Format(
                "{0}/products/400000/portraits/{1}{2}.jpg",
                BASE_URL,
                id.TrimEnd(idEnd),
                picEnd);

            return new Uri(url);
        }

        private static string? GetCover(IDocument doc)
        {
            string? path = doc.QuerySelector("img[src*=\"/products/400000/portraits/\"]")?
                          .GetAttribute("src");

            return path == NO_PICTURE ? null : $"{BASE_URL}{path}";
        }

        private async Task<IEnumerable<ActressResult>> SearchHelper(string searchName)
        {
            HttpResponseMessage response = await httpClient.GetAsync($"{BASE_URL}/directory/{searchName[0]}.asp").ConfigureAwait(false);

            if (!response.IsSuccessStatusCode) return Array.Empty<ActressResult>();

            string html = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            IDocument doc = await context.OpenAsync(req => req.Content(html)).ConfigureAwait(false);

            IEnumerable<IElement>? actressRows = doc.QuerySelectorAll("table[bgcolor='#000000']")
                                 .First(n => n.InnerHtml.Contains("ACTRESS"))
                                 .QuerySelectorAll("tr:not(:last-child)")
                                 .Skip(2);

            if (actressRows is null) return Array.Empty<ActressResult>();

            List<ActressResult> actresses = new List<ActressResult>();

            foreach (IElement row in actressRows)
            {
                string? name = row.QuerySelector("td:nth-child(1)")?.TextContent;
                string? id = row.QuerySelector("a")
                            ?.GetAttribute("href")
                            ?.TrimStart('/')
                            .Replace(".asp", string.Empty);
                if (name is null || id is null ||
                    (!name.Split(' ').Contains(searchName, StringComparer.CurrentCultureIgnoreCase) && Regex.Replace(name, @" #\d", string.Empty) != searchName))
                {
                    continue;
                }

                Uri cover = GenerateCoverUrl(id);

                actresses.Add(new ActressResult
                {
                    Name = name,
                    Id = id,
                    Cover = cover,
                });
            }

            return actresses;
        }
    }
}
