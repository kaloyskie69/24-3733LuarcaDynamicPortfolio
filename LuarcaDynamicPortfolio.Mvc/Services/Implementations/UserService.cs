using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LuarcaDynamicPortfolio.Mvc.Data;
using LuarcaDynamicPortfolio.Mvc.Models.Entities;
using LuarcaDynamicPortfolio.Mvc.Models.ViewModels.Account;
using LuarcaDynamicPortfolio.Mvc.Services.Interfaces;

namespace LuarcaDynamicPortfolio.Mvc.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly PersonalWebsiteDbContext _context;
        private readonly IPasswordHasherService _passwordHasher;
        private readonly IAuditService _auditService;

        public UserService(
            PersonalWebsiteDbContext context,
            IPasswordHasherService passwordHasher,
            IAuditService auditService)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _auditService = auditService;
        }

        public async Task<User?> GetByIdAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<User?> GetByUsernameOrEmailAsync(string usernameOrEmail)
        {
            var trimmed = usernameOrEmail.Trim();
            return await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Username == trimmed || u.Email == trimmed);
        }

        public async Task<bool> UsernameOrEmailExistsAsync(string username, string email)
        {
            var u = username.Trim();
            var e = email.Trim();
            return await _context.Users
                .AsNoTracking()
                .AnyAsync(x => x.Username == u || x.Email == e);
        }

        public async Task<(bool Success, string? Error, User? User)> AuthenticateAsync(string usernameOrEmail, string password)
        {
            var user = await GetByUsernameOrEmailAsync(usernameOrEmail);
            if (user == null)
            {
                return (false, "Invalid username or password.", null);
            }

            if (!user.IsActive)
            {
                return (false, "Your account has been deactivated. Please contact an administrator.", null);
            }

            bool isValid = _passwordHasher.VerifyPassword(password, user.PasswordHash, user.PasswordSalt);
            if (!isValid)
            {
                return (false, "Invalid username or password.", null);
            }

            // Update LastLogin timestamp
            user.LastLogin = DateTime.Now;
            await _context.SaveChangesAsync();

            return (true, null, user);
        }

        public async Task<(bool Success, string? Error, User? User)> RegisterAsync(RegisterViewModel model, string? ipAddress)
        {
            var u = model.Username.Trim();
            var e = model.Email.Trim();

            if (await _context.Users.AsNoTracking().AnyAsync(x => x.Username == u))
            {
                return (false, "The chosen username is already taken.", null);
            }

            if (await _context.Users.AsNoTracking().AnyAsync(x => x.Email == e))
            {
                return (false, "An account with this email address already exists.", null);
            }

            _passwordHasher.CreatePasswordHash(model.Password, out string hash, out string salt);

            var user = new User
            {
                Username = u,
                Email = e,
                FullName = model.FullName.Trim(),
                PasswordHash = hash,
                PasswordSalt = salt,
                RoleId = 2, // Regular User role
                IsActive = true,
                DateCreated = DateTime.Now
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            await _auditService.LogAsync(user.UserId, "SIGNUP", $"User registered as '{user.Username}'", ipAddress);

            return (true, null, user);
        }

        public async Task<bool> UpdateProfileAsync(int userId, string fullName, string email, string? ipAddress)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            var trimmedEmail = email.Trim();
            var emailExists = await _context.Users.AsNoTracking().AnyAsync(u => u.Email == trimmedEmail && u.UserId != userId);
            if (emailExists) return false;

            user.FullName = fullName.Trim();
            user.Email = trimmedEmail;

            await _context.SaveChangesAsync();
            await _auditService.LogAsync(userId, "UPDATE_PROFILE", "User updated personal profile information", ipAddress);

            return true;
        }

        public async Task<(bool Success, string? Error)> ChangePasswordAsync(int userId, string currentPassword, string newPassword, string? ipAddress)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return (false, "User not found.");

            if (!_passwordHasher.VerifyPassword(currentPassword, user.PasswordHash, user.PasswordSalt))
            {
                return (false, "Current password does not match.");
            }

            _passwordHasher.CreatePasswordHash(newPassword, out string newHash, out string newSalt);
            user.PasswordHash = newHash;
            user.PasswordSalt = newSalt;

            await _context.SaveChangesAsync();
            await _auditService.LogAsync(userId, "CHANGE_PASSWORD", "User successfully changed password", ipAddress);

            return (true, null);
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _context.Users
                .AsNoTracking()
                .Include(u => u.Role)
                .OrderByDescending(u => u.DateCreated)
                .ToListAsync();
        }

        public async Task<bool> SetUserActiveStatusAsync(int userId, bool isActive, int adminUserId, string? ipAddress)
        {
            // Safety: Admin cannot deactivate their own account
            if (userId == adminUserId && !isActive)
            {
                return false;
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            user.IsActive = isActive;
            await _context.SaveChangesAsync();

            await _auditService.LogAsync(adminUserId, "ADMIN_TOGGLE_STATUS",
                $"Admin set user #{userId} ({user.Username}) to {(isActive ? "Active" : "Inactive")}", ipAddress);

            return true;
        }

        public async Task<bool> DeleteUserAsync(int userId, int adminUserId, string? ipAddress)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            // Safety: Admin cannot delete their own account
            if (userId == adminUserId) return false;

            // Resolve foreign keys before deletion to prevent SQL reference constraint exceptions
            // 1. Delete associated audit logs (same as sp_Admin_DeleteUser)
            var logs = await _context.AuditLogs.Where(a => a.UserId == userId).ToListAsync();
            _context.AuditLogs.RemoveRange(logs);

            // 2. Nullify any PersonalDetails modified by this user
            var modifiedDetails = await _context.PersonalDetails.Where(p => p.LastModifiedBy == userId).ToListAsync();
            foreach (var pd in modifiedDetails)
            {
                pd.LastModifiedBy = null;
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            await _auditService.LogAsync(adminUserId, "ADMIN_DELETE_USER",
                $"Admin deleted user #{userId} ({user.Username})", ipAddress);

            return true;
        }

        public async Task<(int Total, int Active, int Inactive, int Admin, int Regular, int Signups7Days)> GetUserStatsAsync()
        {
            var total = await _context.Users.AsNoTracking().CountAsync();
            var active = await _context.Users.AsNoTracking().CountAsync(u => u.IsActive);
            var inactive = total - active;
            var admin = await _context.Users.AsNoTracking().CountAsync(u => u.RoleId == 1);
            var regular = await _context.Users.AsNoTracking().CountAsync(u => u.RoleId == 2);
            var sevenDaysAgo = DateTime.Now.AddDays(-7);
            var signups7Days = await _context.Users.AsNoTracking().CountAsync(u => u.DateCreated >= sevenDaysAgo);

            return (total, active, inactive, admin, regular, signups7Days);
        }

        public async Task<List<AuditLog>> GetRecentActivityAsync(int count = 20)
        {
            return await _context.AuditLogs
                .AsNoTracking()
                .Include(a => a.User)
                .OrderByDescending(a => a.ActionDate)
                .Take(count)
                .ToListAsync();
        }
    }
}
