using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LuarcaDynamicPortfolio.Mvc.Data;
using LuarcaDynamicPortfolio.Mvc.Models.Entities;
using LuarcaDynamicPortfolio.Mvc.Services.Interfaces;

namespace LuarcaDynamicPortfolio.Mvc.Services.Implementations
{
    public class ContentService : IContentService
    {
        private readonly PersonalWebsiteDbContext _context;
        private readonly IAuditService _auditService;

        public ContentService(PersonalWebsiteDbContext context, IAuditService auditService)
        {
            _context = context;
            _auditService = auditService;
        }

        // ==================== 1. ABOUT THE BUSINESS / COMPANY ====================

        public async Task<CompanyInfo> GetCompanyInfoAsync()
        {
            var company = await _context.CompanyInfo.OrderByDescending(c => c.CompanyInfoId).FirstOrDefaultAsync();
            if (company == null)
            {
                company = new CompanyInfo
                {
                    CompanyName = "Luarca Web Solutions & Technologies",
                    Tagline = "Building Modern, Scalable, and Dynamic Enterprise Web Applications",
                    AboutDescription = "Specializing in enterprise web architecture, ASP.NET Core MVC, Entity Framework Core, and modern MS SQL systems.",
                    Mission = "Deliver high-performance, robust, and clean digital experiences.",
                    Vision = "Empower businesses and clients with modern, reliable software.",
                    LastModifiedDate = DateTime.Now
                };
                _context.CompanyInfo.Add(company);
                await _context.SaveChangesAsync();
            }
            return company;
        }

        public async Task<bool> UpdateCompanyInfoAsync(CompanyInfo model, int adminUserId, string? ipAddress)
        {
            var existing = await _context.CompanyInfo.FindAsync(model.CompanyInfoId);
            if (existing == null)
            {
                existing = new CompanyInfo();
                _context.CompanyInfo.Add(existing);
            }

            existing.CompanyName = model.CompanyName.Trim();
            existing.Tagline = model.Tagline?.Trim();
            existing.AboutDescription = model.AboutDescription?.Trim();
            existing.Mission = model.Mission?.Trim();
            existing.Vision = model.Vision?.Trim();
            existing.LastModifiedDate = DateTime.Now;

            await _context.SaveChangesAsync();
            await _auditService.LogAsync(adminUserId, "ADMIN_UPDATE_CONTENT", "Updated About Business/Company section", ipAddress);
            return true;
        }

        // ==================== 2. PRODUCTS AND SERVICES ====================

        public async Task<List<ProductService>> GetProductsServicesAsync(bool activeOnly = false)
        {
            var query = _context.ProductServices.AsQueryable();
            if (activeOnly)
            {
                query = query.Where(p => p.IsActive);
            }
            return await query.OrderBy(p => p.DisplayOrder).ThenBy(p => p.Title).ToListAsync();
        }

        public async Task<ProductService?> GetProductServiceByIdAsync(int id)
        {
            return await _context.ProductServices.FindAsync(id);
        }

        public async Task<bool> UpsertProductServiceAsync(ProductService model, int adminUserId, string? ipAddress)
        {
            if (model.ProductServiceId > 0)
            {
                var existing = await _context.ProductServices.FindAsync(model.ProductServiceId);
                if (existing == null) return false;

                existing.Title = model.Title.Trim();
                existing.Description = model.Description.Trim();
                existing.Category = string.IsNullOrWhiteSpace(model.Category) ? "Service" : model.Category.Trim();
                existing.Tags = model.Tags?.Trim();
                existing.ActionUrl = model.ActionUrl?.Trim();
                existing.DisplayOrder = model.DisplayOrder;
                existing.IsActive = model.IsActive;

                await _auditService.LogAsync(adminUserId, "ADMIN_UPDATE_CONTENT", $"Updated Product/Service: {model.Title}", ipAddress);
            }
            else
            {
                model.Title = model.Title.Trim();
                model.Description = model.Description.Trim();
                model.Category = string.IsNullOrWhiteSpace(model.Category) ? "Service" : model.Category.Trim();
                model.Tags = model.Tags?.Trim();
                model.ActionUrl = model.ActionUrl?.Trim();

                _context.ProductServices.Add(model);
                await _auditService.LogAsync(adminUserId, "ADMIN_UPDATE_CONTENT", $"Added Product/Service: {model.Title}", ipAddress);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteProductServiceAsync(int id, int adminUserId, string? ipAddress)
        {
            var item = await _context.ProductServices.FindAsync(id);
            if (item == null) return false;

            // Soft delete by default or remove
            item.IsActive = false;
            await _context.SaveChangesAsync();

            await _auditService.LogAsync(adminUserId, "ADMIN_UPDATE_CONTENT", $"Deactivated Product/Service {id} ({item.Title})", ipAddress);
            return true;
        }

        // ==================== 3. CONTACT INFORMATION ====================

        public async Task<ContactInfo> GetContactInfoAsync()
        {
            var contact = await _context.ContactInfo.OrderByDescending(c => c.ContactInfoId).FirstOrDefaultAsync();
            if (contact == null)
            {
                contact = new ContactInfo
                {
                    Address = "Cavite, Philippines",
                    ContactNumber = "+63 912 345 6789",
                    Email = "karl.luarca@example.com",
                    OfficeHours = "Monday - Friday: 9:00 AM - 6:00 PM (PHT)",
                    SocialLinks = "GitHub: github.com/luarca | LinkedIn: linkedin.com/in/karl-luarca",
                    LastModifiedDate = DateTime.Now
                };
                _context.ContactInfo.Add(contact);
                await _context.SaveChangesAsync();
            }
            return contact;
        }

        public async Task<bool> UpdateContactInfoAsync(ContactInfo model, int adminUserId, string? ipAddress)
        {
            var existing = await _context.ContactInfo.FindAsync(model.ContactInfoId);
            if (existing == null)
            {
                existing = new ContactInfo();
                _context.ContactInfo.Add(existing);
            }

            existing.Address = model.Address?.Trim();
            existing.ContactNumber = model.ContactNumber?.Trim();
            existing.Email = model.Email?.Trim();
            existing.OfficeHours = model.OfficeHours?.Trim();
            existing.SocialLinks = model.SocialLinks?.Trim();
            existing.LastModifiedDate = DateTime.Now;

            await _context.SaveChangesAsync();
            await _auditService.LogAsync(adminUserId, "ADMIN_UPDATE_CONTENT", "Updated Contact Information section", ipAddress);
            return true;
        }

        // ==================== 4. OWNER'S INFORMATION ====================

        public async Task<OwnerInfo> GetOwnerInfoAsync()
        {
            var owner = await _context.OwnerInfo.OrderByDescending(o => o.OwnerInfoId).FirstOrDefaultAsync();
            if (owner == null)
            {
                owner = new OwnerInfo
                {
                    FullName = "Karl Ivan B. Luarca",
                    TitleRole = "Full-Stack Software Engineer & Solutions Architect",
                    Bio = "Experienced software engineer specializing in ASP.NET Core, Entity Framework Core, SQL Server, and modern full-stack web architectures.",
                    ProfileImagePath = "/images/avatar.png",
                    LastModifiedDate = DateTime.Now
                };
                _context.OwnerInfo.Add(owner);
                await _context.SaveChangesAsync();
            }
            return owner;
        }

        public async Task<bool> UpdateOwnerInfoAsync(OwnerInfo model, int adminUserId, string? ipAddress)
        {
            var existing = await _context.OwnerInfo.FindAsync(model.OwnerInfoId);
            if (existing == null)
            {
                existing = new OwnerInfo();
                _context.OwnerInfo.Add(existing);
            }

            existing.FullName = model.FullName.Trim();
            existing.TitleRole = model.TitleRole?.Trim();
            existing.Bio = model.Bio?.Trim();
            existing.ProfileImagePath = model.ProfileImagePath?.Trim();
            existing.LastModifiedDate = DateTime.Now;

            await _context.SaveChangesAsync();
            await _auditService.LogAsync(adminUserId, "ADMIN_UPDATE_CONTENT", "Updated Owner's Information section", ipAddress);
            return true;
        }

        // ==================== PORTFOLIO COMPATIBILITY ====================

        public async Task<List<Education>> GetEducationAsync()
        {
            return await _context.Education
                .Where(e => e.IsActive)
                .OrderBy(e => e.DisplayOrder)
                .ToListAsync();
        }

        public async Task<List<Skill>> GetSkillsAsync()
        {
            return await _context.Skills
                .Where(s => s.IsActive)
                .OrderBy(s => s.DisplayOrder)
                .ToListAsync();
        }

        public async Task<List<Hobby>> GetHobbiesAsync()
        {
            return await _context.Hobbies
                .Where(h => h.IsActive)
                .OrderBy(h => h.DisplayOrder)
                .ToListAsync();
        }

        public async Task<List<Project>> GetProjectsAsync()
        {
            return await _context.Projects
                .Where(p => p.IsActive)
                .OrderBy(p => p.DisplayOrder)
                .ToListAsync();
        }
    }
}
