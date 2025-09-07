using System.Collections.Generic;
using BugTrackerWithLayout.Models;

namespace BugTrackerWithLayout.ViewModels
{
    public class UserProfileViewModel
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string ProfileImagePath { get; set; }
        public List<Bug> Bugs { get; set; }
        public string Role { get; set; }

        public int TotalBugs { get; set; }
        public int OpenBugCount { get; set; }
        public int InProgressBugCount { get; set; }
        public int ResolvedBugCount { get; set; }
        public int CommentCount { get; set; }
        public int SolutionCount { get; set; }
        public string Avatar { get; set; }
    }
}
