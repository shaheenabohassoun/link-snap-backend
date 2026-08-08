using LinkSnap.Application.Interfaces;
using LinkSnap.Application.DTOs;

namespace LinkSnap.Application.Services
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly ILinkRepository _linkRepository;
        private readonly IClickRepository _clickRepository;

        public AnalyticsService(ILinkRepository linkRepository, IClickRepository clickRepository)
        {
            _linkRepository = linkRepository;
            _clickRepository = clickRepository;
        }

        public async Task<LinkAnalyticsDto> GetLinkAnalyticsAsync(Guid linkId)
        {
            var link = await _linkRepository.GetByIdAsync(linkId);
            if (link == null)
                throw new KeyNotFoundException("Link not found");

            var clicks = await _clickRepository.GetByLinkIdAsync(linkId);

            return new LinkAnalyticsDto
            {
                LinkId = linkId,
                TotalClicks = link.ClickCount,
                DailyClicks = clicks
                    .Where(c => c.ClickedAt.HasValue)
                    .GroupBy(c => c.ClickedAt!.Value.Date)
                    .OrderBy(g => g.Key)
                    .Select(g => new AnalyticsPointDto
                    {
                        Label = g.Key.ToString("yyyy-MM-dd"),
                        Value = g.Count()
                    })
                    .ToList(),
                Referrers = clicks
                    .Where(c => !string.IsNullOrEmpty(c.Referrer))
                    .GroupBy(c => c.Referrer!)
                    .Select(g => new AnalyticsPointDto { Label = g.Key, Value = g.Count() })
                    .OrderByDescending(x => x.Value)
                    .ToList(),
                DeviceBreakdown = clicks
                    .GroupBy(c => GetDeviceCategory(c.UserAgent))
                    .Select(g => new AnalyticsPointDto { Label = g.Key, Value = g.Count() })
                    .OrderByDescending(x => x.Value)
                    .ToList()
            };
        }

        private static string GetDeviceCategory(string? userAgent)
        {
            if (string.IsNullOrEmpty(userAgent)) return "Unknown";
            if (userAgent.Contains("Mobile", StringComparison.OrdinalIgnoreCase)) return "Mobile";
            if (userAgent.Contains("Tablet", StringComparison.OrdinalIgnoreCase)) return "Tablet";
            return "Desktop";
        }
    }
}
