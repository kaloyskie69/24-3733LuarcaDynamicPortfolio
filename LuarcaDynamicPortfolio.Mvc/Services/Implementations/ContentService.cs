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
            var company = await _context.CompanyInfo.AsNoTracking().OrderByDescending(c => c.CompanyInfoId).FirstOrDefaultAsync();
            if (company == null)
            {
                var newCompany = new CompanyInfo
                {
                    CompanyName = "Luarca Web Solutions",
                    Tagline = "Building High Quality Web Applications",
                    AboutDescription = "We create clean, secure, and easy-to-use web applications using ASP.NET Core MVC, Entity Framework, and SQL Server.",
                    Mission = "To build reliable, practical software that helps users and businesses get work done efficiently.",
                    Vision = "To deliver well-designed web applications that are simple to use and easy to maintain.",
                    LastModifiedDate = DateTime.Now
                };
                _context.CompanyInfo.Add(newCompany);
                await _context.SaveChangesAsync();
                return newCompany;
            }
            return company;
        }

        public async Task<bool> UpdateCompanyInfoAsync(CompanyInfo model, int adminUserId, string? ipAddress)
        {
            CompanyInfo? existing = null;
            if (model.CompanyInfoId > 0)
            {
                existing = await _context.CompanyInfo.FindAsync(model.CompanyInfoId);
            }

            // Fallback to latest record if ID was not matched to prevent duplicate insertion
            if (existing == null)
            {
                existing = await _context.CompanyInfo.OrderByDescending(c => c.CompanyInfoId).FirstOrDefaultAsync();
                if (existing == null)
                {
                    existing = new CompanyInfo();
                    _context.CompanyInfo.Add(existing);
                }
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
            var query = _context.ProductServices.AsNoTracking();
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

            item.IsActive = false;
            await _context.SaveChangesAsync();

            await _auditService.LogAsync(adminUserId, "ADMIN_UPDATE_CONTENT", $"Deactivated Product/Service #{id} ({item.Title})", ipAddress);
            return true;
        }

        // ==================== 3. CONTACT INFORMATION ====================

        public async Task<ContactInfo> GetContactInfoAsync()
        {
            var contact = await _context.ContactInfo.AsNoTracking().OrderByDescending(c => c.ContactInfoId).FirstOrDefaultAsync();
            if (contact == null)
            {
                var newContact = new ContactInfo
                {
                    Address = "Cavite, Philippines",
                    ContactNumber = "+63 912 345 6789",
                    Email = "karl.luarca@example.com",
                    OfficeHours = "Monday - Friday: 9:00 AM - 6:00 PM",
                    SocialLinks = "GitHub: github.com/luarca | LinkedIn: linkedin.com/in/karl-luarca",
                    LastModifiedDate = DateTime.Now
                };
                _context.ContactInfo.Add(newContact);
                await _context.SaveChangesAsync();
                return newContact;
            }
            return contact;
        }

        public async Task<bool> UpdateContactInfoAsync(ContactInfo model, int adminUserId, string? ipAddress)
        {
            ContactInfo? existing = null;
            if (model.ContactInfoId > 0)
            {
                existing = await _context.ContactInfo.FindAsync(model.ContactInfoId);
            }

            // Fallback to latest record if ID was not matched to prevent duplicate insertion
            if (existing == null)
            {
                existing = await _context.ContactInfo.OrderByDescending(c => c.ContactInfoId).FirstOrDefaultAsync();
                if (existing == null)
                {
                    existing = new ContactInfo();
                    _context.ContactInfo.Add(existing);
                }
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
            var owner = await _context.OwnerInfo.AsNoTracking().OrderByDescending(o => o.OwnerInfoId).FirstOrDefaultAsync();
            if (owner == null)
            {
                var newOwner = new OwnerInfo
                {
                    FullName = "Karl Ivan B. Luarca",
                    TitleRole = "Full-Stack Web Developer",
                    Bio = "A dedicated web developer with experience building applications using C#, ASP.NET Core MVC, and SQL Server.",
                    ProfileImagePath = "/images/avatar.png",
                    LastModifiedDate = DateTime.Now
                };
                _context.OwnerInfo.Add(newOwner);
                await _context.SaveChangesAsync();
                return newOwner;
            }
            return owner;
        }

        public async Task<bool> UpdateOwnerInfoAsync(OwnerInfo model, int adminUserId, string? ipAddress)
        {
            OwnerInfo? existing = null;
            if (model.OwnerInfoId > 0)
            {
                existing = await _context.OwnerInfo.FindAsync(model.OwnerInfoId);
            }

            // Fallback to latest record if ID was not matched to prevent duplicate insertion
            if (existing == null)
            {
                existing = await _context.OwnerInfo.OrderByDescending(o => o.OwnerInfoId).FirstOrDefaultAsync();
                if (existing == null)
                {
                    existing = new OwnerInfo();
                    _context.OwnerInfo.Add(existing);
                }
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
                .AsNoTracking()
                .Where(e => e.IsActive)
                .OrderBy(e => e.DisplayOrder)
                .ToListAsync();
        }

        public async Task<List<Skill>> GetSkillsAsync()
        {
            return await _context.Skills
                .AsNoTracking()
                .Where(s => s.IsActive)
                .OrderBy(s => s.DisplayOrder)
                .ToListAsync();
        }

        public async Task<List<Hobby>> GetHobbiesAsync()
        {
            return await _context.Hobbies
                .AsNoTracking()
                .Where(h => h.IsActive)
                .OrderBy(h => h.DisplayOrder)
                .ToListAsync();
        }

        public async Task<List<Project>> GetProjectsAsync()
        {
            return await _context.Projects
                .AsNoTracking()
                .Where(p => p.IsActive)
                .OrderBy(p => p.DisplayOrder)
                .ToListAsync();
        }
    }
}
