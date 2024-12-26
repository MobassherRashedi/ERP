using DevSkill.Inventory.Infrastructure.Identity;

namespace DevSkill.Inventory.Infrastructure.Identity
{
    public class RolePermission
    {
        public Guid RoleId { get; set; }
        public ApplicationRole Role { get; set; }

        public RolePermissions Permission { get; set; }
    }
}
