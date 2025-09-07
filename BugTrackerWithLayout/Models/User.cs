using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BugTrackerWithLayout.Models
{
    [Table("Users")]
    public class User
    {
        public int Id { get; set; }

        [Required, StringLength(32)]
        public string Username { get; set; }   // << DB sütunu ile eşleşir

        // İstersen kaldırabilirsin, sadece kod kolaylığı
        [NotMapped]
        public string UserName
        {
            get => Username;
            set => Username = value;
        }

        [Required, EmailAddress, StringLength(120)]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [StringLength(60)]
        public string DisplayName { get; set; }

        [StringLength(20)]
        public string Avatar { get; set; }

        [StringLength(20)]
        public string Role { get; set; } = "User";
    }
}
