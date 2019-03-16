using Fixtures.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Fixtures.API.Data
{
    public class DataContext: DbContext
    {
        public DataContext(DbContextOptions<DataContext> options): base(options)
        {}

        public DbSet<User> Users { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Fixture> Fixtures { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            builder.Entity<Fixture>()
                    .HasOne(f => f.HomeTeam)
                    .WithMany(f => f.HomeFixtures)
                    .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Fixture>()
                    .HasOne(f => f.AwayTeam)
                    .WithMany(f => f.AwayFixtures)
                    .OnDelete(DeleteBehavior.Restrict);
        }
        
    }
}