using JellyJav.Plugin.Client;
using JellyJav.Plugin.Entity;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;

namespace JellyJav.Providers.AsianscreensProvider
{
    /// <summary>The provider for Asianscreens actress headshots.</summary>
    public class AsianScreensPersonImageProvider : IRemoteImageProvider
    {
        private static readonly AsianScreensClient client = new();

        /// <inheritdoc />
        public string Name => "AsianScreens";

        /// <inheritdoc />
        public async Task<IEnumerable<RemoteImageInfo>> GetImages(BaseItem item, CancellationToken cancelToken)
        {
            string? id = item.GetProviderId("Asianscreens");
            if (string.IsNullOrEmpty(id))
            {
                return Array.Empty<RemoteImageInfo>();
            }

            Actress? actress = await client.LoadActress(id).ConfigureAwait(false);
            if (actress is null || actress.Cover == null)
            {
                return Array.Empty<RemoteImageInfo>();
            }

            return new RemoteImageInfo[]
            {
                new RemoteImageInfo
                {
                    ProviderName = this.Name,
                    Type = ImageType.Primary,
                    Url = actress.Cover,
                },
            };
        }

        /// <inheritdoc />
        public async Task<HttpResponseMessage> GetImageResponse(string url, CancellationToken cancelToken)
        {
            return await client.GetClient().GetAsync(url, cancelToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public IEnumerable<ImageType> GetSupportedImages(BaseItem item)
        {
            return new[] { ImageType.Primary };
        }

        /// <inheritdoc />
        public bool Supports(BaseItem item) => item is Person;
    }
}
