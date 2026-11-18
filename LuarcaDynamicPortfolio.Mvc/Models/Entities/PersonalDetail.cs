using System;

namespace LuarcaDynamicPortfolio.Mvc.Models.Entities
{
    public class PersonalDetail
    {
        public int PersonalDetailId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? ContactNumber { get; set; }
        public string? Email { get; set; }
        public string? Bio { get; set; }
        public string? ProfileImagePath { get; set; }
        public int? LastModifiedBy { get; set; }
        public DateTime LastModifiedDate { get; set; } = DateTime.Now;

        public virtual User? LastModifiedByUser { get; set; }
    }
}
