using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace divelog.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;

        public LoginModel(SignInManager<IdentityUser> signInManager)
        {
            _signInManager = signInManager;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public string? ReturnUrl { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "E-post krävs")]
            [EmailAddress]
            [Display(Name = "E-post")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "Lösenord krävs")]
            [DataType(DataType.Password)]
            [Display(Name = "Lösenord")]
            public string Password { get; set; } = string.Empty;

            [Display(Name = "Kom ihåg mig")]
            public bool RememberMe { get; set; }
        }

        public void OnGet(string? returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            ReturnUrl ??= Url.Content("~/");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _signInManager.UserManager.FindByEmailAsync(Input.Email);

            if (user == null)
            {
                ModelState.AddModelError("", "Felaktig e-post eller lösenord");
                return Page();
            }

            var result = await _signInManager.PasswordSignInAsync(
                user.UserName!,
                Input.Password,
                Input.RememberMe,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return LocalRedirect(ReturnUrl);
            }

            ModelState.AddModelError(string.Empty, "Felaktig e-post eller lösenord");

            return Page();
        }
    }
}