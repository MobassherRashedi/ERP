using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevSkill.Inventory.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;


using Microsoft.AspNetCore.Authorization;
using DevSkill.Inventory.Domain;

namespace DevSkill.Inventory.Infrastructure.Authorization
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public RolePermissions RequiredPermission { get; }

        public PermissionRequirement(RolePermissions permission)
        {
            RequiredPermission = permission;
        }
    }
}
