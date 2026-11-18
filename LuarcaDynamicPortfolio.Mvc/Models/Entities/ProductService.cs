using System.ComponentModel.DataAnnotations;

namespace LuarcaDynamicPortfolio.Mvc.Models.Entities
{
    public class ProductService
    {
        public int ProductServiceId { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(150, ErrorMessage = "Title cannot exceed 150 characters.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category is required.")]
        [StringLength(50, ErrorMessage = "Category cannot exceed 50 characters.")]
        public string Category { get; set; } = "Service"; // Product or Service

        [StringLength(300, ErrorMessage = "Tags cannot exceed 300 characters.")]
        public string? Tags { get; set; } // e.g., "Web App, C#, Cloud"

        [StringLength(300, ErrorMessage = "Action URL cannot exceed 300 characters.")]
        [Url(ErrorMessage = "Please enter a valid URL or leave blank.")]
        public string? ActionUrl { get; set; }

        [Range(0, 9999, ErrorMessage = "Display Order must be between 0 and 9999.")]
        public int DisplayOrder { get; set; } = 0;

        public bool IsActive { get; set; } = true;
    }
}
