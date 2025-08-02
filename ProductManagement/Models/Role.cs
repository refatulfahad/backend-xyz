using System.ComponentModel.DataAnnotations;

namespace ProductManagement.Models
{
    public class Role
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public ICollection<PermissionRole> PermissionRoles { get; set; } = new List<PermissionRole>();
    }
}
