using System.Threading.Tasks;

namespace LuarcaDynamicPortfolio.Mvc.Services.Interfaces
{
    public interface IAuditService
    {
        Task LogAsync(int? userId, string actionType, string? actionDetail, string? ipAddress);
    }
}
