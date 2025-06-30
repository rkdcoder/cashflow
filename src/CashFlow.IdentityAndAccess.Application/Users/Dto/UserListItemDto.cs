namespace CashFlow.IdentityAndAccess.Application.Users.Dto
{
    public class UserListItemDto
    {
        public Guid Id { get; set; }
        public string LoginName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool Enabled { get; set; }
        public List<string> Roles { get; set; } = new();
    }
}