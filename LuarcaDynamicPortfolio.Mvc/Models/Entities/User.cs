using System;
using System.Collections.Generic;

namespace LuarcaDynamicPortfolio.Mvc.Models.Entities
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string PasswordSalt { get; set; } = string.Empty;
        public int RoleId { get; set; } = 2; // Default: 2 = User, 1 = Admin
        public string? FullName { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime? LastLogin { get; set; }

        public virtual Role? Role { get; set; }
        public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

        public bool IsAdmin => Role?.RoleName == "Admin" || RoleId == 1;
    }
}
