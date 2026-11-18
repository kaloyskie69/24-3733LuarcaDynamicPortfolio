using System.Collections.Generic;
using LuarcaDynamicPortfolio.Mvc.Models.Entities;

namespace LuarcaDynamicPortfolio.Mvc.Models.ViewModels.Home
{
    public class DynamicWebsiteViewModel
    {
        // 4 Rubric Sections
        public CompanyInfo Company { get; set; } = new CompanyInfo();
        public List<ProductService> ProductsServices { get; set; } = new List<ProductService>();
        public ContactInfo Contact { get; set; } = new ContactInfo();
        public OwnerInfo Owner { get; set; } = new OwnerInfo();

        // Portfolio compatibility sections
        public List<Project> Projects { get; set; } = new List<Project>();
        public List<Education> EducationList { get; set; } = new List<Education>();
        public List<Skill> SkillsList { get; set; } = new List<Skill>();
        public List<Hobby> HobbiesList { get; set; } = new List<Hobby>();

        public string CurrentUserName { get; set; } = string.Empty;
    }
}
