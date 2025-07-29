using System.ComponentModel.DataAnnotations;

namespace BugTrackerWithLayout.Models
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Kullanıcı Adı")]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "E-posta")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Rol")]
        public string Role { get; set; } // "Admin" veya "Kullanıcı"
    }
}
