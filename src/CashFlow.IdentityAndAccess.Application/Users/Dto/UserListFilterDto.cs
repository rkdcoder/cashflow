namespace CashFlow.IdentityAndAccess.Application.Users.Dto
{
    public class UserListFilterDto
    {
        public Guid? UserId { get; set; }
        public string? LoginOrEmail { get; set; }
        public bool? Enabled { get; set; }
    }
}