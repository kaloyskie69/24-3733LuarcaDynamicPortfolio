using System;

namespace LuarcaDynamicPortfolio.Mvc.Models.Entities
{
    public class AuditLog
    {
        public int AuditLogId { get; set; }
        public int? UserId { get; set; }
        public string ActionType { get; set; } = string.Empty;
        public string? ActionDetail { get; set; }
        public DateTime ActionDate { get; set; } = DateTime.Now;
        public string? IPAddress { get; set; }

        public virtual User? User { get; set; }
    }
}
