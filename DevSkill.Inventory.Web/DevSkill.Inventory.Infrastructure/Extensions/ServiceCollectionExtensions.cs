using DevSkill.Inventory.Infrastructure.Identity;
using DevSkill.Inventory.Infrastructure.Identity.Requirements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DevSkill.Inventory.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        
        public static void AddIdentity(this IServiceCollection services)
        {
               services
                .AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddUserManager<ApplicationUserManager>()
                .AddRoleManager<ApplicationRoleManager>()
                .AddSignInManager<ApplicationSignInManager>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 0;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("CustomAccess", policy =>
                {
                    policy.RequireRole("Member");
                    policy.RequireRole("Support");
                });

                options.AddPolicy("CreatePermission", policy =>
                {
                    policy.RequireClaim("create", "true");
                });

                options.AddPolicy("AgeRestriction", policy =>
                {
                    policy.Requirements.Add(new AgeRequirement());
                });
            });

            services.AddSingleton<IAuthorizationHandler, AgeRequirementHandler>();
        }
    }
}
