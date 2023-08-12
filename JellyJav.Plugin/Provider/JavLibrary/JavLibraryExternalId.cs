using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;

namespace JellyJav.Providers.JavlibraryProvider
{
    /// <summary>External ID for a Javlibrary video.</summary>
    public class JavLibraryExternalId : IExternalId
    {
        /// <inheritdoc />
        public string ProviderName => "JavLibrary";

        /// <inheritdoc />
        public string Key => "Javlibrary";

        /// <inheritdoc />
        public string UrlFormatString => "https://www.javlibrary.com/en/?v={0}";

        /// <inheritdoc />
        public ExternalIdMediaType? Type => ExternalIdMediaType.Movie;

        /// <inheritdoc />
        public bool Supports(IHasProviderIds item)
        {
            return item is Movie;
        }
    }
}
