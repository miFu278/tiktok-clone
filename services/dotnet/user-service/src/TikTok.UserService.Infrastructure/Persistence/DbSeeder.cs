using Microsoft.EntityFrameworkCore;
using TikTok.UserService.Domain.Entities;

namespace TikTok.UserService.Infrastructure.Persistence
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            // Seed Roles
            await SeedRolesAsync(context);
        }

        private static async Task SeedRolesAsync(ApplicationDbContext context)
        {
            if (await context.Roles.AnyAsync())
            {
                return; // Roles already seeded
            }

            var roles = new List<Role>
            {
                new Role
                {
                    Name = "User",
                    Description = "Default user role",
                    IsActive = true
                },
                new Role
                {
                    Name = "Creator",
                    Description = "Content creator with verified badge",
                    IsActive = true
                },
                new Role
                {
                    Name = "Moderator",
                    Description = "Content moderator",
                    IsActive = true
                },
                new Role
                {
                    Name = "Admin",
                    Description = "System administrator",
                    IsActive = true
                }
            };

            await context.Roles.AddRangeAsync(roles);
            await context.SaveChangesAsync();
        }
    }
}
