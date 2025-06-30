namespace CashFlow.IdentityAndAccess.Application.Users.Dto
{
    public class LoginRequestDto
    {
        public string LoginOrEmail { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}