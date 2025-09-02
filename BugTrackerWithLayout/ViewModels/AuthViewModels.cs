using System.ComponentModel.DataAnnotations;

namespace BugTrackerWithLayout.ViewModels
{
    public class RegisterViewModel
    {
        [Required, StringLength(32)]
        [Display(Name = "Kullanıcı Adı")]
        public string UserName { get; set; }

        [Required, EmailAddress]
        [Display(Name = "E-posta")]
        public string Email { get; set; }

        [Required, StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Parola")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Parolalar uyuşmuyor.")]
        [Display(Name = "Parola (Tekrar)")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Görünen Ad (opsiyonel)")]
        public string DisplayName { get; set; }

        [Display(Name = "Avatar")]
        public string Avatar { get; set; } // "Tweety","Bugs Bunny","Tom","Jerry"
    }

    public class LoginViewModel
    {
        [Required, Display(Name = "Kullanıcı Adı veya E-posta")]
        public string UserOrEmail { get; set; }

        [Required, DataType(DataType.Password), Display(Name = "Parola")]
        public string Password { get; set; }

        [Display(Name = "Beni Hatırla")]
        public bool RememberMe { get; set; }
    }
}
