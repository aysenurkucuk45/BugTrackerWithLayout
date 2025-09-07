using System;
using System.ComponentModel.DataAnnotations;

namespace BugTrackerWithLayout.Models
{
    public class Solution
    {
        public int Id { get; set; }
        public int BugId { get; set; }

        [Required, StringLength(64)]
        public string Author { get; set; }

        [Required]
        public string Text { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
