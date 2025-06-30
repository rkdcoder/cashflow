// BCrypt.Net-Next
using CashFlow.IdentityAndAccess.Domain.Services;

namespace CashFlow.IdentityAndAccess.Infrastructure.Security
{
    /// <summary>
    /// Implements hashing and verification using BCrypt algorithm.
    /// For BCrypt, enforces a 72-character maximum data length (limitation of the algorithm).
    /// </summary>
    public class BCryptHashService : IHashService
    {
        private const int BcryptMaxLength = 72;

        /// <summary>
        /// Generates a salted BCrypt hash of the provided data.
        /// </summary>
        /// <param name="data">The data to hash.</param>
        /// <returns>The BCrypt hash string.</returns>
        /// <exception cref="ArgumentNullException">Thrown if data is null.</exception>
        /// <exception cref="ArgumentException">Thrown if data exceeds 72 characters.</exception>
        public string HashData(string data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (data.Length > BcryptMaxLength)
                throw new ArgumentException($"Data must not exceed {BcryptMaxLength} characters (BCrypt algorithm limitation).");

            return BCrypt.Net.BCrypt.HashPassword(data, workFactor: 12);
        }

        /// <summary>
        /// Verifies the data against a stored BCrypt hash.
        /// </summary>
        /// <param name="data">The plain data to verify.</param>
        /// <param name="hash">The stored hash.</param>
        /// <returns>True if the data matches the hash, false otherwise.</returns>
        public bool VerifyHash(string data, string hash)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (data.Length > BcryptMaxLength)
                throw new ArgumentException($"Data must not exceed {BcryptMaxLength} characters (BCrypt algorithm limitation).");

            try
            {
                return BCrypt.Net.BCrypt.Verify(data, hash);
            }
            catch
            {
                // Hash format error, tampered/corrupted hash
                return false;
            }
        }
    }
}