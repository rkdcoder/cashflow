namespace CashFlow.IdentityAndAccess.Application.Users.Dto
{
    public class JwtTokenDto
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
    }
}