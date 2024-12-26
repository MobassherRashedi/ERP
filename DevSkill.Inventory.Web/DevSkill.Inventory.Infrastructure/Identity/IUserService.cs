using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevSkill.Inventory.Infrastructure.Identity
{
    public interface IUserService
    {
        Task AddPermissionsToUser(ApplicationUser user, string roleName);

        Task<IdentityResult> CreateUserWithRoleAndPermissions(ApplicationUser user, IEnumerable<string> roleNames, string password);
    }
}
