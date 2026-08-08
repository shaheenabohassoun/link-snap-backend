using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Entities/Link.cs
using LinkSnap.Domain.Common;
using LinkSnap.Domain.Enums;

namespace LinkSnap.Domain.Entities
{
    public class Link : BaseEntity
    {
        public string OriginalUrl { get; set; }
        public string ShortCode { get; set; } // unique
        public string? UserId { get; set; }   // IdentityUser Id (string)
        public int ClickCount { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public bool IsActive { get; set; } = true;
        public LinkStatus Status { get; set; } = LinkStatus.Active;

        // Navigation property
        [NotMapped]
        public ICollection<Click> Clicks { get; set; } = new List<Click>();
    }
}
