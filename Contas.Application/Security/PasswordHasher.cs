using System.Security.Cryptography;
using System.Text;

namespace Contas.Application.Security
{
    public static class PasswordHasher
    {
        public static (string Hash, string Salt) Hash(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new Exception("Senha inválida.");

            var saltBytes = RandomNumberGenerator.GetBytes(16);
            var salt = Convert.ToBase64String(saltBytes);

            var combined = password + salt;
            var combinedBytes = Encoding.UTF8.GetBytes(combined);

            using var sha = SHA256.Create();
            var hashBytes = sha.ComputeHash(combinedBytes);
            var hash = Convert.ToBase64String(hashBytes);

            return (hash, salt);
        }

        public static bool Verify(string password, string hash, string salt)
        {
            var combined = password + salt;
            var combinedBytes = Encoding.UTF8.GetBytes(combined);

            using var sha = SHA256.Create();
            var newHashBytes = sha.ComputeHash(combinedBytes);
            var newHash = Convert.ToBase64String(newHashBytes);

            return newHash == hash;
        }
    }
}
