using Microsoft.EntityFrameworkCore;
using LinkSnap.Domain.Entities;
using LinkSnap.Application.Interfaces;
using LinkSnap.Infrastructure.Persistence;

namespace LinkSnap.Infrastructure.Repositories
{
    public class ClickRepository : IClickRepository
    {
        private readonly ApplicationDbContext _context;

        public ClickRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Click> AddAsync(Click click)
        {
            _context.Clicks.Add(click);
            await _context.SaveChangesAsync();
            return click;
        }

        public async Task<IEnumerable<Click>> GetByLinkIdAsync(Guid linkId)
            => await _context.Clicks
                .Where(c => c.LinkId == linkId)
                .OrderByDescending(c => c.ClickedAt)
                .ToListAsync();

        public async Task<int> GetClickCountByLinkIdAsync(Guid linkId)
            => await _context.Clicks
                .CountAsync(c => c.LinkId == linkId);
    }
}