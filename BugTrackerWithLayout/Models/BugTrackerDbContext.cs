using System.Collections.Generic;
using System.Data.Entity;

namespace BugTrackerWithLayout.Models
{
    public class BugTrackerDbContext : DbContext
    {
        public BugTrackerDbContext() : base("BugTrackerDbContext") { }

        public DbSet<Bug> Bugs { get; set; }
        public DbSet<User> Users { get; set; }

        public System.Data.Entity.DbSet<BugTrackerWithLayout.Models.Category> Categories { get; set; }
    }
}
