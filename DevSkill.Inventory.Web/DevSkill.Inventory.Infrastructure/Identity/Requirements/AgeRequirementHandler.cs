using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Inventory.Infrastructure.Identity.Requirements
{
    public class AgeRequirementHandler : AuthorizationHandler<AgeRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, 
            AgeRequirement requirement)
        {
            if(context.User.HasClaim(x => x.Type == "age" && 
                int.Parse(x.Value) > 10 && int.Parse(x.Value) < 50))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
