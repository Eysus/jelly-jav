using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;

namespace JellyJav.Plugin.Providers.AsianscreensProvider
{
    /// <summary>External ID for an Asianscreens actress.</summary>
    public class AsianScreensExternalId : IExternalId
    {
        /// <inheritdoc />
        public string ProviderName => "Asianscreens";

        /// <inheritdoc />
        public string Key => "Asianscreens";

        /// <inheritdoc />
        public string UrlFormatString => "https://www.asianscreens.com/{0}.asp";

        /// <inheritdoc />
        public ExternalIdMediaType? Type => ExternalIdMediaType.Person;

        /// <inheritdoc />
        public bool Supports(IHasProviderIds item)
        {
            return item is Person;
        }
    }
}
