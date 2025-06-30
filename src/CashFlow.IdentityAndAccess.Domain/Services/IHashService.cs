namespace CashFlow.IdentityAndAccess.Domain.Services
{
    /// <summary>
    /// Defines generic hashing and verification contract for sensitive data.
    /// </summary>
    public interface IHashService
    {
        /// <summary>
        /// Generates a salted hash of the provided data.
        /// </summary>
        /// <param name="data">The plain data to hash (e.g., password, secret).</param>
        /// <returns>The hash string.</returns>
        string HashData(string data);

        /// <summary>
        /// Verifies data against a stored hash.
        /// </summary>
        /// <param name="data">The plain data to verify.</param>
        /// <param name="hash">The stored hash to compare against.</param>
        /// <returns>True if the data matches the hash, false otherwise.</returns>
        bool VerifyHash(string data, string hash);
    }
}