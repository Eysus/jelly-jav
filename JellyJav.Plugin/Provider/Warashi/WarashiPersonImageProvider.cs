using JellyJav.Plugin.Client;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;

namespace JellyJav.Providers.WarashiProvider
{
    /// <summary>The provider for Warashi actress headshots.</summary>
    public class WarashiPersonImageProvider : IRemoteImageProvider
    {
        private WarashiClient client;
        public string Name => "Warashi";

        public WarashiPersonImageProvider()
        {
            client = new();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<RemoteImageInfo>> GetImages(BaseItem item, CancellationToken cancelToken)
        {
            string? id = item.GetProviderId("Warashi");
            if (string.IsNullOrEmpty(id))
            {
                return Array.Empty<RemoteImageInfo>();
            }

            Plugin.Entity.Actress? actress = await client.LoadActress(id).ConfigureAwait(false);
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
