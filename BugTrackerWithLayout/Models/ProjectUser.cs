using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTrackerWithLayout.Models
{
    public class ProjectUser
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int UserId { get; set; }
        public string Role { get; set; }
    }
}
