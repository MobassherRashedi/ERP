using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Inventory.Infrastructure.Identity
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public UserService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task AddPermissionsToUser(ApplicationUser user, string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);

            if (role != null)
            {
                var permissions = Enum.GetValues<RolePermissions>().Where(p => role.Permissions.HasFlag(p));

                foreach (var permission in permissions)
                {
                    await _userManager.AddClaimAsync(user, new Claim("Permission", permission.ToString()));
                }
            }
        }

/*        public async Task CreateUserWithRoleAndPermissions(ApplicationUser user, string roleName, string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, roleName);
                await AddPermissionsToUser(user, roleName);  // Add permissions based on the role
            }
        }*/
        public async Task<IdentityResult> CreateUserWithRoleAndPermissions(ApplicationUser user, IEnumerable<string> roleNames, string password)
        {
            // Create user
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                // Assign roles
                await _userManager.AddToRolesAsync(user, roleNames);

                // Add permissions to the user based on roles
                foreach (var roleName in roleNames)
                {
                    await AddPermissionsToUser(user, roleName);
                }
            }
            return result;
        }


    }

}
