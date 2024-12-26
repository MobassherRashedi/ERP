using Autofac.Extensions.DependencyInjection;
using Autofac;
using DevSkill.Inventory.Domain;
using DevSkill.Inventory.Infrastructure.Identity;
using DevSkill.Inventory.Infrastructure;
using DevSkill.Inventory.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Reflection;
using Serilog.Events;
using DevSkill.Inventory.Infrastructure.Seeders;
using DevSkill.Inventory.Infrastructure.Extensions;
using DevSkill.Inventory.Infrastructure.Authorization; 
using Microsoft.AspNetCore.Authorization;


public class Program
{
    public static async Task Main(string[] args)
    {
        #region Bootstrap Logger Configuration
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration, "BootstrapLogger")
            .CreateBootstrapLogger();
        #endregion

        try
        {
            Log.Information("Application Started..............");

            var builder = WebApplication.CreateBuilder(args);

            #region Serilog General Configuration
            builder.Host.UseSerilog((ctx, lc) => lc
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .ReadFrom.Configuration(ctx.Configuration));
            #endregion

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            var migrationAssembly = Assembly.GetExecutingAssembly().FullName;

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString, (x) => x.MigrationsAssembly(migrationAssembly)));

            builder.Services.AddDbContext<InventoryDbContext>(options =>
                options.UseSqlServer(connectionString, (x) => x.MigrationsAssembly(migrationAssembly)));

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddIdentity();


            // Configure SMTP Settings
            builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
            builder.Services.AddKeyedScoped<IEmailSender, EmailSender>("home");

            // CORS Configuration
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder => builder.AllowAnyOrigin()
                                      .AllowAnyMethod()
                                      .AllowAnyHeader());
            });

            builder.WebHost.UseUrls("http://*:80");

            #region Autofac Configuration
            builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
            builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
            {
                //containerBuilder.Populate(builder.Services);  // Let Autofac access IServiceCollection registrations
                containerBuilder.RegisterModule(new WebModule(connectionString, migrationAssembly));
            });

            #endregion

            // Add authorization policies
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("CanView", policy =>
                    policy.RequireClaim("Permission", "CanView"));
                options.AddPolicy("CanEdit", policy =>
                    policy.RequireClaim("Permission", "CanEdit"));
                options.AddPolicy("CanDelete", policy =>
                    policy.RequireClaim("Permission", "CanDelete"));
            });
/*
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("CanView", policy =>
                    policy.Requirements.Add(new PermissionRequirement(RolePermissions.CanView)));

                options.AddPolicy("CanEdit", policy =>
                    policy.Requirements.Add(new PermissionRequirement(RolePermissions.CanEdit)));

                options.AddPolicy("CanDelete", policy =>
                    policy.Requirements.Add(new PermissionRequirement(RolePermissions.CanDelete)));
            });*/

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = "/Account/AccessDenied"; // Correctly point to the Account controller
            });



            builder.Services.AddControllersWithViews();
            //builder.Services.AddRazorPages();
            builder.Services.AddScoped<UserDatabaseSeeder>();

            var app = builder.Build();
            // Seed the database
            await SeedDatabaseAsync(app);


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseCors("AllowAllOrigins"); // Apply CORS policy
            app.UseAuthentication(); // Ensure authentication middleware is used
            app.UseAuthorization();

            app.MapControllerRoute(
                   name: "areas",
                   pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
               );

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // Uncomment if using Razor Pages
            // app.MapRazorPages();

            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Failed to start application.");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
    private static async Task SeedDatabaseAsync(WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            try
            {
                // Seed the user-related data
                var userSeeder = services.GetRequiredService<UserDatabaseSeeder>();
                await userSeeder.SeedAsync(); // Seed user database

                // Seed the inventory-related data
                var inventorySeeder = services.GetRequiredService<InventoryDatabaseSeeder>();
                await inventorySeeder.SeedAsync(); // Seed inventory database
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred seeding the DB.");
            }
        }
    }
}
