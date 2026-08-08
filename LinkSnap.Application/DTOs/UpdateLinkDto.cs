namespace LinkSnap.Application.DTOs
{
    public class UpdateLinkDto
    {
        public DateTime? ExpiresAt { get; set; }
        public bool? IsActive { get; set; }
    }
}