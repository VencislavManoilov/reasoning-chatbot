using Microsoft.EntityFrameworkCore;
using App.Models;

namespace App.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Chat> Chats { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure DateTime properties to use UTC
            modelBuilder.Entity<User>()
                .Property(u => u.createdat)
                .HasConversion(
                    v => v, 
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
                );

            modelBuilder.Entity<User>()
                .Property(u => u.updatedat)
                .HasConversion(
                    v => v, 
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
                );

            modelBuilder.Entity<Chat>()
                .Property(c => c.createdat)
                .HasConversion(
                    v => v, 
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
                );

            modelBuilder.Entity<Chat>()
                .Property(c => c.updatedat)
                .HasConversion(
                    v => v, 
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
                );
        }
    }
}