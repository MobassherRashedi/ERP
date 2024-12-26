using DevSkill.Inventory.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DevSkill.Inventory.Infrastructure.Authorization
{
    public class RolePermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public RolePermissionHandler(RoleManager<ApplicationRole> roleManager, ApplicationDbContext context)
        {
            _roleManager = roleManager;
            _context = context;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            var userRoles = context.User.FindAll(ClaimTypes.Role).Select(r => r.Value);

            foreach (var roleName in userRoles)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role != null)
                {
                    var rolePermissions = await _context.RolePermissions
                        .Where(rp => rp.RoleId == role.Id)
                        .ToListAsync();

                    foreach (var rolePermission in rolePermissions)
                    {
                        if (rolePermission.Permission.HasFlag(requirement.RequiredPermission))
                        {
                            context.Succeed(requirement);
                            return;
                        }
                    }
                }
            }

            context.Fail();
        }
    }
}
