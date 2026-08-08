namespace LinkSnap.Application.DTOs
{
    public class ShortenResponse
    {
        public string ShortCode { get; set; }
        public string ShortUrl { get; set; }
        public string OriginalUrl { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}