namespace LinkSnap.Application.DTOs
{
    public class LinkAnalyticsDto
    {
        public Guid LinkId { get; set; }
        public int TotalClicks { get; set; }
        public List<AnalyticsPointDto> DailyClicks { get; set; } = new();
        public List<AnalyticsPointDto> DeviceBreakdown { get; set; } = new();
        public List<AnalyticsPointDto> Referrers { get; set; } = new();
    }
}
