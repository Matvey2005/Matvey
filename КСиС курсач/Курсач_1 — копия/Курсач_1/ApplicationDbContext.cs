using Microsoft.EntityFrameworkCore;
using Курсач_1.Configuration;
using Курсач_1.Models;

namespace Курсач_1
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User>? Users { get; set; }
        public DbSet<Event>? Events { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) 
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new EventConfiguration());
            base.OnModelCreating(modelBuilder);
        }
    }
}
