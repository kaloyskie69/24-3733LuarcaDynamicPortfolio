namespace LuarcaDynamicPortfolio.Mvc.Models.Entities
{
    public class Hobby
    {
        public int HobbyId { get; set; }
        public string HobbyName { get; set; } = string.Empty;
        public int DisplayOrder { get; set; } = 0;
        public bool IsActive { get; set; } = true;
    }
}
