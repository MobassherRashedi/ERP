using DevSkill.Inventory.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DevSkill.Inventory.Infrastructure
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser,
        ApplicationRole, Guid,
        ApplicationUserClaim, ApplicationUserRole,
        ApplicationUserLogin, ApplicationRoleClaim,
        ApplicationUserToken>
    {
        private readonly string _connectionString;
        private readonly string _migrationAssembly;

        public ApplicationDbContext(string connectionString, string migrationAssembly)
        {
            _connectionString = connectionString;
            _migrationAssembly = migrationAssembly;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(_connectionString,
                    x => x.MigrationsAssembly(_migrationAssembly));
            }

            base.OnConfiguring(optionsBuilder);
        }
        // DbSet for UserProfile
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Customize the UserProfile entity if needed
            modelBuilder.Entity<UserProfile>(entity =>
            {
                entity.HasKey(up => up.Id); // Set Id as primary key
                entity.Property(up => up.ProfilePicturePath)
                    .HasMaxLength(255) // Set max length for profile picture path
                    .IsRequired(false); // Allow null values

                // Configure relationship with ApplicationUser
                entity.HasOne(up => up.ApplicationUser)
                      .WithOne()
                      .HasForeignKey<UserProfile>(up => up.ApplicationUserId)
                      .OnDelete(DeleteBehavior.Cascade); // Delete UserProfile when ApplicationUser is deleted
            });
            // Many-to-many relationship between Role and RolePermissions
            modelBuilder.Entity<RolePermission>()
                .HasKey(rp => new { rp.RoleId, rp.Permission });

            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId);
        }
    }
}
