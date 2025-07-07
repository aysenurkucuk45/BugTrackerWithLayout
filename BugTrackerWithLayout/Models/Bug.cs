using System;
using System.ComponentModel.DataAnnotations;

namespace BugTrackerWithLayout.Models
{
    public class Bug
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        public string Priority { get; set; } // Düşük, Orta, Yüksek gibi

        public string Status { get; set; } = "Açık"; // Açık, Tamamlandı

        public string ReportedBy { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string Attachment { get; set; }
        public string FilePath { get; set; }

    }
}
