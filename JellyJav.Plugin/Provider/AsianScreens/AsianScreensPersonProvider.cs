using JellyJav.Plugin.Client;
using JellyJav.Plugin.Entity;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Providers;
using Microsoft.Extensions.Logging;

namespace JellyJav.Providers.AsianscreensProvider
{
    /// <summary>The provider for Asianscreens actresses.</summary>
    public class AsianScreensPersonProvider : IRemoteMetadataProvider<Person, PersonLookupInfo>
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private static readonly AsianScreensClient client = new();
        private readonly ILogger<AsianScreensPersonProvider> logger;

        /// <summary>Initializes a new instance of the <see cref="AsianscreensPersonProvider"/> class.</summary>
        /// <param name="logger">Instance of the <see cref="ILogger" />.</param>
        public AsianScreensPersonProvider(ILogger<AsianScreensPersonProvider> logger)
        {
            this.logger = logger;
        }

        /// <inheritdoc />
        public string Name => "Asianscreens";

        /// <inheritdoc />
        public async Task<IEnumerable<RemoteSearchResult>> GetSearchResults(PersonLookupInfo info, CancellationToken cancelationToken)
        {
            return from actress in await client.SearchForActresses(info.Name).ConfigureAwait(false)
                   select new RemoteSearchResult
                   {
                       Name = actress.Name,
                       ProviderIds = new Dictionary<string, string>
                       {
                           { "Asianscreens", actress.Id },
                       },
                       ImageUrl = actress.Cover?.ToString(),
                   };
        }

        /// <inheritdoc />
        public async Task<MetadataResult<Person>> GetMetadata(PersonLookupInfo info, CancellationToken cancellationToken)
        {
            this.logger.LogInformation("[JellyJav] Asianscreens - Scanning: " + info.Name);

            Actress? actress;
            if (info.ProviderIds.ContainsKey("Asianscreens"))
            {
                actress = await client.LoadActress(info.ProviderIds["Asianscreens"]).ConfigureAwait(false);
            }
            else
            {
                actress = await client.SearchForActress(info.Name).ConfigureAwait(false);
            }

            if (actress is null)
            {
                return new MetadataResult<Person>();
            }

            return new MetadataResult<Person>
            {
                // Changing the actress name but still keeping them associated with
                // their videos will be a challenge.
                Item = new Person
                {
                    ProviderIds = new Dictionary<string, string> { { "Asianscreens", actress.Id } },
                    PremiereDate = actress.Birthdate,
                    ProductionLocations = new[] { actress.Birthplace }.Where(i => i is string).ToArray(),

                    // Jellyfin will always refresh metadata unless Overview exists.
                    // So giving Overview a zero width character to prevent that.
                    Overview = "\u200B",
                },
                HasMetadata = true,
            };
        }

        /// <inheritdoc />
        public async Task<HttpResponseMessage> GetImageResponse(string url, CancellationToken cancelToken)
        {
            return await httpClient.GetAsync(url, cancelToken).ConfigureAwait(false);
        }
    }
}
