using System;

namespace LuarcaDynamicPortfolio.Mvc.Models.Entities
{
    public class ContactInfo
    {
        public int ContactInfoId { get; set; }
        public string? Address { get; set; }
        public string? ContactNumber { get; set; }
        public string? Email { get; set; }
        public string? OfficeHours { get; set; }
        public string? SocialLinks { get; set; }
        public DateTime LastModifiedDate { get; set; } = DateTime.Now;
    }
}
