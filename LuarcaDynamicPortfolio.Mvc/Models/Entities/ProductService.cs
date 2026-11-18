namespace LuarcaDynamicPortfolio.Mvc.Models.Entities
{
    public class ProductService
    {
        public int ProductServiceId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = "Service"; // Product or Service
        public string? Tags { get; set; } // e.g., "Web App, C#, Cloud"
        public string? ActionUrl { get; set; }
        public int DisplayOrder { get; set; } = 0;
        public bool IsActive { get; set; } = true;
    }
}
