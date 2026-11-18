using System;
using System.Threading.Tasks;
using LuarcaDynamicPortfolio.Mvc.Data;
using LuarcaDynamicPortfolio.Mvc.Models.Entities;
using LuarcaDynamicPortfolio.Mvc.Services.Interfaces;

namespace LuarcaDynamicPortfolio.Mvc.Services.Implementations
{
    public class AuditService : IAuditService
    {
        private readonly PersonalWebsiteDbContext _context;

        public AuditService(PersonalWebsiteDbContext context)
        {
            _context = context;
        }

        public async Task LogAsync(int? userId, string actionType, string? actionDetail, string? ipAddress)
        {
            try
            {
                // Guard maximum column lengths to guarantee zero database truncation crashes
                var safeActionType = !string.IsNullOrEmpty(actionType)
                    ? (actionType.Length > 100 ? actionType.Substring(0, 100) : actionType)
                    : "UNKNOWN";

                var safeActionDetail = !string.IsNullOrEmpty(actionDetail)
                    ? (actionDetail.Length > 500 ? actionDetail.Substring(0, 500) : actionDetail)
                    : null;

                var safeIp = !string.IsNullOrEmpty(ipAddress)
                    ? (ipAddress.Length > 50 ? ipAddress.Substring(0, 50) : ipAddress)
                    : null;

                var log = new AuditLog
                {
                    UserId = userId,
                    ActionType = safeActionType,
                    ActionDetail = safeActionDetail,
                    IPAddress = safeIp,
                    ActionDate = DateTime.Now
                };

                _context.AuditLogs.Add(log);
                await _context.SaveChangesAsync();
            }
            catch
            {
                // Silently swallow audit logging errors to prevent secondary crashes on main flows
            }
        }
    }
}
