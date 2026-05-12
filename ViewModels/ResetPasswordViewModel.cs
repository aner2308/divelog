using System.ComponentModel.DataAnnotations;

namespace divelog.ViewModels
{
    public class ResetPasswordViewModel
    {
        public string UserId { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Nytt lösenord:")]
        public string NewPassword { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Bekräfta lösenord:")]
        [Compare("NewPassword", ErrorMessage = "Lösenorden matchar inte")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}