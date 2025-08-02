using Microsoft.EntityFrameworkCore;
using ProductManagement.Enum;
using ProductManagement.Models;

namespace ProductManagement.Data
{
    public static class DatabaseSeeder
    {
        public static async void SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var _context = scope.ServiceProvider.GetRequiredService<ProductContext>();

            // Ensure the database is migrated
            await _context.Database.MigrateAsync();

            // 1. Seed Permissions
            var permissions = System.Enum.GetValues(typeof(PermissionsEnum))
                                  .Cast<PermissionsEnum>()
                                  .Select(e => new Permission { Name = e.ToString() })
                                  .ToList();
            // Get permissions from Enum File

            var permissionToAdd = new List<Permission>();

            foreach (var permission in permissions)
            {
                if (!_context.Permissions.Any(p => p.Name == permission.Name))
                {
                    permissionToAdd.Add(new Permission
                    {
                        Name = permission.Name,
                    });
                }
            }

            await _context.AddRangeAsync(permissionToAdd);
            await _context.SaveChangesAsync();
        }
    }
}
