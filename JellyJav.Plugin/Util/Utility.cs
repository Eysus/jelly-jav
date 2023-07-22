namespace JellyJav.Plugin.Util
{
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using JellyJav.Plugin.Enumeration;
    using MediaBrowser.Controller.Entities;
    using MediaBrowser.Controller.Library;
    using MediaBrowser.Controller.Providers;
    using SkiaSharp;

    /// <summary>A general utility class for random functions.</summary>
    public static class Utility
    {
        /// <summary>
        /// When setting the video title in a Provider, we lose the JAV code details in MovieInfo.
        /// So this is used to retrieve the JAV code to then be able to search using a different Provider.
        /// </summary>
        /// <param name="info">The video's info.</param>
        /// <param name="libraryManager">Instance of the <see cref="ILibraryManager" />.</param>
        /// <returns>The video's original title.</returns>
        public static string GetVideoOriginalTitle(MovieInfo info, ILibraryManager libraryManager)
        {
            InternalItemsQuery searchQuery = new InternalItemsQuery
            {
                Name = info.Name,
            };
            BaseItem? result = libraryManager.GetItemList(searchQuery).FirstOrDefault();

            if (result is null)
            {
                return info.Name;
            }

            return result.OriginalTitle ?? result.Name;
        }

        /// <summary>Extracts the jav code from a video's filename.</summary>
        /// <param name="filename">The video's filename.</param>
        /// <returns>The video's jav code.</returns>
        public static string? ExtractCodeFromFilename(string filename)
        {
            Regex rx = new Regex(@"[\w\d]+-?\d+");
            string? value = rx.Match(filename)?.Value.ToUpper();

            if (value is null)
            {
                return null;
            }

            if (value.Contains("-"))
            {
                return value;
            }
            else
            {
                rx = new Regex(@"([\w\d]+?)(\d+)");
                GroupCollection groups = rx.Match(value).Groups;
                return groups[1] + "-" + groups[2];
            }
        }

        /// <summary>Creates a video's display name according to the plugin's selected configuration.</summary>
        /// <param name="video">The video.</param>
        /// <returns>The video's created display name.</returns>
        public static string CreateVideoDisplayName(Entity.Video video)
        {
            return JellyJav.Plugin.Plugin.Instance?.Configuration.VideoDisplayName switch
            {
                VideoDisplayName.BOTH => video.Code + " " + video.Title,
                VideoDisplayName.TITLE => video.Title,
                VideoDisplayName.CODE => video.Title,
                _ => throw new System.Exception("Impossible to reach.")
            };
        }

        /// <summary>Crops a full size dvd cover into just the front cover image.</summary>
        /// <param name="httpResponse">The full size dvd cover's http response.</param>
        /// <returns>An empty task when the job is done.</returns>
        public static async Task CropThumb(HttpResponseMessage httpResponse)
        {
            using Stream imageStream = await httpResponse.Content.ReadAsStreamAsync().ConfigureAwait(false);
            using SKBitmap imageBitmap = SKBitmap.Decode(imageStream);

            SKBitmap subset = new SKBitmap();
            imageBitmap.ExtractSubset(subset, SKRectI.Create(421, 0, 379, 538));

            // I think there will be a memory leak if I use MemoryStore.
            FileStream finalStream = File.Open(Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".jpg"), FileMode.OpenOrCreate);
            subset.Encode(finalStream, SKEncodedImageFormat.Jpeg, 100);
            finalStream.Seek(0, 0);

            StreamContent newContent = new StreamContent(finalStream);
            newContent.Headers.ContentType = httpResponse.Content.Headers.ContentType;
            httpResponse.Content = newContent;
        }
    }
}
