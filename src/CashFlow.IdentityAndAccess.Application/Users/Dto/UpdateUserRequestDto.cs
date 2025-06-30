namespace CashFlow.IdentityAndAccess.Application.Users.Dto
{
    public class UpdateUserRequestDto
    {
        public string LoginOrEmail { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
        public bool Enabled { get; set; }
    }
}