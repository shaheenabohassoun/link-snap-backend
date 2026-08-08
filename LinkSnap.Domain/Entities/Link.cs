using LinkSnap.Domain.Common;
using LinkSnap.Domain.Enums;

namespace LinkSnap.Domain.Entities
{
    public class Link : BaseEntity
    {
        public string OriginalUrl { get; set; } = string.Empty;
        public string ShortCode { get; set; } = string.Empty;
        public string? UserId { get; set; }
        public int ClickCount { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public bool IsActive { get; set; } = true;
        public LinkStatus Status { get; set; } = LinkStatus.Active;

        // Navigation property
        public ICollection<Click> Clicks { get; set; } = new List<Click>();
    }
}
