namespace JellyJav.Plugin.SearchResult
{
    public class VideoResult : SearchResultBase
    {
        public string? Code { get; set; }

        public VideoResult(string code, string id)
        {
            Code = code;
            Id = id;
        }
    }
}
