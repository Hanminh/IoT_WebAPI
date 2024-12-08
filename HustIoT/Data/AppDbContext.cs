using HustIoT.Models;
using Microsoft.EntityFrameworkCore;

namespace HustIoT.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Coordinate> Coordinates { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the Coordinate table
            modelBuilder.Entity<Coordinate>(entity =>
            {
                entity.HasKey(c => c.Id); // Primary key
                entity.Property(c => c.Id)
                      .ValueGeneratedOnAdd(); // Auto-increment for the Id column
            });
        }
    }
}
