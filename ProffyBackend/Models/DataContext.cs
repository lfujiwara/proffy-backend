using Microsoft.EntityFrameworkCore;

namespace ProffyBackend.Models
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Subject> Subjects { get; set; }

        public DbSet<UserAPIKey> UserApiKeys { get; set; }

        public DbSet<AvailableTimeWindow> AvailableTimeWindows { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.PhoneNumber)
                .IsUnique();

            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasDefaultValue(Role.User);

            modelBuilder.Entity<UserAPIKey>()
                .HasIndex(k => k.Key)
                .IsUnique();

            modelBuilder.Entity<AvailableTimeWindow>()
                .HasIndex(a => new {a.WeekDay, a.OwnerId})
                .IsUnique();
        }
    }
}