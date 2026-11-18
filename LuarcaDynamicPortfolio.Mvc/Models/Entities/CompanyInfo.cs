using System;
using System.ComponentModel.DataAnnotations;

namespace LuarcaDynamicPortfolio.Mvc.Models.Entities
{
    public class CompanyInfo
    {
        public int CompanyInfoId { get; set; }

        [Required(ErrorMessage = "Company Name is required.")]
        [StringLength(150, ErrorMessage = "Company Name cannot exceed 150 characters.")]
        public string CompanyName { get; set; } = string.Empty;

        [StringLength(250, ErrorMessage = "Tagline cannot exceed 250 characters.")]
        public string? Tagline { get; set; }

        [StringLength(4000, ErrorMessage = "About Description cannot exceed 4000 characters.")]
        public string? AboutDescription { get; set; }

        [StringLength(2000, ErrorMessage = "Mission cannot exceed 2000 characters.")]
        public string? Mission { get; set; }

        [StringLength(2000, ErrorMessage = "Vision cannot exceed 2000 characters.")]
        public string? Vision { get; set; }

        public DateTime LastModifiedDate { get; set; } = DateTime.Now;
    }
}
