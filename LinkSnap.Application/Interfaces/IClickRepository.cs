using LinkSnap.Domain.Entities;

namespace LinkSnap.Application.Interfaces
{
    public interface IClickRepository
    {
        Task<Click> AddAsync(Click click);
        Task<IEnumerable<Click>> GetByLinkIdAsync(Guid linkId);
        Task<int> GetClickCountByLinkIdAsync(Guid linkId);
    }
}