namespace JellyJav
{
    using System;
    using System.Collections.Generic;
    using MediaBrowser.Common.Configuration;
    using MediaBrowser.Common.Plugins;
    using MediaBrowser.Model.Plugins;
    using MediaBrowser.Model.Serialization;

    /// <summary>JellyJav Plugin.</summary>
    public class Plugin : BasePlugin<PluginConfiguration>, IHasWebPages
    {
        /// <summary>Initializes a new instance of the <see cref="Plugin"/> class.</summary>
        /// <param name="applicationPaths">Instance of the <see cref="IApplicationPaths" />.</param>
        /// <param name="xmlSerializer">Instance of the <see cref="IXmlSerializer" />.</param>
        public Plugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer)
            : base(applicationPaths, xmlSerializer)
        {
            Instance = this;
        }

        /// <summary>Gets the current plugin's instance.</summary>
        public static Plugin? Instance { get; private set; }

        /// <inheritdoc />
        public override string Name => "Jelly JAV";

        /// <inheritdoc />
        public override Guid Id => Guid.Parse("5a771ee2-cec0-4313-b02b-733453b1ba5b");

        /// <inheritdoc />
        public IEnumerable<PluginPageInfo> GetPages()
        {
            return new[]
            {
                new PluginPageInfo
                {
                    Name = this.Name,
                    EmbeddedResourcePath = string.Format("{0}.config_page.html", this.GetType().Namespace),
                },
            };
        }
    }
}
