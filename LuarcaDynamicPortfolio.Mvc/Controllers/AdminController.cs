using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LuarcaDynamicPortfolio.Mvc.Models.Entities;
using LuarcaDynamicPortfolio.Mvc.Models.ViewModels.Admin;
using LuarcaDynamicPortfolio.Mvc.Services.Interfaces;

namespace LuarcaDynamicPortfolio.Mvc.Controllers
{
    [Authorize(Roles = "Admin")] // Requirement: Admin only
    public class AdminController : Controller
    {
        private readonly IUserService _userService;
        private readonly IContentService _contentService;

        public AdminController(IUserService userService, IContentService contentService)
        {
            _userService = userService;
            _contentService = contentService;
        }

        private int GetCurrentAdminId()
        {
            var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(idStr, out var id) ? id : 1;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string activeTab = "users")
        {
            ViewBag.ActiveTab = activeTab;

            var (total, active, inactive, adminCount, regularCount, signups7Days) = await _userService.GetUserStatsAsync();
            var users = await _userService.GetAllUsersAsync();
            var activity = await _userService.GetRecentActivityAsync(25);

            var company = await _contentService.GetCompanyInfoAsync();
            var productsServices = await _contentService.GetProductsServicesAsync(activeOnly: false);
            var contact = await _contentService.GetContactInfoAsync();
            var owner = await _contentService.GetOwnerInfoAsync();

            var viewModel = new AdminDashboardViewModel
            {
                TotalUsers = total,
                ActiveUsers = active,
                InactiveUsers = inactive,
                AdminCount = adminCount,
                RegularUserCount = regularCount,
                SignupsLast7Days = signups7Days,
                Users = users,
                RecentActivity = activity,
                Company = company,
                ProductsServices = productsServices,
                Contact = contact,
                Owner = owner
            };

            return View(viewModel);
        }

        // ==================== USERS MANAGEMENT (View-Only Info & Status Toggle/Delete) ====================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleUserStatus(int userId, bool isActive)
        {
            var adminId = GetCurrentAdminId();

            // Safety: Admin cannot deactivate their own account
            if (userId == adminId && !isActive)
            {
                TempData["ErrorMessage"] = "You cannot deactivate your own administrator account.";
                return RedirectToAction("Index", new { activeTab = "users" });
            }

            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            var success = await _userService.SetUserActiveStatusAsync(userId, isActive, adminId, ip);
            if (success)
            {
                TempData["SuccessMessage"] = $"User status has been updated to {(isActive ? "Active" : "Inactive")}.";
            }
            else
            {
                TempData["ErrorMessage"] = "Could not update user status.";
            }

            return RedirectToAction("Index", new { activeTab = "users" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            var adminId = GetCurrentAdminId();

            if (userId == adminId)
            {
                TempData["ErrorMessage"] = "You cannot delete your own administrator account.";
                return RedirectToAction("Index", new { activeTab = "users" });
            }

            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            var success = await _userService.DeleteUserAsync(userId, adminId, ip);
            if (success)
            {
                TempData["SuccessMessage"] = "User account has been successfully deleted.";
            }
            else
            {
                TempData["ErrorMessage"] = "Could not delete user account.";
            }

            return RedirectToAction("Index", new { activeTab = "users" });
        }

        // ==================== WEBSITE CONTENT MANAGEMENT (4 Rubric Sections) ====================

        // Section A: About the Business / Company
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCompany(CompanyInfo model)
        {
            if (!ModelState.IsValid)
            {
                var error = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage;
                TempData["ErrorMessage"] = error ?? "Please correct the errors in the Company form.";
                return RedirectToAction("Index", new { activeTab = "content-company" });
            }

            var adminId = GetCurrentAdminId();
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();

            await _contentService.UpdateCompanyInfoAsync(model, adminId, ip);
            TempData["SuccessMessage"] = "Company information updated successfully!";

            return RedirectToAction("Index", new { activeTab = "content-company" });
        }

        // Section B: Products and Services
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpsertProductService(ProductService model)
        {
            if (!ModelState.IsValid)
            {
                var error = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage;
                TempData["ErrorMessage"] = error ?? "Please correct the errors in the Product/Service form.";
                return RedirectToAction("Index", new { activeTab = "content-products" });
            }

            var adminId = GetCurrentAdminId();
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();

            await _contentService.UpsertProductServiceAsync(model, adminId, ip);
            TempData["SuccessMessage"] = "Product/Service saved successfully!";

            return RedirectToAction("Index", new { activeTab = "content-products" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProductService(int id)
        {
            var adminId = GetCurrentAdminId();
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();

            await _contentService.DeleteProductServiceAsync(id, adminId, ip);
            TempData["SuccessMessage"] = "Product/Service deleted successfully!";

            return RedirectToAction("Index", new { activeTab = "content-products" });
        }

        // Section C: Contact Information
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateContact(ContactInfo model)
        {
            if (!ModelState.IsValid)
            {
                var error = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage;
                TempData["ErrorMessage"] = error ?? "Please correct the errors in the Contact form.";
                return RedirectToAction("Index", new { activeTab = "content-contact" });
            }

            var adminId = GetCurrentAdminId();
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();

            await _contentService.UpdateContactInfoAsync(model, adminId, ip);
            TempData["SuccessMessage"] = "Contact Information updated successfully!";

            return RedirectToAction("Index", new { activeTab = "content-contact" });
        }

        // Section D: Owner's Information
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOwner(OwnerInfo model)
        {
            if (!ModelState.IsValid)
            {
                var error = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage;
                TempData["ErrorMessage"] = error ?? "Please correct the errors in the Owner form.";
                return RedirectToAction("Index", new { activeTab = "content-owner" });
            }

            var adminId = GetCurrentAdminId();
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();

            await _contentService.UpdateOwnerInfoAsync(model, adminId, ip);
            TempData["SuccessMessage"] = "Owner's Information updated successfully!";

            return RedirectToAction("Index", new { activeTab = "content-owner" });
        }
    }
}
