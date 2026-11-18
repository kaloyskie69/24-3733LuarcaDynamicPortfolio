using System.Collections.Generic;
using LuarcaDynamicPortfolio.Mvc.Models.Entities;

namespace LuarcaDynamicPortfolio.Mvc.Models.ViewModels.Admin
{
    public class AdminDashboardViewModel
    {
        // Statistics
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int InactiveUsers { get; set; }
        public int AdminCount { get; set; }
        public int RegularUserCount { get; set; }
        public int SignupsLast7Days { get; set; }

        // Management Lists
        public List<User> Users { get; set; } = new List<User>();
        public List<AuditLog> RecentActivity { get; set; } = new List<AuditLog>();

        // Dynamic Content Objects (4 Rubric Sections)
        public CompanyInfo Company { get; set; } = new CompanyInfo();
        public List<ProductService> ProductsServices { get; set; } = new List<ProductService>();
        public ContactInfo Contact { get; set; } = new ContactInfo();
        public OwnerInfo Owner { get; set; } = new OwnerInfo();
    }
}
