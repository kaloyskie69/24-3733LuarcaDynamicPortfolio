using System.Collections.Generic;
using System.Threading.Tasks;
using LuarcaDynamicPortfolio.Mvc.Models.Entities;

namespace LuarcaDynamicPortfolio.Mvc.Services.Interfaces
{
    public interface IContentService
    {
        // 4 Rubric Sections
        Task<CompanyInfo> GetCompanyInfoAsync();
        Task<bool> UpdateCompanyInfoAsync(CompanyInfo model, int adminUserId, string? ipAddress);

        Task<List<ProductService>> GetProductsServicesAsync(bool activeOnly = false);
        Task<ProductService?> GetProductServiceByIdAsync(int id);
        Task<bool> UpsertProductServiceAsync(ProductService model, int adminUserId, string? ipAddress);
        Task<bool> DeleteProductServiceAsync(int id, int adminUserId, string? ipAddress);

        Task<ContactInfo> GetContactInfoAsync();
        Task<bool> UpdateContactInfoAsync(ContactInfo model, int adminUserId, string? ipAddress);

        Task<OwnerInfo> GetOwnerInfoAsync();
        Task<bool> UpdateOwnerInfoAsync(OwnerInfo model, int adminUserId, string? ipAddress);

        // Portfolio compatibility
        Task<List<Education>> GetEducationAsync();
        Task<List<Skill>> GetSkillsAsync();
        Task<List<Hobby>> GetHobbiesAsync();
        Task<List<Project>> GetProjectsAsync();
    }
}
