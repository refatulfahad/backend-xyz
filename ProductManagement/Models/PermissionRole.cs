namespace ProductManagement.Models
{
    public class PermissionRole
    {
        public int PermissionId { get; set; }
        public Permission Permission { get; set; } = new Permission();

        public int RoleId { get; set; }
        public Role Role { get; set; } = new Role();
    }
}
