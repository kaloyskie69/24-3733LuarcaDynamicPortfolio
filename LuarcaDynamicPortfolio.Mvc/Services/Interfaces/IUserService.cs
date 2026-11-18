using System.Collections.Generic;
using System.Threading.Tasks;
using LuarcaDynamicPortfolio.Mvc.Models.Entities;
using LuarcaDynamicPortfolio.Mvc.Models.ViewModels.Account;

namespace LuarcaDynamicPortfolio.Mvc.Services.Interfaces
{
    public interface IUserService
    {
        Task<User?> GetByIdAsync(int userId);
        Task<User?> GetByUsernameOrEmailAsync(string usernameOrEmail);
        Task<bool> UsernameOrEmailExistsAsync(string username, string email);
        Task<(bool Success, string? Error, User? User)> AuthenticateAsync(string usernameOrEmail, string password);
        Task<(bool Success, string? Error, User? User)> RegisterAsync(RegisterViewModel model, string? ipAddress);
        Task<bool> UpdateProfileAsync(int userId, string fullName, string email, string? ipAddress);
        Task<(bool Success, string? Error)> ChangePasswordAsync(int userId, string currentPassword, string newPassword, string? ipAddress);
        Task<List<User>> GetAllUsersAsync();
        Task<bool> SetUserActiveStatusAsync(int userId, bool isActive, int adminUserId, string? ipAddress);
        Task<bool> DeleteUserAsync(int userId, int adminUserId, string? ipAddress);
        Task<(int Total, int Active, int Inactive, int Admin, int Regular, int Signups7Days)> GetUserStatsAsync();
        Task<List<AuditLog>> GetRecentActivityAsync(int count = 20);
    }
}
