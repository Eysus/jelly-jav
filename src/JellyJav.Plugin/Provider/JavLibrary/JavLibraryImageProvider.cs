using JellyJav.Plugin.Client;
using JellyJav.Plugin.Util;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;

namespace JellyJav.Providers.JavlibraryProvider
{
    /// <summary>The provider for Javlibrary video covers.</summary>
    public class JAVHDPornImageProvider : IRemoteImageProvider, IHasOrder
    {
        private static readonly JavLibraryClient client = new();

        /// <inheritdoc />
        public string Name => "JavLibrary";

        /// <inheritdoc />
        public int Order => 100;

        /// <inheritdoc />
        public async Task<IEnumerable<RemoteImageInfo>> GetImages(BaseItem item, CancellationToken cancelToken)
        {
            string? id = item.GetProviderId("Javlibrary");
            if (string.IsNullOrEmpty(id))
            {
                return Array.Empty<RemoteImageInfo>();
            }

            Plugin.Entity.Video? video = await client.LoadVideo(id).ConfigureAwait(false);

            return new RemoteImageInfo[]
            {
                new RemoteImageInfo
                {
                    ProviderName = this.Name,
                    Type = ImageType.Primary,
                    Url = video?.BoxArt,
                },
            };
        }

        /// <inheritdoc />
        public async Task<HttpResponseMessage> GetImageResponse(string url, CancellationToken cancelToken)
        {
            HttpResponseMessage httpResponse = await client.GetClient().GetAsync(url, cancelToken).ConfigureAwait(false);
            await Utility.CropThumb(httpResponse).ConfigureAwait(false);
            return httpResponse;
        }

        /// <inheritdoc />
        public IEnumerable<ImageType> GetSupportedImages(BaseItem item)
        {
            return new[] { ImageType.Primary };
        }

        /// <inheritdoc />
        public bool Supports(BaseItem item) => item is Movie;
    }
}
