using Autofac;
using AutoMapper;
using DevSkill.Inventory.Application.Services;
using DevSkill.Inventory.Application;
using DevSkill.Inventory.Domain;
using DevSkill.Inventory.Infrastructure.Repositories;
using DevSkill.Inventory.Infrastructure.UnitOfWorks;
using DevSkill.Inventory.Infrastructure;
using Microsoft.Extensions.Logging;
using DevSkill.Inventory.Web;
using Autofac.Core;
using DevSkill.Inventory.Domain.Interfaces;
using DevSkill.Inventory.Infrastructure.Identity;
using DevSkill.Inventory.Infrastructure.Seeders;
using Microsoft.AspNetCore.Identity;
using Autofac.Extensions.DependencyInjection;

public class WebModule : Module
{
    private readonly string _connectionString;
    private readonly string _migrationAssembly;

    public WebModule(string connectionString, string migrationAssembly)
    {
        _connectionString = connectionString;
        _migrationAssembly = migrationAssembly;
    }

    protected override void Load(ContainerBuilder builder)
    {
        // Register DbContext
        builder.RegisterType<InventoryDbContext>().AsSelf()
            .WithParameter("connectionString", _connectionString)
            .WithParameter("migrationAssembly", _migrationAssembly)
            .InstancePerLifetimeScope();

        builder.RegisterType<ApplicationDbContext>().AsSelf()
            .WithParameter("connectionString", _connectionString)
            .WithParameter("migrationAssembly", _migrationAssembly)
            .InstancePerLifetimeScope();

        // Register Unit of Work with Logger
        builder.RegisterType<ProductUnitOfWork>()
            .As<IInventoryUnitOfWork>()
            .WithParameter(
                (pi, ctx) => pi.ParameterType == typeof(ILogger<ProductUnitOfWork>),
                (pi, ctx) => ctx.Resolve<ILogger<ProductUnitOfWork>>()
            )
            .InstancePerLifetimeScope();

        // Register Management Services
        builder.RegisterAssemblyTypes(typeof(ProductManagementServices).Assembly)
            .Where(t => t.Name.EndsWith("Service") || t.Name.EndsWith("Services"))
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();

        // Register Repositories
        builder.RegisterAssemblyTypes(typeof(ProductRepository).Assembly)
            .Where(t => t.Name.EndsWith("Repository"))
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();

        // Registering Email Utility
        builder.RegisterType<UserProfileRepository>()
            .As<IUserProfileRepository>()
            .InstancePerLifetimeScope();

        // Register Email Utility
        builder.RegisterType<EmailUtility>()
            .As<IEmailUtility>()
            .InstancePerLifetimeScope();


        // Register UserDatabaseSeeder with necessary dependencies
        builder.RegisterType<UserDatabaseSeeder>().AsSelf().InstancePerLifetimeScope();
        //builder.RegisterType<UserService>().AsSelf().InstancePerLifetimeScope();
        

        // Register InventoryDatabaseSeeder with necessary dependencies
        builder.RegisterType<InventoryDatabaseSeeder>().AsSelf().InstancePerLifetimeScope();

        builder.RegisterType<UserService>().As <IUserService>().InstancePerLifetimeScope();

        // Registering AutoMapper
        builder.Register(c => new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<WebProfile>();
        }).CreateMapper()).As<IMapper>().InstancePerLifetimeScope();
    }
}
