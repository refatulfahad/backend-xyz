using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace ProductManagement.Mappings
{
    public class CustomAuthorizationAttribute : Attribute, IAuthorizationFilter
    {
        public string Roles { get; set; }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (!user.Identity?.IsAuthenticated ?? false)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            if (!string.IsNullOrEmpty(Roles))
            {
                var allowedRoles = Roles.Split(',');

                var hasRole = allowedRoles.Any(role =>
                    user.Claims.Any(c =>
                    {
                        if (string.IsNullOrWhiteSpace(c.Value))
                            return false;

                        var userRoles = c.Value.Split(',');
                        return (c.Type == ClaimTypes.Role || c.Type == "roles" || c.Type == "role") &&
                               userRoles.Any(x => string.Equals(x.Trim(), role.Trim(), StringComparison.OrdinalIgnoreCase));
                    })
                );


                if (!hasRole)
                {
                    context.Result = new ForbidResult();
                    return;
                }
            }
        }
    }
}
