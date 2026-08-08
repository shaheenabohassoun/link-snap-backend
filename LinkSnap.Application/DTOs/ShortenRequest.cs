namespace LinkSnap.Application.DTOs
{
    public class ShortenRequest
    {
        public string OriginalUrl { get; set; }
        public string? CustomAlias { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }
}