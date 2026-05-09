using divelog.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace divelog.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();

            var model = new List<AdminUserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                model.Add(new AdminUserViewModel
                {
                    Id = user.Id,
                    Email = user.Email ?? "",
                    Role = roles.FirstOrDefault() ?? "Ingen roll"
                });
            }

            return View(model);
        }

        public IActionResult CreateUser()
        {
            return View();
        }

        //SKAPA ANVÄNDARE
        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new IdentityUser
            {
                UserName = model.Email,
                Email = model.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, model.Role);

                return RedirectToAction(nameof(Index));
            }

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

        //GET Redigera användares roller
        public async Task<IActionResult> ManageRoles(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            var allRoles = _roleManager.Roles.Select(r => r.Name!).ToList();

            var vm = new ManageRolesViewModel
            {
                UserId = user.Id,
                Email = user.Email!,
                SelectedRole = currentRoles.FirstOrDefault() ?? "",
                AvailableRoles = allRoles
            };

            return View(vm);
        }

        //POST Redigera användares roller
        [HttpPost]
        public async Task<IActionResult> ManageRoles(ManageRolesViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);

            if (user == null)
            {
                return NotFound();
            }

            var currentRoles = await _userManager.GetRolesAsync(user);

            // Skydd: admin får inte ta bort sin egen admin-roll
            if (user.Id == _userManager.GetUserId(User) &&
                currentRoles.Contains("Admin") &&
                model.SelectedRole != "Admin")
            {
                ModelState.AddModelError("", "Du kan inte ta bort din egen administratörsroll.");

                model.AvailableRoles = _roleManager.Roles
                    .Select(r => r.Name!)
                    .ToList();

                model.Email = user.Email!;

                return View(model);
            }

            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            await _userManager.AddToRoleAsync(user, model.SelectedRole);

            TempData["SuccessMessage"] = "Rollen uppdaterades!";

            return RedirectToAction(nameof(Index));
        }

        //GET Radera konto
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        //POST Radera konto
        [HttpPost, ActionName("DeleteUser")]
        public async Task<IActionResult> DeleteUserConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser != null && user.Id == currentUser.Id)
            {
                TempData["ErrorMessage"] = "Du kan inte ta bort ditt eget konto.";

                return RedirectToAction(nameof(Index));
            }

            await _userManager.DeleteAsync(user);

            TempData["SuccessMessage"] = "Användaren togs bort.";

            return RedirectToAction(nameof(Index));
        }

        //GET Återställ lösenord
        public async Task<IActionResult> ResetPassword(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return View(new ResetPasswordViewModel
            {
                UserId = user.Id,
                Email = user.Email!
            });
        }

        //POST Återställ lösenord
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);

            if (user == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var result = await _userManager.ResetPasswordAsync(
                user,
                token,
                model.NewPassword);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Lösenordet har uppdaterats!";
                return RedirectToAction(nameof(Index));
            }

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