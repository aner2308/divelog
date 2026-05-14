using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace divelog.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        //Hanterar inloggning och autentisering av användare
        private readonly SignInManager<IdentityUser> _signInManager;

        //Konstruktor som injicerar SignInManager
        public LoginModel(SignInManager<IdentityUser> signInManager)
        {
            _signInManager = signInManager;
        }

        //Binder formulärdata från Login formuläret
        [BindProperty]
        public InputModel Input { get; set; } = new();

        //URL som användren ska skickas tillbaka till efter inloggning
        public string? ReturnUrl { get; set; }

        //Modell för inloggningsformuläret
        public class InputModel
        {
            //Email med validering
            [Required(ErrorMessage = "E-post krävs")]
            [EmailAddress]
            [Display(Name = "E-post")]
            public string Email { get; set; } = string.Empty;

            //Lösenord med validering
            [Required(ErrorMessage = "Lösenord krävs")]
            [DataType(DataType.Password)]
            [Display(Name = "Lösenord")]
            public string Password { get; set; } = string.Empty;

            //Checkbox för att komma ihåg användaren
            [Display(Name = "Kom ihåg mig")]
            public bool RememberMe { get; set; }
        }

        //Körs när sidan öppnas via get 
        public void OnGet(string? returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        //Körs när formuläret skickas via POST
        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            //Standard URL om ingen return URL skickas med
            ReturnUrl ??= Url.Content("~/");

            //Kontroll av korrekt ifyllt formulär
            if (!ModelState.IsValid)
            {
                return Page();
            }

            //Hämtar användaren baserat på Email
            var user = await _signInManager.UserManager.FindByEmailAsync(Input.Email);

            //Felmeddelande om användare inte finns
            if (user == null)
            {
                ModelState.AddModelError("", "Felaktig e-post eller lösenord");
                return Page();
            }

            //Försöker logga in användare
            var result = await _signInManager.PasswordSignInAsync(
                user.UserName!,
                Input.Password,
                Input.RememberMe,
                lockoutOnFailure: false);

            //Om resultat lyckas skickas användaren vidare
            if (result.Succeeded)
            {
                return LocalRedirect(ReturnUrl);
            }

            //Felmeddelande vid misslyckad inloggning
            ModelState.AddModelError(string.Empty, "Felaktig e-post eller lösenord");

            return Page();
        }
    }
}