using LinkSnap.Domain.Entities;

namespace LinkSnap.Application.Interfaces
{
    public interface ILinkRepository
    {
        Task<Link?> GetByIdAsync(Guid id);
        Task<Link?> GetByShortCodeAsync(string shortCode);
        Task<List<Link>> GetUserLinksAsync(string userId);
        Task<Link> AddAsync(Link link);
        Task UpdateAsync(Link link);
        Task DeleteAsync(Link link);
        Task<bool> ExistsByShortCodeAsync(string shortCode);
    }
}