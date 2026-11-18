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
                var log = new AuditLog
                {
                    UserId = userId,
                    ActionType = actionType,
                    ActionDetail = actionDetail,
                    IPAddress = ipAddress,
                    ActionDate = DateTime.Now
                };

                _context.AuditLogs.Add(log);
                await _context.SaveChangesAsync();
            }
            catch
            {
                // Never allow audit logging failure to crash an operation
            }
        }
    }
}
