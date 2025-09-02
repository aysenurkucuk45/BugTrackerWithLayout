// Models/BugTrackerDbContext.cs
using System.Data.Entity;

namespace BugTrackerWithLayout.Models
{
    public class BugTrackerDbContext : DbContext
    {
        public BugTrackerDbContext()
            : base("BugTrackerDbContext")
        {
        }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Solution> Solutions { get; set; }

        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectUser> ProjectUsers { get; set; }
        public DbSet<Bug> Bugs { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }

    }
}
