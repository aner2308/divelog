using System.ComponentModel.DataAnnotations;

namespace divelog.ViewModels
{
    public class CreateUserViewModel
    {
        [Required(ErrorMessage = "E-post krävs")]
        [EmailAddress(ErrorMessage = "Ogiltig e-postadress")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lösenord krävs")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = "Reader";
    }
}