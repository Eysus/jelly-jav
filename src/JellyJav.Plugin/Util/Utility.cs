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
        /// <remarks>This method should evolve in order to make difference between 4 digit code, and prefix 0</remarks>
        public static string? ExtractCodeFromFilename(string filename)
        {
            const string universalRegex = @"([a-zA-Z]+(\d+)?)(-)?(\d{3})";
            filename = Path.GetFileNameWithoutExtension(filename);
            Regex rx = new Regex(universalRegex);

            if (!rx.IsMatch(filename)) return null;

            const string validPattern = @"([A-Z]+(\d+)?)-(\d{3})";
            GroupCollection parts = rx.Match(filename).Groups;
            string serie = parts[1].Value.Replace("00", "").ToUpper();
            string number = parts[4].Value;

            if (number.Length > 3)
            {
                number = number.Substring(number.Length - 3);
            }

            string value = string.Format($"{serie}-{number}");

            return Regex.IsMatch(value, validPattern) ? value : null;
        }

        /// <summary>Extracts the jav code from a video's filename.</summary>
        /// <param name="filename">The video's filename.</param>
        /// <returns>The video's jav code.</returns>
        public static string? ExtractFC2CodeFromFilename(string filename)
        {
            const string universalRegex = @"(FC2)([\s-])?(PPV)([\s-])?(\d+)";
            filename = Path.GetFileNameWithoutExtension(filename);
            Regex rx = new Regex($"{universalRegex}", RegexOptions.IgnoreCase);

            if (!rx.IsMatch(filename)) return filename;

            const string validPattern = "FC2-PPV-\\d+";
            GroupCollection parts = rx.Match(filename).Groups;
            string code = "FC2-PPV-" + parts[5].Value;

            return Regex.IsMatch(code, validPattern) ? code : null;
        }

        /// <summary>Creates a video's display name according to the plugin's selected configuration.</summary>
        /// <param name="video">The video.</param>
        /// <returns>The video's created display name.</returns>
        public static string CreateVideoDisplayName(Entity.Video video)
        {
            return Plugin.Instance?.Configuration.VideoDisplayName switch
            {
                VideoDisplayName.BOTH => video.Code + " - " + video.Title,
                VideoDisplayName.TITLE => video.Title,
                VideoDisplayName.CODE => video.Title,
                _ => throw new Exception("Impossible to reach.")
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
