using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Sample.Data.Entities;

namespace Sample.Data
{
    public class DataContext : DbContext
    {
        public IConfiguration Configuration { get; private set; }

        public virtual DbSet<User> Users { get; set; }

        public DataContext() : base()
        {
        }

        public DataContext(IConfiguration configuration) : base()
            => Configuration = configuration;

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            var connectionstring = Configuration?.GetConnectionString("DataContext");

            if (!options.IsConfigured)
            {
                options.UseSqlServer(connectionstring);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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
    }
}