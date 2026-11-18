using System;

namespace LuarcaDynamicPortfolio.Mvc.Models.Entities
{
    public class CompanyInfo
    {
        public int CompanyInfoId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string? Tagline { get; set; }
        public string? AboutDescription { get; set; }
        public string? Mission { get; set; }
        public string? Vision { get; set; }
        public DateTime LastModifiedDate { get; set; } = DateTime.Now;
    }
}
