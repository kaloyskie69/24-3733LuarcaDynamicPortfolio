using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LuarcaDynamicPortfolio.Mvc.Models;
using LuarcaDynamicPortfolio.Mvc.Models.ViewModels.Home;
using LuarcaDynamicPortfolio.Mvc.Services.Interfaces;

namespace LuarcaDynamicPortfolio.Mvc.Controllers
{
    [Authorize] // Requirement: User need to Sign-In first before he/she can view the website
    public class HomeController : Controller
    {
        private readonly IContentService _contentService;

        public HomeController(IContentService contentService)
        {
            _contentService = contentService;
        }

        public async Task<IActionResult> Index()
        {
            var model = new DynamicWebsiteViewModel
            {
                Company = await _contentService.GetCompanyInfoAsync(),
                ProductsServices = await _contentService.GetProductsServicesAsync(activeOnly: true),
                Contact = await _contentService.GetContactInfoAsync(),
                Owner = await _contentService.GetOwnerInfoAsync(),
                Projects = await _contentService.GetProjectsAsync(),
                EducationList = await _contentService.GetEducationAsync(),
                SkillsList = await _contentService.GetSkillsAsync(),
                HobbiesList = await _contentService.GetHobbiesAsync(),
                CurrentUserName = User.FindFirst("FullName")?.Value ?? User.Identity?.Name ?? "User"
            };

            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [AllowAnonymous]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
