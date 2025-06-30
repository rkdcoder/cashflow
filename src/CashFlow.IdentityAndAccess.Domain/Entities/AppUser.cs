namespace CashFlow.IdentityAndAccess.Domain.Entities
{
    public class AppUser
    {
        public Guid Id { get; set; }
        public string LoginName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public bool Enabled { get; set; } = true;

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}