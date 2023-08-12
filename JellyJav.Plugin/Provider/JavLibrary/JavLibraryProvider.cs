using JellyJav.Plugin.Client;
using JellyJav.Plugin.Enumeration;
using JellyJav.Plugin.Util;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;
using Microsoft.Extensions.Logging;

namespace JellyJav.Providers.JavlibraryProvider
{
    /// <summary>The provider for Javlibrary videos.</summary>
    public class JavLibraryProvider : IRemoteMetadataProvider<Movie, MovieInfo>, IHasOrder
    {
        private readonly JavLibraryClient client;
        private readonly ILibraryManager libraryManager;
        private readonly ILogger<JavLibraryProvider> logger;

        /// <summary>Initializes a new instance of the <see cref="JavlibraryProvider"/> class.</summary>
        /// <param name="libraryManager">Instance of the <see cref="ILibraryManager" />.</param>
        /// <param name="logger">Instance of the <see cref="ILogger" />.</param>
        public JavLibraryProvider(
            ILibraryManager libraryManager,
            ILogger<JavLibraryProvider> logger)
        {
            this.libraryManager = libraryManager;
            this.logger = logger;
            client = new();
        }

        /// <inheritdoc />
        public string Name => "Javlibrary";

        /// <inheritdoc />
        public int Order => 100;

        /// <inheritdoc />
        public async Task<MetadataResult<Movie>> GetMetadata(MovieInfo info, CancellationToken cancelToken)
        {
            string originalTitle = Utility.GetVideoOriginalTitle(info, this.libraryManager);

            this.logger.LogInformation("[JellyJav] Javlibrary - Scanning: " + originalTitle);

            Plugin.Entity.Video? result;
            if (info.ProviderIds.ContainsKey("Javlibrary"))
            {
                result = await client.LoadVideo(info.ProviderIds["Javlibrary"]).ConfigureAwait(false);
            }
            else
            {
                string? code = Utility.ExtractCodeFromFilename(originalTitle);
                if (code is null)
                {
                    return new MetadataResult<Movie>();
                }

                result = await client.SearchVideo(code).ConfigureAwait(false);
            }

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
                    ProviderIds = new Dictionary<string, string> { { "Javlibrary", result.Id } },
                    Studios = new[] { result.Studio },
                    Genres = result.Genres.ToArray(),
                },
                People = CreateActressList(result),
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
            return await client.GetClient().GetAsync(url, cancelToken).ConfigureAwait(false);
        }

        private static string NormalizeActressName(string name)
        {
            if (Plugin.Plugin.Instance?.Configuration.ActressNameOrder == ActressNameOrder.FIRST_LAST)
            {
                return string.Join(" ", name.Split(' ').Reverse());
            }

            return name;
        }

        private static List<PersonInfo> CreateActressList(Plugin.Entity.Video video)
        {
            if (Plugin.Plugin.Instance?.Configuration.EnableActresses == false)
            {
                return new List<PersonInfo>();
            }

            return (from actress in video.Actresses
                    select new PersonInfo
                    {
                        Name = NormalizeActressName(actress),
                        Type = PersonType.Actor,
                    }).ToList();
        }
    }
}
