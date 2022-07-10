using HealthCare.API.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace HealthCare.API.Data
{
    public class DataContext:DbContext
    {
        public DataContext(DbContextOptions<DataContext>options):base(options)
        {
        }
        public DbSet <BloodType> BloodTypes { get; set; }
        public DbSet<diagonisic> diagonisics { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<BloodType>().HasIndex(x => x.Description).IsUnique();
            modelBuilder.Entity<diagonisic>().HasIndex(x => x.Description).IsUnique();
        }
    }
}
