using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LuarcaDynamicPortfolio.Mvc.Models.Entities;

namespace LuarcaDynamicPortfolio.Mvc.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(PersonalWebsiteDbContext context)
        {
            // Ensure tables for new rubric requirements exist if they were not in the original 01_Schema script
            await CreateRubricTablesIfNotExistAsync(context);

            // 1. Roles
            if (!await context.Roles.AnyAsync())
            {
                context.Roles.AddRange(
                    new Role { RoleId = 1, RoleName = "Admin" },
                    new Role { RoleId = 2, RoleName = "User" }
                );
                await context.SaveChangesAsync();
            }

            // Read existing PersonalDetails if available
            var personal = await context.PersonalDetails.FirstOrDefaultAsync();

            // 2. Company Info (Rubric Requirement: About the Business/Company)
            if (!await context.CompanyInfo.AnyAsync())
            {
                context.CompanyInfo.Add(new CompanyInfo
                {
                    CompanyName = "Luarca Web Solutions & Technologies",
                    Tagline = "Building Modern, Scalable, and Dynamic Enterprise Web Applications",
                    AboutDescription = "Luarca Web Solutions is a technology firm dedicated to architecting clean, secure, and data-driven web platforms using modern ASP.NET Core, Entity Framework, and MS SQL Server technologies.",
                    Mission = "To provide high-quality, maintainable, and robust digital solutions that solve real-world problems and empower businesses through technology.",
                    Vision = "To be recognized as a premier software development partner trusted for performance, modern engineering, and clean architecture.",
                    LastModifiedDate = DateTime.Now
                });
                await context.SaveChangesAsync();
            }

            // 3. Owner's Info (Rubric Requirement: Owner's Information)
            if (!await context.OwnerInfo.AnyAsync())
            {
                context.OwnerInfo.Add(new OwnerInfo
                {
                    FullName = personal?.FullName ?? "Karl Ivan B. Luarca",
                    TitleRole = "Full-Stack Software Engineer & Solution Architect",
                    Bio = personal?.Bio ?? "Passionate software engineer experienced in building robust web applications using C#, ASP.NET MVC, Entity Framework Core, MS SQL Server, and modern web design.",
                    ProfileImagePath = personal?.ProfileImagePath ?? "/images/avatar.png",
                    LastModifiedDate = DateTime.Now
                });
                await context.SaveChangesAsync();
            }

            // 4. Contact Info (Rubric Requirement: Contact Information)
            if (!await context.ContactInfo.AnyAsync())
            {
                context.ContactInfo.Add(new ContactInfo
                {
                    Address = personal?.Address ?? "Cavite, Philippines",
                    ContactNumber = personal?.ContactNumber ?? "+63 912 345 6789",
                    Email = personal?.Email ?? "karl.luarca@example.com",
                    OfficeHours = "Monday - Friday: 9:00 AM - 6:00 PM (PHT)",
                    SocialLinks = "GitHub: github.com/luarca | LinkedIn: linkedin.com/in/karl-luarca",
                    LastModifiedDate = DateTime.Now
                });
                await context.SaveChangesAsync();
            }

            // 5. Products and Services (Rubric Requirement: Product and Services)
            if (!await context.ProductServices.AnyAsync())
            {
                // Also check if there are projects to port over
                var existingProjects = await context.Projects.ToListAsync();
                if (existingProjects.Any())
                {
                    int order = 1;
                    foreach (var proj in existingProjects)
                    {
                        context.ProductServices.Add(new ProductService
                        {
                            Title = proj.Title,
                            Description = proj.Description,
                            Category = "Software Solution",
                            Tags = proj.TechTags,
                            DisplayOrder = order++,
                            IsActive = proj.IsActive
                        });
                    }
                }
                else
                {
                    context.ProductServices.AddRange(
                        new ProductService
                        {
                            Title = "Custom Web Application Development",
                            Description = "End-to-end bespoke web systems built on ASP.NET Core MVC with secure role-based access control and high-performance MS SQL data layers.",
                            Category = "Service",
                            Tags = "ASP.NET Core, C#, MS SQL Server, MVC",
                            DisplayOrder = 1,
                            IsActive = true
                        },
                        new ProductService
                        {
                            Title = "Database Architecture & Optimization",
                            Description = "Relational database modeling, query tuning, indexing, and Entity Framework Core ORM mapping for maximum throughput.",
                            Category = "Service",
                            Tags = "MS SQL Server, EF Core, Database Design",
                            DisplayOrder = 2,
                            IsActive = true
                        },
                        new ProductService
                        {
                            Title = "Enterprise Task Manager System",
                            Description = "A production-grade task management and workflow tracking platform with real-time state management and audit logging.",
                            Category = "Product",
                            Tags = "C#, Web Application, Workflow",
                            DisplayOrder = 3,
                            IsActive = true
                        },
                        new ProductService
                        {
                            Title = "Financial Budget Tracker Dashboard",
                            Description = "Interactive financial tracking and expenditure analytics portal with interactive charts and reporting.",
                            Category = "Product",
                            Tags = "Dashboard, Analytics, Charts",
                            DisplayOrder = 4,
                            IsActive = true
                        }
                    );
                }
                await context.SaveChangesAsync();
            }
        }

        private static async Task CreateRubricTablesIfNotExistAsync(PersonalWebsiteDbContext context)
        {
            var sql = @"
IF OBJECT_ID('dbo.CompanyInfo', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.CompanyInfo
    (
        CompanyInfoId    INT IDENTITY(1,1) PRIMARY KEY,
        CompanyName      NVARCHAR(150) NOT NULL,
        Tagline          NVARCHAR(250) NULL,
        AboutDescription NVARCHAR(MAX) NULL,
        Mission          NVARCHAR(MAX) NULL,
        Vision           NVARCHAR(MAX) NULL,
        LastModifiedDate DATETIME NOT NULL DEFAULT GETDATE()
    );
END

IF OBJECT_ID('dbo.ProductServices', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.ProductServices
    (
        ProductServiceId INT IDENTITY(1,1) PRIMARY KEY,
        Title            NVARCHAR(150) NOT NULL,
        Description      NVARCHAR(500) NOT NULL,
        Category         NVARCHAR(50)  NOT NULL DEFAULT 'Service',
        Tags             NVARCHAR(300) NULL,
        ActionUrl        NVARCHAR(300) NULL,
        DisplayOrder     INT NOT NULL DEFAULT 0,
        IsActive         BIT NOT NULL DEFAULT 1
    );
END

IF OBJECT_ID('dbo.ContactInfo', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.ContactInfo
    (
        ContactInfoId    INT IDENTITY(1,1) PRIMARY KEY,
        Address          NVARCHAR(300) NULL,
        ContactNumber    NVARCHAR(50)  NULL,
        Email            NVARCHAR(150) NULL,
        OfficeHours      NVARCHAR(150) NULL,
        SocialLinks      NVARCHAR(300) NULL,
        LastModifiedDate DATETIME NOT NULL DEFAULT GETDATE()
    );
END

IF OBJECT_ID('dbo.OwnerInfo', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.OwnerInfo
    (
        OwnerInfoId      INT IDENTITY(1,1) PRIMARY KEY,
        FullName         NVARCHAR(150) NOT NULL,
        TitleRole        NVARCHAR(150) NULL,
        Bio              NVARCHAR(MAX) NULL,
        ProfileImagePath NVARCHAR(300) NULL,
        LastModifiedDate DATETIME NOT NULL DEFAULT GETDATE()
    );
END
";
            try
            {
                await context.Database.ExecuteSqlRawAsync(sql);
            }
            catch
            {
                // If tables already exist or db connection is in memory/offline, continue gracefully
            }
        }
    }
}
