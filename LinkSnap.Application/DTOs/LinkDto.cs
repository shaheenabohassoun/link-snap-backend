using LinkSnap.Domain.Enums;

namespace LinkSnap.Application.DTOs
{
    public class LinkDto
    {
        public Guid Id { get; set; }
        public string OriginalUrl { get; set; } = string.Empty;
        public string ShortCode { get; set; } = string.Empty;
        public string? CustomAlias { get; set; }
        public int ClickCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public bool IsActive { get; set; }
        public string? UserId { get; set; }
        public LinkStatus Status { get; set; } = LinkStatus.Active;
    }
}
