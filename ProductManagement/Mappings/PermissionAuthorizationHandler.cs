using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ProductManagement.Data;
using System.Security.Claims;
using Enyim.Caching;
public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IMemcachedClient memcachedClient;
    public PermissionAuthorizationHandler(IServiceScopeFactory scopeFactory, IMemcachedClient memcachedClient)
    {
        _scopeFactory = scopeFactory;
        this.memcachedClient = memcachedClient;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ProductContext>();

        var userRoles = context.User.Claims
                    .Where(c => c.Type == ClaimTypes.Role || c.Type == "roles" || c.Type == "role")
                    .SelectMany(c => c.Value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        .ToList();
        
        var userRolesLower = userRoles
                    .Select(r => r.ToLower())
                    .ToList();

        if (!userRolesLower.Any())
            return;

        var cacheKey = $"role_permissions_{string.Join("_", userRolesLower)}";
        var cachedPermissions = await memcachedClient.GetValueAsync<List<string>>(cacheKey);
        List<string> rolePermissions;

        if (cachedPermissions != null)
        {
            rolePermissions = cachedPermissions;
        }
        else
        {
            rolePermissions = await dbContext.PermissionRoles
                 .Where(rp => userRolesLower.Contains(rp.Role.Name.ToLower())) 
                 .Select(rp => rp.Permission.Name)
                 .Distinct()
                 .ToListAsync();
            await memcachedClient.SetAsync(cacheKey, rolePermissions, TimeSpan.FromMinutes(10));
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