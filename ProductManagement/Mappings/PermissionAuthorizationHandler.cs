using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ProductManagement.Data;
using System.Security.Claims;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IServiceScopeFactory _scopeFactory;

    public PermissionAuthorizationHandler(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
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

        var rolePermissions = await dbContext.PermissionRoles
             .Where(rp => userRolesLower.Contains(rp.Role.Name.ToLower())) 
             .Select(rp => rp.Permission.Name)
             .Distinct()
             .ToListAsync();

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