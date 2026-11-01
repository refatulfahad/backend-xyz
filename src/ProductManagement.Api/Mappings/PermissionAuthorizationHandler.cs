using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ProductManagement.Data;
using ProductManagement.Services;
using System.Security.Claims;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IRolePermissionCache _rolePermissionCache;

    public PermissionAuthorizationHandler(IServiceScopeFactory scopeFactory, IRolePermissionCache rolePermissionCache)
    {
        _scopeFactory = scopeFactory;
        _rolePermissionCache = rolePermissionCache;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        var userRoles = context.User.Claims
                    .Where(c => c.Type == ClaimTypes.Role || c.Type == "roles" || c.Type == "role")
                    .SelectMany(c => c.Value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        .ToList();

        var userRolesLower = userRoles
                    .Select(r => r.ToLower())
                    .Distinct()
                    .OrderBy(r => r)
                    .ToList();

        if (!userRolesLower.Any())
            return;

        var cacheKey = $"role_permissions_{string.Join("_", userRolesLower)}";
        List<string> rolePermissions;

        if (_rolePermissionCache.TryGetPermissions(cacheKey, out var cachedPermissions))
        {
            rolePermissions = [.. cachedPermissions];
        }
        else
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ProductContext>();

            rolePermissions = await dbContext.PermissionRoles
                .Where(rp => userRolesLower.Contains(rp.Role.Name.ToLower()))
                .Select(rp => rp.Permission.Name)
                .Distinct()
                .ToListAsync();

            _rolePermissionCache.SetPermissions(cacheKey, rolePermissions, TimeSpan.FromMinutes(10));
        }

        if (requirement.Permissions.Any(permission => rolePermissions.Contains(permission)))
        {
            context.Succeed(requirement);
        }
    }
}


public class PermissionRequirement(IEnumerable<string> permissions) : IAuthorizationRequirement
{
    public IEnumerable<string> Permissions { get; } = permissions;
}