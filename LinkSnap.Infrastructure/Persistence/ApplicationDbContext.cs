using LinkSnap.Domain.Entities;
using LinkSnap.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace LinkSnap.Infrastructure.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Link> Links { get; set; }
        public DbSet<Click> Clicks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Link
            modelBuilder.Entity<Link>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.OriginalUrl)
                      .IsRequired()
                      .HasMaxLength(2048);
                entity.Property(e => e.ShortCode)
                      .IsRequired()
                      .HasMaxLength(10);
                entity.HasIndex(e => e.ShortCode)
                      .IsUnique();
                entity.Property(e => e.UserId)
                      .HasMaxLength(450); // IdentityUser Id length
                entity.HasMany(e => e.Clicks)
                      .WithOne(c => c.Link)
                      .HasForeignKey(c => c.LinkId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Click
            modelBuilder.Entity<Click>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.IPAddress).HasMaxLength(45);
                entity.Property(e => e.UserAgent).HasMaxLength(500);
                entity.Property(e => e.Referrer).HasMaxLength(2048);
                entity.Property(e => e.Country).HasMaxLength(100);
            });
        }
    }
}