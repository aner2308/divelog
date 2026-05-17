using divelog.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace divelog.Controllers
{
    //Endast ADMIN kan komma åt controllern
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        //Hantera användare med ASP.NET Identity
        private readonly UserManager<IdentityUser> _userManager;
        //Hantera roller med ASP.NET Identity
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        //VISA alla användare
        public async Task<IActionResult> Index()
        {

            //Hämtar in användare
            var users = _userManager.Users.ToList();

            //Lista som skickas till vyn
            var model = new List<AdminUserViewModel>();

            //Loopar igenom användare
            foreach (var user in users)
            {

                //Hämtar användarens roll
                var roles = await _userManager.GetRolesAsync(user);

                //Lägger till användaren i ViewModel-listan
                model.Add(new AdminUserViewModel
                {
                    Id = user.Id,
                    Email = user.Email ?? "",
                    Role = roles.FirstOrDefault() ?? "Ingen roll"
                });
            }

            return View(model);
        }
        
        //GET - SKAPA användare
        public IActionResult CreateUser()
        {
            return View();
        }

        //POST - SKAPA användare
        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserViewModel model)
        {

            //Kontrollerar formuläret
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //Skapar ny användare
            var user = new IdentityUser
            {
                UserName = model.Email,
                Email = model.Email,
                EmailConfirmed = true
            };

            //Skapar användaren i databasen
            var result = await _userManager.CreateAsync(user, model.Password);

            //Om skapandet lyckas
            if (result.Succeeded)
            {
                //Lägger till vald roll
                await _userManager.AddToRoleAsync(user, model.Role);

                return RedirectToAction(nameof(Index));
            }

            //Översätter felkoder till svenska meddelanden
            foreach (var error in result.Errors)
            {
                var message = error.Code switch
                {
                    "PasswordTooShort" => "Lösenordet är för kort (minst 8 tecken).",
                    "PasswordRequiresNonAlphanumeric" => "Lösenordet måste innehålla ett specialtecken.",
                    "PasswordRequiresDigit" => "Lösenordet måste innehålla minst en siffra.",
                    "PasswordRequiresUpper" => "Lösenordet måste innehålla minst en stor bokstav.",
                    "DuplicateUserName" => "E-postadressen är redan registrerad.",
                    _ => error.Description
                };

                ModelState.AddModelError("", message);
            }

            return View(model);
        }

        //GET - Redigera användares roll
        public async Task<IActionResult> ManageRoles(string id)
        {
            //Hämta användaren
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            //Hämtar nuvarande roll
            var currentRoles = await _userManager.GetRolesAsync(user);
            //Hämtar alla roller från databasen
            var allRoles = _roleManager.Roles.Select(r => r.Name!).ToList();

            //Skapar ViewModel
            var vm = new ManageRolesViewModel
            {
                UserId = user.Id,
                Email = user.Email!,
                SelectedRole = currentRoles.FirstOrDefault() ?? "",
                AvailableRoles = allRoles
            };

            return View(vm);
        }

        //POST - Redigera användares roll
        [HttpPost]
        public async Task<IActionResult> ManageRoles(ManageRolesViewModel model)
        {
            //Hämta användaren
            var user = await _userManager.FindByIdAsync(model.UserId);

            if (user == null)
            {
                return NotFound();
            }

            //Hämtar användarens nuvarande roll
            var currentRoles = await _userManager.GetRolesAsync(user);

            // Skydd så att admin inte kan ta bort sin egen admin-roll
            if (user.Id == _userManager.GetUserId(User) &&
                currentRoles.Contains("Admin") &&
                model.SelectedRole != "Admin")
            {
                ModelState.AddModelError("", "Du kan inte ta bort din egen administratörsroll.");

                //Laddar om roller om sidan visas igen
                model.AvailableRoles = _roleManager.Roles
                    .Select(r => r.Name!)
                    .ToList();

                model.Email = user.Email!;

                return View(model);
            }

            //Tar bort användarens tidigare valda roll
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            //Lägger till nya rollen
            await _userManager.AddToRoleAsync(user, model.SelectedRole);
            //Meddelar att uppdateringen lyckades
            TempData["SuccessMessage"] = "Rollen uppdaterades!";

            return RedirectToAction(nameof(Index));
        }

        //GET - Radera användare
        public async Task<IActionResult> DeleteUser(string id)
        {
            //Hämtar användaren
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        //POST - Radera anvndare
        [HttpPost, ActionName("DeleteUser")]
        public async Task<IActionResult> DeleteUserConfirmed(string id)
        {
            //Hämtar användaren som ska tas bort
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }
            //Hämtar inloggad användare
            var currentUser = await _userManager.GetUserAsync(User);
            
            //Skydd för att inloggad admin inte kan ta bort sitt eget konto
            if (currentUser != null && user.Id == currentUser.Id)
            {
                TempData["ErrorMessage"] = "Du kan inte ta bort ditt eget konto.";

                return RedirectToAction(nameof(Index));
            }

            //Tar bort användaren
            await _userManager.DeleteAsync(user);

            //Meddelande om lyckad radering
            TempData["SuccessMessage"] = "Användaren togs bort.";

            return RedirectToAction(nameof(Index));
        }

        //GET - Återställ lösenord
        public async Task<IActionResult> ResetPassword(string id)
        {
            //Hämtar användaren
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            //Skickar information till formuläret
            return View(new ResetPasswordViewModel
            {
                UserId = user.Id,
                Email = user.Email!
            });
        }

        //POST - Återställ lösenord
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            //Hämtar användaren
            var user = await _userManager.FindByIdAsync(model.UserId);

            if (user == null)
            {
                return NotFound();
            }

            //Kontroll av formulär
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //Skapar återställningstoken
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            //Återställer lösenordet
            var result = await _userManager.ResetPasswordAsync(
                user,
                token,
                model.NewPassword);

            //Om återställningen lyckades
            if (result.Succeeded)
            {
                //Meddelande om lyckad återställning
                TempData["SuccessMessage"] = "Lösenordet har uppdaterats!";
                return RedirectToAction(nameof(Index));
            }

            //Byter ut felmeddelanden mot svenska
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    var message = error.Code switch
                    {
                        "PasswordTooShort" => "Lösenordet är för kort (minst 8 tecken).",
                        "PasswordRequiresNonAlphanumeric" => "Lösenordet måste innehålla ett specialtecken.",
                        "PasswordRequiresDigit" => "Lösenordet måste innehålla minst en siffra.",
                        "PasswordRequiresUpper" => "Lösenordet måste innehålla minst en stor bokstav.",
                        "DuplicateUserName" => "E-postadressen är redan registrerad.",
                        _ => error.Description
                    };

                    ModelState.AddModelError("", message);
                }

                return View(model);
            }

            return View(model);
        }
    }
}