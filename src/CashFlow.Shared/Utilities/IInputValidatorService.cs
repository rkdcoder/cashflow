using CashFlow.Shared.Dtos;

namespace CashFlow.Shared.Utilities
{
    /// <summary>
    /// Defines contract for validating user input such as email, login, user name and password.
    /// </summary>
    public interface IInputValidatorService
    {
        ValidationResultDto ValidateEmail(string email);
        ValidationResultDto ValidateLoginName(string loginName);
        ValidationResultDto ValidateUserName(string userName);
        ValidationResultDto ValidatePassword(string password);
    }
}
