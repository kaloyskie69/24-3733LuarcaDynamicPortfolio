using System;

namespace LuarcaDynamicPortfolio.Mvc.Models.Entities
{
    public class OwnerInfo
    {
        public int OwnerInfoId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? TitleRole { get; set; }
        public string? Bio { get; set; }
        public string? ProfileImagePath { get; set; }
        public DateTime LastModifiedDate { get; set; } = DateTime.Now;
    }
}
