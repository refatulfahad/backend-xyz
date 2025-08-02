using System.ComponentModel.DataAnnotations;

namespace ProductManagement.Models
{
    public class Permission
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public IList<PermissionRole> PermissionRoles { get; set; } = new List<PermissionRole>();
    }
}
