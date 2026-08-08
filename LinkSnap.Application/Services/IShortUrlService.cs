using LinkSnap.Application.DTOs;

namespace LinkSnap.Application.Services
{
    public interface IShortUrlService
    {
        Task<LinkDto> CreateShortUrlAsync(ShortenRequest request, string? userId = null);
        Task<string> ResolveShortCodeAsync(
            string shortCode,
            string? ipAddress = null,
            string? userAgent = null,
            string? referrer = null);
        Task<LinkDto?> GetLinkByShortCodeAsync(string shortCode);
        Task<IEnumerable<LinkDto>> GetUserLinksAsync(string userId);
        Task<LinkDto?> GetLinkByIdAsync(Guid id);
        Task<LinkDto> UpdateLinkAsync(Guid id, UpdateLinkDto update);
        Task DeleteLinkAsync(Guid id);
    }
}
