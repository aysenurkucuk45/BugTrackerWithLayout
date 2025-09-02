using System.Collections.Generic;

namespace BugTrackerWithLayout.Models
{
    public class Project
    {
        public int ProjectId { get; set; }
        public string Name { get; set; }        // Proje adı
        public string Description { get; set; } // İsteğe bağlı açıklama

        // İlişkiler
        public virtual ICollection<ProjectUser> ProjectUsers { get; set; }
        public virtual ICollection<Bug> Bugs { get; set; }
    }
}
