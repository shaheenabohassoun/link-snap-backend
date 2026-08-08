using Microsoft.EntityFrameworkCore;
using LinkSnap.Domain.Entities;
using LinkSnap.Application.Interfaces;
using LinkSnap.Infrastructure.Persistence;

namespace LinkSnap.Infrastructure.Repositories
{
    public class LinkRepository : ILinkRepository
    {
        private readonly ApplicationDbContext _context;

        public LinkRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Link?> GetByIdAsync(Guid id)
            => await _context.Links.FindAsync(id);

        public async Task<Link?> GetByShortCodeAsync(string shortCode)
            => await _context.Links
                .FirstOrDefaultAsync(l => l.ShortCode == shortCode);

        public async Task<List<Link>> GetUserLinksAsync(string userId)
            => await _context.Links
                .AsNoTracking()
                .Where(l => l.UserId == userId)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();

        public async Task<Link> AddAsync(Link link)
        {
            _context.Links.Add(link);
            await _context.SaveChangesAsync();
            return link;
        }

        public async Task UpdateAsync(Link link)
        {
            _context.Links.Update(link);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Link link)
        {
            _context.Links.Remove(link);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsByShortCodeAsync(string shortCode)
            => await _context.Links.AnyAsync(l => l.ShortCode == shortCode);
    }
}