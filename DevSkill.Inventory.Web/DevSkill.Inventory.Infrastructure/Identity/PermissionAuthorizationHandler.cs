using DevSkill.Inventory.Infrastructure.Authorization;
using DevSkill.Inventory.Infrastructure.Identity;
using DevSkill.Inventory.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly ApplicationDbContext _context;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public PermissionAuthorizationHandler(ApplicationDbContext context, RoleManager<ApplicationRole> roleManager)
    {
        _context = context;
        _roleManager = roleManager;
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
                    if (context.User.HasClaim(c => c.Type == "Permission" && c.Value == rolePermission.Permission.ToString()))
                    {
                        context.Succeed(requirement);
                        return;
                    }
                }
            }
        }

        context.Fail();  // Optional: to explicitly fail if no matching claim is found
    }
}
