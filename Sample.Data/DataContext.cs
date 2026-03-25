using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sample.Data.Entities;

namespace Sample.Data
{
    public class DataContext : DbContext
    {
        private static readonly SqliteConnection _keepAliveConnection = new("Data Source=:memory:");

        static DataContext()
        {
            _keepAliveConnection.Open();
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Permission> Permissions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if (!options.IsConfigured)
                options.UseSqlite(_keepAliveConnection);

            options.LogTo(s => System.Diagnostics.Debug.WriteLine(s),
                LogLevel.Information);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            InsertSeeds(modelBuilder);

            // User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Username, "UQ_User_Username")
                    .IsUnique();

                entity.Property(e => e.Username).HasMaxLength(30);
                entity.Property(e => e.FirstName).HasMaxLength(255);
                entity.Property(e => e.LastName).HasMaxLength(255);
                entity.Property(e => e.Password).HasMaxLength(450);

                entity.HasOne(e => e.Role)
                    .WithMany(pr => pr.Users)
                    .HasForeignKey(e => e.RoleId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_User_RoleId");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasIndex(e => e.Description, "UQ_Role_Description")
                    .IsUnique();

                entity.Property(e => e.Description).HasMaxLength(255);

                entity.HasMany(e => e.Permissions)
                    .WithMany(pr => pr.Roles);
            });

            modelBuilder.Entity<Permission>(entity =>
            {
                entity.HasIndex(e => e.Description, "UQ_Permission_Description")
                    .IsUnique();

                entity.Property(e => e.Description).HasMaxLength(255);

                entity.HasMany(e => e.Roles)
                    .WithMany(pr => pr.Permissions);
            });
        }

        private void InsertSeeds(ModelBuilder modelBuilder)
        {
            // Roles
            modelBuilder.Entity<Role>().HasData(
                new Role("Admin", 1),
                new Role("User", 2)
            );

            // Permissions
            modelBuilder.Entity<Permission>().HasData(
                new Permission("user.list", 1),
                new Permission("user.manage", 2)
            );

            modelBuilder.Entity<Role>()
                .HasMany(r => r.Permissions)
                .WithMany(p => p.Roles)
                .UsingEntity(j => j.HasData(
                    new { RolesId = 1, PermissionsId = 1 }, // Admin - List
                    new { RolesId = 1, PermissionsId = 2 }, // Admin - Manage
                    new { RolesId = 2, PermissionsId = 1 }  // User - List
                ));
        }
    }
}