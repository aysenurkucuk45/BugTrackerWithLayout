using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace BugTrackerWithLayout.Models
{
    public class Bug
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        public string Priority { get; set; }

        public string Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public string ReportedBy { get; set; }

        public int CategoryId { get; set; }

        public virtual Category Category { get; set; }
        public string Attachment { get; set; }  // dosya adı gibi düşün
        public string FilePath { get; set; }    // tam dosya yolu (opsiyonel)
        public string Solution { get; set; }    // çözüm içeriği (eğer ayrı tablo yapmadıysan)
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Solution> Solutions { get; set; }


    }

}
