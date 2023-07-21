using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JellyJav.Plugin.Entity
{
    public class Video: EntityBase
    {
        public string Code { get; set; }
        public string Title { get; set; }
        public string? Studio { get; set; }
        public string? BoxArt { get; set; }
        public string? Cover { get; set; }
        public IEnumerable<string> Actresses { get; set; }
        public IEnumerable<string> Genres { get; set; }
        public DateTime ReleaseDate { get; set; }

        public Video(string id, string code, string title, IEnumerable<string> actresses, IEnumerable<string> genres, string? studio, string? boxArt, string? cover, DateTime releaseDate): base(id)
        {
            Code = code ?? throw new ArgumentNullException(nameof(code));
            Title = title ?? throw new ArgumentNullException(nameof(title));
            Actresses = actresses ?? throw new ArgumentNullException(nameof(actresses));
            Genres = genres ?? throw new ArgumentNullException(nameof(genres));
            Studio = studio;
            BoxArt = boxArt;
            Cover = cover;
            ReleaseDate = releaseDate;
        }
    }
}
