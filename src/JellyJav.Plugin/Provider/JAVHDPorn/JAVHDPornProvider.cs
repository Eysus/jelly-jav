using JellyJav.Plugin.Client;
using JellyJav.Plugin.Util;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Providers;
using Microsoft.Extensions.Logging;

namespace JellyJav.Providers.JAVHDPorn
{
    /// <summary>The provider for Javlibrary videos.</summary>
    public class JAVHDPornProvider : IRemoteMetadataProvider<Movie, MovieInfo>, IHasOrder
    {
        private readonly JAVHDPornClient client;
        private readonly ILibraryManager libraryManager;
        private readonly ILogger<JAVHDPornProvider> logger;

        /// <summary>Initializes a new instance of the <see cref="JavlibraryProvider"/> class.</summary>
        /// <param name="libraryManager">Instance of the <see cref="ILibraryManager" />.</param>
        /// <param name="logger">Instance of the <see cref="ILogger" />.</param>
        public JAVHDPornProvider(
            ILibraryManager libraryManager,
            ILogger<JAVHDPornProvider> logger)
        {
            this.libraryManager = libraryManager;
            this.logger = logger;
            client = new();
        }

        /// <inheritdoc />
        public string Name => "JAVHDPorn";

        /// <inheritdoc />
        public int Order => 110;

        /// <inheritdoc />
        public async Task<MetadataResult<Movie>> GetMetadata(MovieInfo info, CancellationToken cancelToken)
        {
            string originalTitle = Utility.GetVideoOriginalTitle(info, this.libraryManager);
            this.logger.LogInformation("[JellyJav] JAVHDPorn - Scanning: " + originalTitle);

            Plugin.Entity.Video? result;
            if (info.ProviderIds.ContainsKey("JAVHDPorn"))
            {
                result = await client.LoadVideo(info.ProviderIds["JAVHDPorn"]).ConfigureAwait(false);
            }
            else
            {
                string? code = Utility.ExtractFC2CodeFromFilename(originalTitle);

                if (code is null)
                {
                    return new MetadataResult<Movie>();
                }

                result = await client.SearchVideo(code).ConfigureAwait(false);
            }

            this.logger.LogInformation("[JellyJav] JAVHDPorn - Result: " + result?.Code);

            if (result is null)
            {
                return new MetadataResult<Movie>();
            }

            return new MetadataResult<Movie>
            {
                Item = new Movie
                {
                    OriginalTitle = originalTitle,
                    Name = Utility.CreateVideoDisplayName(result),
                    ProviderIds = new Dictionary<string, string> { { "JAVHDPorn", result.Id } }
                },
                HasMetadata = true,
            };
        }

        /// <inheritdoc />
        public async Task<IEnumerable<RemoteSearchResult>> GetSearchResults(MovieInfo info, CancellationToken cancelToken)
        {
            return from video in await client.SearchVideos(info.Name).ConfigureAwait(false)
                   select new RemoteSearchResult
                   {
                       Name = video.Code,
                       ProviderIds = new Dictionary<string, string>
                       {
                           { "Javlibrary", video.Id },
                       },
                   };
        }

        /// <inheritdoc />
        public async Task<HttpResponseMessage> GetImageResponse(string url, CancellationToken cancelToken)
        {
            this.logger.LogInformation("[JellyJav] JAVHDPorn - Url: " + url);
            return await client.GetClient().GetAsync(url, cancelToken).ConfigureAwait(false);
        }
    }
}
