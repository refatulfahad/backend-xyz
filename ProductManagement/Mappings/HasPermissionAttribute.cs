using Microsoft.AspNetCore.Authorization;
using ProductManagement.Enum;

namespace ProductManagement.Mappings
{
    public sealed class HasPermissionAttribute : AuthorizeAttribute
    {
        public HasPermissionAttribute(params PermissionsEnum[] permissions)
            : base(policy: string.Join(",", permissions.Select(p => p.ToString())))
        {

        }
    }
}
