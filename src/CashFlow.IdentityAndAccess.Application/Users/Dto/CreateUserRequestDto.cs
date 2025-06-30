namespace CashFlow.IdentityAndAccess.Application.Users.Dto
{
    public class CreateUserRequestDto
    {
        public string LoginName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
    }
}