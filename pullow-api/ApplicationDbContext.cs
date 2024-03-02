using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using pullow_api.Authentication;
using pullow_api.Entities;

namespace pullow_api
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Goal> Goals { get; set; }
        public DbSet<ApplicationUserGoal> ApplicationUserGoals { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Goal>()
                .HasMany(e => e.ApplicationUsers)
                .WithMany(e => e.Goals)
                .UsingEntity<ApplicationUserGoal>();
        }

    }
}
