# ASP.NET Core MVC + Entity Framework Core + MS SQL Project
## Dynamic Portfolio & Solutions Web Application

This project has been upgraded to **ASP.NET Core MVC** with **Entity Framework Core (EF Core)** and **MS SQL Server**, satisfying all requirements specified in the project rubric.

---

## 1. Quick Start

### Option A: Using Visual Studio 2022
1. Double-click **`LuarcaDynamicPortfolio.sln`** in this folder to open the project in Visual Studio.
2. Ensure your local SQL Server instance is running (`PersonalWebsiteDB` created).
3. Check the connection string in `LuarcaDynamicPortfolio.Mvc/appsettings.json`:
   ```json
   "ConnectionStrings": {
     "PersonalWebsiteDB": "Data Source=.;Initial Catalog=PersonalWebsiteDB;Integrated Security=True;TrustServerCertificate=True;"
   }
   ```
4. Press **F5** (or Ctrl+F5) to launch the web application.

### Option B: Using .NET CLI / Terminal
Open PowerShell in this directory and run:
```powershell
dotnet run --project LuarcaDynamicPortfolio.Mvc
```
Open your browser to the URL displayed in the terminal (typically `https://localhost:7198` or `http://localhost:5242`).

---

## 2. Rubric Compliance & Feature Verification

| Rubric Requirement | Implementation Detail | Location |
|---|---|---|
| **a. User: Sign-In first before viewing website** | Enforced via `[Authorize]` filter on `HomeController`. Unauthenticated visitors are automatically redirected to the Sign-In page. | `HomeController.cs`, `Views/Account/Login.cshtml` |
| **b. User: Sign-Up if not in database** | Self-service registration with validation and PBKDF2 password hashing. New users default to Role = `User`. | `AccountController.cs`, `Views/Account/Register.cshtml` |
| **c. User: Modify his/her own information** | Dedicated profile management allowing users to update Full Name, Email, and Change Password with live feedback. | `ProfileController.cs`, `Views/Profile/Index.cshtml` |
| **d. Admin: View-only users information** | Admin dashboard displays a clean, read-only table of all user accounts (ID, Username, Full Name, Email, Role, Registration Date). | `AdminController.cs`, `Views/Admin/Index.cshtml` |
| **e. Admin: Remove users or tag as inactive** | Inactive toggle button deactivates account (blocks login); Delete button removes account with safety checks. | `AdminController.ToggleUserStatus`, `AdminController.DeleteUser` |
| **f. Admin: View statistic reports** | Cards displaying Total Users, Active, Inactive, Admins, Regular Users, and 7-Day Signups, plus Recent Audit Logs. | `AdminController.Index` (Statistic Reports tab) |
| **g. Admin: Modify website content** | Full management forms to update Company Info, Products/Services, Contact Details, and Owner's Info. | `AdminController.Index` (Content Editor tab) |
| **Website Content a: About Business/Company** | Dynamic company name, tagline, description, mission, and vision rendered from MS SQL. | `Views/Home/Index.cshtml` (Section A) |
| **Website Content b: Products and Services** | Dynamic catalog of products and services with category badges, tech tags, and order controls. | `Views/Home/Index.cshtml` (Section B) |
| **Website Content c: Contact Information** | Dynamic address, phone, email, office hours, social links, and direct inquiry link. | `Views/Home/Index.cshtml` (Section C) |
| **Website Content d: Owner's Information** | Dynamic owner name, professional role, biography, credentials, and skills. | `Views/Home/Index.cshtml` (Section D) |

---

## 3. Technology Stack

- **Runtime & Framework:** ASP.NET Core MVC (.NET 8.0)
- **Data Access:** Entity Framework Core (`Microsoft.EntityFrameworkCore.SqlServer` 8.0.11)
- **Database Engine:** Microsoft SQL Server (`PersonalWebsiteDB`)
- **Authentication:** Cookie Authentication with Claims (`ClaimsPrincipal`, `[Authorize]`, `[Authorize(Roles = "Admin")]`)
- **Security:** PBKDF2 with SHA-256 and 100,000 iterations (100% backward compatible with existing user password hashes)
- **Styling:** Bootstrap 5.3 Dark Mode + Custom responsive CSS
