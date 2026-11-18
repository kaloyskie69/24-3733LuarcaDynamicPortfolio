namespace LuarcaDynamicPortfolio.Mvc.Services.Interfaces
{
    public interface IPasswordHasherService
    {
        void CreatePasswordHash(string plainPassword, out string hash, out string salt);
        bool VerifyPassword(string plainPassword, string storedHash, string storedSalt);
    }
}
