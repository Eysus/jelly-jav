namespace JellyJav.Plugin.Entity
{
    public class Video : EntityBase
    {
        public string Code { get; set; }
        public string Title { get; set; }
        public string? Studio { get; set; }
        public string? BoxArt { get; set; }
        public string? Cover { get; set; }
        public IEnumerable<string> Actresses { get; set; }
        public IEnumerable<string> Genres { get; set; }
        public DateTime? ReleaseDate { get; set; }

        public Video(string id, string code, string title, IEnumerable<string> actresses, IEnumerable<string> genres, string? studio, string? boxArt, string? cover, DateTime? releaseDate) : base(id)
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

        public override bool Equals(object? obj)
        {
            return obj is Video video &&
                   Code == video.Code &&
                   Title == video.Title &&
                   Studio == video.Studio &&
                   BoxArt == video.BoxArt &&
                   Cover == video.Cover &&
                   Actresses.SequenceEqual(video.Actresses) &&
                   Genres.SequenceEqual(video.Genres) &&
                   ReleaseDate == video.ReleaseDate;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Code, Title, Studio, BoxArt, Cover, Actresses, Genres, ReleaseDate);
        }
    }
}
