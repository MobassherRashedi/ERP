using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;

namespace DevSkill.Inventory.Infrastructure.Identity
{
    public enum RolePermissions
    {
        None = 0,
        CanView = 1,
        CanEdit = 2,
        CanDelete = 4
        
    }

    public class ApplicationRole : IdentityRole<Guid>
    {
        public RolePermissions Permissions { get; set; }
        public ICollection<RolePermission> RolePermissions { get; set; }



       
    }

  
}
