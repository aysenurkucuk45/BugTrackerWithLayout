using System.Collections.Generic;
using BugTrackerWithLayout.Models;

namespace BugTrackerWithLayout.ViewModels
{
    public class UserProfileViewModel
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }

        // Profil resmi yolu (örnek: ~/Content/Images/Tom.png)
        public string ProfileImagePath { get; set; }

        // Hata sayıları
        public int TotalBugs { get; set; }
        public int OpenBugCount { get; set; }
        public int InProgressBugCount { get; set; }
        public int ResolvedBugCount { get; set; }

        // Yorum ve çözüm istatistikleri
        public int CommentCount { get; set; }
        public int SolutionCount { get; set; }

        // Kullanıcının hataları listesi
        public List<Bug> Bugs { get; set; }
    }
}
