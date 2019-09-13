using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;

namespace JellyfinJav.Providers.Asianscreens
{
    public class AsianscreensPersonImageProvider : IRemoteImageProvider
    {
        private readonly IHttpClient httpClient;

        public string Name => "Asianscreens";

        public AsianscreensPersonImageProvider(IHttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<IEnumerable<RemoteImageInfo>> GetImages(BaseItem item, CancellationToken cancelToken)
        {
            var result = new List<RemoteImageInfo>();

            var client = new AsianscreensApi();

            var id = item.GetProviderId("Asianscreens");
            if (string.IsNullOrEmpty(id))
            {
                await client.findActress(item.Name);
            }
            else
            {
                await client.loadActress(id);
            }

            result.Add(new RemoteImageInfo
            {
                ProviderName = Name,
                Type = ImageType.Primary,
                Url = client.getCover()
            });

            return result;
        }

        public Task<HttpResponseInfo> GetImageResponse(string url, CancellationToken cancelToken)
        {
            return httpClient.GetResponse(new HttpRequestOptions
            {
                Url = url,
                CancellationToken = cancelToken
            });
        }

        public IEnumerable<ImageType> GetSupportedImages(BaseItem item)
        {
            return new[] { ImageType.Primary };
        }

        public bool Supports(BaseItem item)
        {
            return item is Person;
        }
    }
}