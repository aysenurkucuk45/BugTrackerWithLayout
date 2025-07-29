using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace BugTrackerWithLayout.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Kategori adı boş bırakılamaz.")]
        [StringLength(100)]
        public string Name { get; set; }

        // Hatalarla ilişki (1 kategori - N hata)
        public virtual ICollection<Bug> Bugs { get; set; }
    }
}
