namespace UrlShortener.Models
{
    public class ShortenedUrl
    {
        public string Id { get; set; }
        public string Url { get; set; }

        public ShortenedUrl(string id, string url)
        {
            (Id, Url) = (id, url);
        }
    }
}
