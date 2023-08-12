using JellyJav.Plugin.Enumeration;
using MediaBrowser.Model.Plugins;

namespace JellyJav.Plugin.Configuration
{
    /// <summary>
    /// Plugin configuration.
    /// </summary>
    public class PluginConfiguration : BasePluginConfiguration
    {
        public ActressNameOrder ActressNameOrder { get; set; }
        public VideoDisplayName VideoDisplayName { get; set; }
        public bool EnableActresses { get; set; }

        public PluginConfiguration()
        {
            ActressNameOrder = ActressNameOrder.FIRST_LAST;
            VideoDisplayName = VideoDisplayName.TITLE;
            EnableActresses = false;
        }
    }
}
