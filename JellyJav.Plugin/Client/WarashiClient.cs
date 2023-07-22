namespace JellyJav.Plugin.Client
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Net.Http;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using AngleSharp;
    using AngleSharp.Dom;
    using JellyJav.Plugin.Entity;
    using JellyJav.Plugin.SearchResult;

    /// <summary>A web scraping client for warashi-asian-pornstars.fr.</summary>
    public class WarashiClient : ClientBase
    {
        /// <summary>Searches for an actress by name.</summary>
        /// <param name="searchName">The actress name to search for.</param>
        /// <returns>A list of all actresses found.</returns>
        public async Task<IEnumerable<ActressResult>> SearchForActresses(string searchName)
        {
            FormUrlEncodedContent form = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string?, string?>("recherche_critere", "f"),
                new KeyValuePair<string?, string?>("recherche_valeur", searchName),
                new KeyValuePair<string?, string?>("x", "0"),
                new KeyValuePair<string?, string?>("y", "0"),
            });

            HttpResponseMessage response = await httpClient.PostAsync("http://warashi-asian-pornstars.fr/en/s-12/search", form).ConfigureAwait(false);
            string html = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            IDocument doc = await context.OpenAsync(req => req.Content(html)).ConfigureAwait(false);

            List<ActressResult> actresses = new List<ActressResult>();

            foreach (IElement row in doc.QuerySelectorAll(".resultat-pornostar"))
            {
                string? name = NormalizeName(row.QuerySelector("p")?.TextContent.Split('-')[0].Trim());
                string? id = ExtractId(row.QuerySelector("a")?.GetAttribute("href") ?? string.Empty);
                string cover = "http://warashi-asian-pornstars.fr" + row.QuerySelector("img")?.GetAttribute("src");

                if (name is null || id is null)
                {
                    continue;
                }

                actresses.Add(new ActressResult
                {
                    Name = name,
                    Id = id,
                    Cover = new Uri(cover),
                });
            }

            return actresses;
        }

        /// <summary>Same as <see cref="Actress" />, but parses and returns the first found actress.</summary>
        /// <param name="searchName">The actress name to search for.</param>
        /// <returns>The first actress found.</returns>
        public async Task<Actress?> SearchForActress(string searchName)
        {
            FormUrlEncodedContent form = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string?, string?>("recherche_critere", "f"),
                new KeyValuePair<string?, string?>("recherche_valeur", searchName),
                new KeyValuePair<string?, string?>("x", "0"),
                new KeyValuePair<string?, string?>("y", "0"),
            });

            HttpResponseMessage response = await httpClient.PostAsync("http://warashi-asian-pornstars.fr/en/s-12/search", form).ConfigureAwait(false);
            string html = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            IDocument doc = await context.OpenAsync(req => req.Content(html)).ConfigureAwait(false);

            string? id = ExtractId(doc.QuerySelector(".correspondance_exacte a")?.GetAttribute("href") ?? string.Empty);
            if (id is null)
            {
                return null;
            }

            return await LoadActress(id).ConfigureAwait(false);
        }

        /// <summary>Finds and parses an actress by id.</summary>
        /// <param name="id">The actress' asianscreens.com identifier.</param>
        /// <returns>The parsed actress.</returns>
        public async Task<Actress?> LoadActress(string id)
        {
            string[] parsedId = id.Split('/');
            if (parsedId.Length != 2)
            {
                return null;
            }

            return await LoadActress(new Uri($"http://warashi-asian-pornstars.fr/en/{parsedId[0]}/anything/anything/{parsedId[1]}")).ConfigureAwait(false);
        }

        /// <summary>Finds and parses an actress by url.</summary>
        /// <param name="url">The actress' asianscreens.com absolute url.</param>
        /// <returns>The parsed actress.</returns>
        private async Task<Actress?> LoadActress(Uri url)
        {
            HttpResponseMessage response = await httpClient.GetAsync(url).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            string html = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            IDocument doc = await context.OpenAsync(req => req.Content(html)).ConfigureAwait(false);

            string? id = ExtractId(url.ToString());
            string name = NormalizeName(doc.QuerySelector("#pornostar-profil [itemprop=name]")?.TextContent) ??
                       NormalizeName(doc.QuerySelector("#main h1")?.TextContent);
            DateTime? birthdate = ExtractBirthdate(doc);
            string birthplace = doc.QuerySelector("[itemprop=birthPlace]")?.TextContent.Trim();
            string cover = ExtractCover(doc);

            if (id is null || name is null)
            {
                return null;
            }

            return new Actress(
                id: id,
                name: name,
                birthdate: birthdate,
                birthplace: birthplace,
                cover: cover);
        }

        private DateTime? ExtractBirthdate(IDocument doc)
        {
            string? bd = doc.QuerySelector("[itemprop=birthDate]")?.GetAttribute("content");
            if (bd != null)
            {
                return DateTime.Parse(bd);
            }

            return null;
        }

        private string? ExtractId(string url)
        {
            Match match = Regex.Match(url, @"\/en\/(.+?)\/.+\/(\d+)$");

            if (!match.Success)
            {
                return null;
            }

            return $"{match.Groups[1].Value}/{match.Groups[2].Value}";
        }

        private string? NormalizeName(string? name)
        {
            if (name is null)
            {
                return null;
            }

            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(name.ToLower()).Trim();
        }

        private string? ExtractCover(IDocument doc)
        {
            // First try asian-female-pornstar
            string? cover = doc.QuerySelector("#pornostar-profil-photos-0 [itemprop=image]")?.GetAttribute("src") ??
                        doc.QuerySelector("#casting-profil-preview [itemprop=image]")?.GetAttribute("src");

            if (cover == null)
            {
                return null;
            }

            return "http://warashi-asian-pornstars.fr" + cover;
        }
    }
}
