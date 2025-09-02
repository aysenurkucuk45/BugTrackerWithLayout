using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // [Index] için (EF 6.1+)

namespace BugTrackerWithLayout.Models
{
    [Table("Users")] // tablo adını netleştirelim
    public class User   // İsmi koruyoruz
    {
        public int Id { get; set; }

        [Required, StringLength(32)]
        [Index("IX_User_UserName", IsUnique = true)]
        public string UserName { get; set; }

        [Required, EmailAddress, StringLength(120)]
        [Index("IX_User_Email", IsUnique = true)]
        public string Email { get; set; }

        [Required]                  // PBKDF2 hash (salt içerir)
        public string PasswordHash { get; set; }

        [StringLength(60)]
        public string DisplayName { get; set; }

        // Basit avatar adı (Tweety, Bugs Bunny, Tom, Jerry)
        // İleride tam dosya yolu kullanacaksan uzunluğu artırırız.
        [StringLength(20)]
        public string Avatar { get; set; }

        [StringLength(20)]
        public string Role { get; set; } = "User";
    }
}
