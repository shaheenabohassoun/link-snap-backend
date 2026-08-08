using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Entities/Click.cs
using LinkSnap.Domain.Common;

namespace LinkSnap.Domain.Entities
{
    public class Click : BaseEntity
    {
        public Guid LinkId { get; set; }
        public string? IPAddress { get; set; }
        public string? UserAgent { get; set; }
        public string? Referrer { get; set; }
        public string? Country { get; set; }
        public DateTime? ClickedAt { get; set; }

        // Navigation property
        public Link Link { get; set; } = null!;
        
    }
}
