namespace ERService.Infrastructure.Helpers
{
    public interface IPasswordHasher
    {
        void GenerateSaltedHash(string password, out string hash, out string salt);
        bool VerifyPassword(string enteredPassword, string storedHash, string storedSalt);
    }
}