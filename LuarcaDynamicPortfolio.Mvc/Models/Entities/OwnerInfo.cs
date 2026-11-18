using System;
using System.ComponentModel.DataAnnotations;

namespace LuarcaDynamicPortfolio.Mvc.Models.Entities
{
    public class OwnerInfo
    {
        public int OwnerInfoId { get; set; }

        [Required(ErrorMessage = "Full Name is required.")]
        [StringLength(150, ErrorMessage = "Full Name cannot exceed 150 characters.")]
        public string FullName { get; set; } = string.Empty;

        [StringLength(150, ErrorMessage = "Title or Role cannot exceed 150 characters.")]
        public string? TitleRole { get; set; }

        [StringLength(4000, ErrorMessage = "Biography cannot exceed 4000 characters.")]
        public string? Bio { get; set; }

        [StringLength(300, ErrorMessage = "Profile Image Path cannot exceed 300 characters.")]
        public string? ProfileImagePath { get; set; }

        public DateTime LastModifiedDate { get; set; } = DateTime.Now;
    }
}
