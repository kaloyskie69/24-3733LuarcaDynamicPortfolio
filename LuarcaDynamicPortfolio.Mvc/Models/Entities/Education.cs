namespace LuarcaDynamicPortfolio.Mvc.Models.Entities
{
    public class Education
    {
        public int EducationId { get; set; }
        public string Level { get; set; } = string.Empty;
        public string SchoolName { get; set; } = string.Empty;
        public string? Course { get; set; }
        public int? YearFrom { get; set; }
        public string? YearTo { get; set; }
        public int DisplayOrder { get; set; } = 0;
        public bool IsActive { get; set; } = true;
    }
}
