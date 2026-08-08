using LinkSnap.Application.DTOs;

namespace LinkSnap.Application.Interfaces
{
    public interface IAnalyticsService
    {
        Task<LinkAnalyticsDto> GetLinkAnalyticsAsync(Guid linkId);
    }
}