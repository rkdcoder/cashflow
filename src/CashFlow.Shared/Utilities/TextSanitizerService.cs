using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace CashFlow.Shared.Utilities
{
    /// <summary>
    /// Provides methods to sanitize and normalize text strings, removing diacritics, special characters, duplicate spaces, and trimming whitespace.
    /// Keeps only letters, digits, and a few basic symbols (- . / @ &). Always returns the result in upper case.
    /// </summary>
    public class TextSanitizerService : ITextSanitizerService
    {
        /// <inheritdoc />
        public string Normalize(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // Remove diacritics and unicode marks
            string normalized = input.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (char c in normalized)
            {
                var category = CharUnicodeInfo.GetUnicodeCategory(c);
                if (category != UnicodeCategory.NonSpacingMark)
                {
                    // Allow only letters, digits, selected symbols and whitespace
                    if (char.IsLetterOrDigit(c) || c == '-' || c == '.' || c == '/' || c == '@' || c == '&' || char.IsWhiteSpace(c))
                        sb.Append(c);
                }
            }

            string result = sb.ToString();

            // Remove any remaining unwanted special characters (except the allowed ones)
            result = Regex.Replace(result, @"[^A-Z0-9\-\.\/@&\s]", "", RegexOptions.IgnoreCase);

            // Collapse duplicate spaces into a single space
            result = Regex.Replace(result, @"\s+", " ");

            // Trim leading and trailing whitespace
            result = result.Trim();

            // Always return upper case
            return result.ToUpperInvariant();
        }
    }
}