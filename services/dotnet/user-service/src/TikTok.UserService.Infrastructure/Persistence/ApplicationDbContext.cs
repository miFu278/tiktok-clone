using Microsoft.EntityFrameworkCore;
using TikTok.UserService.Domain.Entities;

namespace TikTok.UserService.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<UserRole> UserRoles => Set<UserRole>();
        public DbSet<UserSettings> UserSettings => Set<UserSettings>();
        public DbSet<UserStats> UserStats => Set<UserStats>();
        public DbSet<Follow> Follows => Set<Follow>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateAuditableEntities();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateAuditableEntities()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is TikTok.Shared.Common.Abstractions.Entities.AuditableEntity &&
                           (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                var entity = (TikTok.Shared.Common.Abstractions.Entities.AuditableEntity)entry.Entity;

                if (entry.State == EntityState.Added)
                {
                    entity.SetCreated(null);
                }
                else if (entry.State == EntityState.Modified)
                {
                    entity.SetUpdated(null);
                }
            }
        }
    }
}
