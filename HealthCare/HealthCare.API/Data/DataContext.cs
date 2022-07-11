using HealthCare.API.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace HealthCare.API.Data
{
    public class DataContext:IdentityDbContext<User>
    {
        public DataContext(DbContextOptions<DataContext>options):base(options)
        {
        }
        public DbSet <BloodType> BloodTypes { get; set; }
        public DbSet<diagonisic> diagonisics { get; set; }
        public DbSet<Natianality> natianalities { get; set; }
        public DbSet<City> Cities { get; set; } 
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<BloodType>().HasIndex(x => x.Description).IsUnique();
            modelBuilder.Entity<diagonisic>().HasIndex(x => x.Description).IsUnique();
            modelBuilder.Entity<Natianality>().HasIndex(x => x.Description).IsUnique();
            modelBuilder.Entity<City>().HasIndex(x => x.Description).IsUnique();    
        }
    }
}
