using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;

namespace JellyJav.Providers.JAVHDPorn
{
    /// <summary>External ID for a Javlibrary video.</summary>
    public class JAVHDPornExternalId : IExternalId
    {
        /// <inheritdoc />
        public string ProviderName => "JAVHDPorn";

        /// <inheritdoc />
        public string Key => "JAVHDPorn";

        /// <inheritdoc />
        public string UrlFormatString => "https://www4.javhdporn.net/video/{0}";

        /// <inheritdoc />
        public ExternalIdMediaType? Type => ExternalIdMediaType.Movie;

        /// <inheritdoc />
        public bool Supports(IHasProviderIds item)
        {
            return item is Movie;
        }
    }
}
