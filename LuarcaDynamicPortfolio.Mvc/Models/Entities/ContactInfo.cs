using System;
using System.ComponentModel.DataAnnotations;

namespace LuarcaDynamicPortfolio.Mvc.Models.Entities
{
    public class ContactInfo
    {
        public int ContactInfoId { get; set; }

        [StringLength(300, ErrorMessage = "Address cannot exceed 300 characters.")]
        public string? Address { get; set; }

        [StringLength(50, ErrorMessage = "Contact Number cannot exceed 50 characters.")]
        public string? ContactNumber { get; set; }

        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [StringLength(150, ErrorMessage = "Email cannot exceed 150 characters.")]
        public string? Email { get; set; }

        [StringLength(150, ErrorMessage = "Office Hours cannot exceed 150 characters.")]
        public string? OfficeHours { get; set; }

        [StringLength(300, ErrorMessage = "Social Links cannot exceed 300 characters.")]
        public string? SocialLinks { get; set; }

        public DateTime LastModifiedDate { get; set; } = DateTime.Now;
    }
}
