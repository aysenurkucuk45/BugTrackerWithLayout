namespace BugTrackerWithLayout.Models
{
    public class ProjectUser
    {
        public int ProjectUserId { get; set; }

        // Foreign Keys
        public int ProjectId { get; set; }
        public int UserId { get; set; }

        // Navigation
        public virtual Project Project { get; set; }
        public virtual User User { get; set; }
    }
}
