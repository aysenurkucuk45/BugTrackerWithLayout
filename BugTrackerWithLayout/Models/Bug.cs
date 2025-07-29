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

        public string Priority { get; set; } // Düşük, Orta, Yüksek 

        public string Status { get; set; } = "Açık"; // Açık, Devam ediyor, Tamamlandı

        public string ReportedBy { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string Attachment { get; set; }
        public string FilePath { get; set; }
        public string Solution { get; set; }
        public int? CategoryId { get; set; } // Nullable yaptık
        public virtual Category Category { get; set; }


    }
}
