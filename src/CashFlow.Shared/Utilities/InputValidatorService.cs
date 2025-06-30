using CashFlow.Shared.Dtos;
using System.Text.RegularExpressions;

namespace CashFlow.Shared.Utilities
{
    /// <summary>
    /// Service for validating user input (email, login, username, password) with standard rules.
    /// </summary>
    public class InputValidatorService : IInputValidatorService
    {
        /// <inheritdoc />
        public ValidationResultDto ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return ValidationResultDto.FailureResult("E-mail is required.");

            var trimmed = email.Trim();

            // Email regex from Microsoft docs (.NET official)
            const string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(trimmed, pattern, RegexOptions.IgnoreCase))
                return ValidationResultDto.FailureResult("Invalid email format.");

            if (trimmed.Length > 360)
                return ValidationResultDto.FailureResult("E-mail must be at most 360 characters.");

            return ValidationResultDto.SuccessResult();
        }

        /// <inheritdoc />
        public ValidationResultDto ValidateLoginName(string loginName)
        {
            if (string.IsNullOrWhiteSpace(loginName))
                return ValidationResultDto.FailureResult("Login name is required.");

            var trimmed = loginName.Trim();

            if (trimmed.Length < 3)
                return ValidationResultDto.FailureResult("Login name must have at least 3 characters.");

            if (trimmed.Contains('@'))
                return ValidationResultDto.FailureResult("Login name must not contain the '@' character.");

            if (!Regex.IsMatch(trimmed, @"^[A-Za-z]+$"))
                return ValidationResultDto.FailureResult("Login name must contain only letters (A-Z, a-z).");

            return ValidationResultDto.SuccessResult();
        }

        /// <inheritdoc />
        public ValidationResultDto ValidateUserName(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return ValidationResultDto.FailureResult("User name is required.");

            var trimmed = userName.Trim();

            if (trimmed.Length < 3)
                return ValidationResultDto.FailureResult("User name must have at least 3 characters.");

            if (!Regex.IsMatch(trimmed, @"^[A-Za-z]+(?: [A-Za-z]+)*$"))
                return ValidationResultDto.FailureResult("User name must contain only letters (A-Z, a-z) and spaces.");

            return ValidationResultDto.SuccessResult();
        }

        /// <inheritdoc />
        public ValidationResultDto ValidatePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return ValidationResultDto.FailureResult("Password is required.");

            if (password.Length < 8 || password.Length > 72)
                return ValidationResultDto.FailureResult("Password must be between 8 and 72 characters.");

            if (!Regex.IsMatch(password, @"[A-Z]"))
                return ValidationResultDto.FailureResult("Password must contain at least one uppercase letter.");

            if (!Regex.IsMatch(password, @"[a-z]"))
                return ValidationResultDto.FailureResult("Password must contain at least one lowercase letter.");

            if (!Regex.IsMatch(password, @"[0-9]"))
                return ValidationResultDto.FailureResult("Password must contain at least one numerical digit.");

            return ValidationResultDto.SuccessResult();
        }
    }
}