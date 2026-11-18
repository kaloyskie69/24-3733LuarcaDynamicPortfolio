using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LuarcaDynamicPortfolio.Mvc.Models.ViewModels.Profile;
using LuarcaDynamicPortfolio.Mvc.Services.Interfaces;

namespace LuarcaDynamicPortfolio.Mvc.Controllers
{
    [Authorize] // Requirement: User can modify their own profile
    public class ProfileController : Controller
    {
        private readonly IUserService _userService;

        public ProfileController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out var userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _userService.GetByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var model = new UserProfileViewModel
            {
                UserId = user.UserId,
                Username = user.Username,
                FullName = user.FullName ?? string.Empty,
                Email = user.Email,
                RoleName = user.Role?.RoleName ?? "User",
                DateCreated = user.DateCreated,
                IsActive = user.IsActive
            };

            ViewBag.PasswordModel = new ChangePasswordViewModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(UserProfileViewModel model)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out var userId) || userId != model.UserId)
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.PasswordModel = new ChangePasswordViewModel();
                return View("Index", model);
            }

            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var success = await _userService.UpdateProfileAsync(userId, model.FullName, model.Email, ipAddress);

            if (!success)
            {
                ModelState.AddModelError("Email", "Could not update profile. That email address may already be in use by another user.");
                ViewBag.PasswordModel = new ChangePasswordViewModel();
                return View("Index", model);
            }

            TempData["SuccessMessage"] = "Your profile information has been successfully updated!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel passwordModel)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out var userId))
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                var user = await _userService.GetByIdAsync(userId);
                var profileModel = new UserProfileViewModel
                {
                    UserId = user!.UserId,
                    Username = user.Username,
                    FullName = user.FullName ?? string.Empty,
                    Email = user.Email,
                    RoleName = user.Role?.RoleName ?? "User",
                    DateCreated = user.DateCreated,
                    IsActive = user.IsActive
                };
                ViewBag.PasswordModel = passwordModel;
                return View("Index", profileModel);
            }

            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var (success, error) = await _userService.ChangePasswordAsync(userId, passwordModel.CurrentPassword, passwordModel.NewPassword, ipAddress);

            if (!success)
            {
                TempData["ErrorMessage"] = error ?? "Failed to change password. Please verify your current password.";
                return RedirectToAction("Index");
            }

            TempData["SuccessMessage"] = "Your password has been changed successfully!";
            return RedirectToAction("Index");
        }
    }
}
