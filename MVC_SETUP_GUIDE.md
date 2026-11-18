# ASP.NET Core MVC + Entity Framework Core + MS SQL Project
## Karl Ivan Luarca — Personal & Business Dynamic Website

This web application is built with **ASP.NET Core MVC (.NET 8.0)**, **Entity Framework Core**, and **MS SQL Server**, styled with a clean, high-contrast black-and-white theme.

---

## 1. Quick Start

### Running in Visual Studio 2022
1. Open **`LuarcaDynamicPortfolio.sln`** in Visual Studio 2022.
2. Check your database connection string in `LuarcaDynamicPortfolio.Mvc/appsettings.json`:
   ```json
   "ConnectionStrings": {
     "PersonalWebsiteDB": "Data Source=.;Initial Catalog=PersonalWebsiteDB;Integrated Security=True;TrustServerCertificate=True;"
   }
   ```
3. Press **F5** (or Ctrl+F5) to run the application.

### Running from the Terminal
```powershell
dotnet run --project LuarcaDynamicPortfolio.Mvc
```
Open your browser to the local URL shown in the terminal (e.g. `https://localhost:7198`).

---

## 2. Key Features

| Requirement | Description | How to Test |
|---|---|---|
| **User Sign-In** | Users must sign in first before viewing the website. | Visit the home page while signed out; you will be redirected to the Sign In page. |
| **User Sign-Up** | New users can create an account if one does not exist. | Click "Sign up here" on the Sign In page and fill out the registration form. |
| **Modify Profile** | Users can update their full name, email, and password. | Click your name in the top navbar and select "My Profile". |
| **View-Only User List (Admin)** | Admins can view all registered users. | Go to Admin Panel &rarr; Users tab. |
| **Activate / Deactivate Users** | Admins can turn accounts on or off, or delete them. | Use the "Activate", "Deactivate", or "Delete" buttons in the Users tab. |
| **Statistics** | Admins can view user counts, signup numbers, and recent activity. | Go to Admin Panel &rarr; Statistics tab. |
| **Dynamic Website Content** | Admins can edit company info, products/services, contact details, and owner info. | Go to Admin Panel &rarr; Edit Website Content tab. |

---

## 3. Dynamic Website Content Sections

- **Section A: About the Business / Company** &mdash; Displays company name, tagline, description, mission, and vision.
- **Section B: Products and Services** &mdash; Displays active products and services with categories and tags.
- **Section C: Contact Information** &mdash; Displays address, phone number, email, and office hours.
- **Section D: Owner's Information** &mdash; Displays the owner's name, role, biography, education, and skills.
