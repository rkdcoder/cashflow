namespace CashFlow.Shared.Utilities
{
    /// <summary>
    /// Defines a contract for sanitizing and normalizing text strings (removing accents, special characters, duplicate spaces, etc.).
    /// </summary>
    public interface ITextSanitizerService
    {
        /// <summary>
        /// Normalizes the specified input by removing diacritics, special characters, duplicate spaces and trims whitespace.
        /// Keeps only letters, digits, and a few basic symbols (- . / @ &).
        /// Always returns the result in upper case.
        /// </summary>
        /// <param name="input">The original string. Can be null.</param>
        /// <returns>The sanitized and normalized string.</returns>
        string Normalize(string? input);
    }
}