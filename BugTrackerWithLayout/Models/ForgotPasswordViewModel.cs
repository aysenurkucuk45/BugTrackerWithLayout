using System.ComponentModel.DataAnnotations;

public class ForgotPasswordViewModel
{
    [Required(ErrorMessage = "E-posta adresi gereklidir")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi girin")]
    public string Email { get; set; }
}
