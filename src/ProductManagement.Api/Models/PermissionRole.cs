namespace ProductManagement.Models
{
    public class PermissionRole
    {
        public int PermissionId { get; set; }
        public Permission Permission { get; set; } = null!;

        public int RoleId { get; set; }
        public Role Role { get; set; } = null!;
    }
}
