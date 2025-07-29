using System.Collections.Generic;
using BugTrackerWithLayout.Models;

namespace BugTrackerWithLayout.ViewModels
{
    public class UserProfileViewModel
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public List<Bug> Bugs { get; set; }
    }
}
