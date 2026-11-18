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
            // Ensure tables for rubric requirements exist
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

            // 2. Company Info (Requirement: About the Business/Company)
            if (!await context.CompanyInfo.AnyAsync())
            {
                context.CompanyInfo.Add(new CompanyInfo
                {
                    CompanyName = "Luarca Web Solutions",
                    Tagline = "Building High Quality Web Applications",
                    AboutDescription = "We create clean, secure, and easy-to-use web applications using ASP.NET Core MVC, Entity Framework, and SQL Server.",
                    Mission = "To build reliable, practical software that helps users and businesses get work done efficiently.",
                    Vision = "To deliver well-designed web applications that are simple to use and easy to maintain.",
                    LastModifiedDate = DateTime.Now
                });
                await context.SaveChangesAsync();
            }

            // 3. Owner's Info (Requirement: Owner's Information)
            if (!await context.OwnerInfo.AnyAsync())
            {
                context.OwnerInfo.Add(new OwnerInfo
                {
                    FullName = personal?.FullName ?? "Karl Ivan B. Luarca",
                    TitleRole = "Full-Stack Web Developer",
                    Bio = personal?.Bio ?? "A dedicated web developer with experience building applications using C#, ASP.NET Core MVC, and SQL Server.",
                    ProfileImagePath = personal?.ProfileImagePath ?? "/images/avatar.png",
                    LastModifiedDate = DateTime.Now
                });
                await context.SaveChangesAsync();
            }

            // 4. Contact Info (Requirement: Contact Information)
            if (!await context.ContactInfo.AnyAsync())
            {
                context.ContactInfo.Add(new ContactInfo
                {
                    Address = personal?.Address ?? "Cavite, Philippines",
                    ContactNumber = personal?.ContactNumber ?? "+63 912 345 6789",
                    Email = personal?.Email ?? "karl.luarca@example.com",
                    OfficeHours = "Monday - Friday: 9:00 AM - 6:00 PM",
                    SocialLinks = "GitHub: github.com/luarca | LinkedIn: linkedin.com/in/karl-luarca",
                    LastModifiedDate = DateTime.Now
                });
                await context.SaveChangesAsync();
            }

            // 5. Products and Services (Requirement: Product and Services)
            if (!await context.ProductServices.AnyAsync())
            {
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
                            Category = "Project",
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
                            Title = "Web Application Development",
                            Description = "Custom web applications built with user login, database management, and responsive design.",
                            Category = "Service",
                            Tags = "ASP.NET Core, C#, SQL Server",
                            DisplayOrder = 1,
                            IsActive = true
                        },
                        new ProductService
                        {
                            Title = "Database Design & Setup",
                            Description = "Database modeling, tables, queries, and Entity Framework data access.",
                            Category = "Service",
                            Tags = "SQL Server, Entity Framework",
                            DisplayOrder = 2,
                            IsActive = true
                        },
                        new ProductService
                        {
                            Title = "Task Manager App",
                            Description = "An app for organizing and tracking daily tasks with deadlines and priorities.",
                            Category = "Product",
                            Tags = "Web App, C#, SQL",
                            DisplayOrder = 3,
                            IsActive = true
                        },
                        new ProductService
                        {
                            Title = "Budget Tracker",
                            Description = "A dashboard to help users monitor income, expenses, and savings.",
                            Category = "Product",
                            Tags = "Dashboard, Reports",
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
                // Continue if already exists or offline
            }
        }
    }
}
