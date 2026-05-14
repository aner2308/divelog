using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace divelog.Areas.Identity.Pages.Account
{
    public class LogoutModel : PageModel
    {
        //Hanterar utloggning av användare
        private readonly SignInManager<IdentityUser> _signInManager;

        //Konstruktor som injicerar SignInManager
        public LogoutModel(SignInManager<IdentityUser> signInManager)
        {
            _signInManager = signInManager;
        }

        //Körs när användaren loggar ut med POST
        public async Task<IActionResult> OnPost()
        {
            //Loggar ut användare
            await _signInManager.SignOutAsync();

            //Skickar användare till startsidan
            return RedirectToAction("Index", "Home");
        }
    }
}