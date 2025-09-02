using BugTrackerWithLayout.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BugTrackerWithLayout.Models
{
    public class Comment
    {
        public int Id { get; set; }

        public string Content { get; set; }

        public DateTime CreatedAt { get; set; }

        public int BugId { get; set; }

        public virtual Bug Bug { get; set; }
        public string Author { get; set; }
    }
}

