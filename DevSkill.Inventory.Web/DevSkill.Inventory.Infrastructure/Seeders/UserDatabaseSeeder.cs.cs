using DevSkill.Inventory.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DevSkill.Inventory.Infrastructure.Seeders
{
    public class UserDatabaseSeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public UserDatabaseSeeder(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task SeedAsync()
        {
            await SeedRolesAsync();
            await SeedUsersAsync();
        }

        private async Task SeedRolesAsync()
        {
            // Define the roles and associated permissions
            var roles = new Dictionary<string, RolePermissions>
            {
                { "Admin", RolePermissions.CanView | RolePermissions.CanEdit | RolePermissions.CanDelete },
                { "Manager", RolePermissions.CanView | RolePermissions.CanEdit },
                { "Member", RolePermissions.CanView },
                { "Support", RolePermissions.CanView }
            };

            foreach (var role in roles)
            {
                // Check if the role exists, if not, create it
                var roleExists = await _roleManager.RoleExistsAsync(role.Key);
                if (!roleExists)
                {
                    var newRole = new ApplicationRole
                    {
                        Name = role.Key,
                        Permissions = role.Value
                    };

                    var createRoleResult = await _roleManager.CreateAsync(newRole);
                    if (!createRoleResult.Succeeded)
                    {
                        throw new Exception($"Failed to create role '{role.Key}': {string.Join(", ", createRoleResult.Errors.Select(e => e.Description))}");
                    }

                    // Assign permissions to the role
                    foreach (var permission in Enum.GetValues<RolePermissions>().Cast<RolePermissions>())
                    {
                        if (role.Value.HasFlag(permission))
                        {
                            var rolePermission = new RolePermission
                            {
                                RoleId = newRole.Id,
                                Permission = permission
                            };

                            await _context.RolePermissions.AddAsync(rolePermission);
                        }
                    }

                    await _context.SaveChangesAsync();
                }
            }
        }

        private async Task SeedUsersAsync()
        {
            const string defaultPassword = "Code123#";
            var roles = new[] { "Admin", "Member", "Manager", "Support" };

            foreach (var roleName in roles)
            {
                var userEmail = $"{roleName.ToLower()}@example.com";
                var user = await _userManager.FindByEmailAsync(userEmail);

                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        UserName = userEmail,
                        Email = userEmail,
                        FirstName = roleName,
                        LastName = "User",
                        IsActive = true,
                        EmailConfirmed = true
                    };

                    var createUserResult = await _userManager.CreateAsync(user, defaultPassword);
                    if (createUserResult.Succeeded)
                    {
                        // Add role to user
                        await _userManager.AddToRoleAsync(user, roleName);

                        // Add permission claims
                        var role = await _roleManager.FindByNameAsync(roleName);
                        var rolePermissions = await _context.RolePermissions
                            .Where(rp => rp.RoleId == role.Id)
                            .ToListAsync();

                        foreach (var rolePermission in rolePermissions)
                        {
                            var permissionClaim = new Claim("Permission", rolePermission.Permission.ToString());
                            await _userManager.AddClaimAsync(user, permissionClaim);
                        }
                    }
                    else
                    {
                        throw new Exception($"Failed to create user '{user.UserName}': {string.Join(", ", createUserResult.Errors.Select(e => e.Description))}");
                    }
                }
                else
                {
                    user.EmailConfirmed = true;
                    user.IsActive = true;
                    await _userManager.UpdateAsync(user);
                }
            }
        }
    }
}
