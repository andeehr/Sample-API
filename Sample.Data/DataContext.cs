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
            });
        }
    }
}