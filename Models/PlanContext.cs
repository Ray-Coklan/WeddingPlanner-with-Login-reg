using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace WeddingPlanner.Models
{
    public class PlanContext : DbContext
    {
        // base() calls the parent class' constructor passing the "options" parameter along
        public PlanContext(DbContextOptions<PlanContext> options) : base(options) { }
        public DbSet<User> Users { get;set; }
        public DbSet<Planner> Planners { get;set; }
        public DbSet<Guest> Guests {get;set;}
    }
}