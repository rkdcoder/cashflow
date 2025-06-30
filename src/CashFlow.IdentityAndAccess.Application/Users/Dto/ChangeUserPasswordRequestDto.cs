namespace CashFlow.IdentityAndAccess.Application.Users.Dto
{
    public class ChangeUserPasswordRequestDto
    {
         public string LoginOrEmail { get; set; } = string.Empty;
         public string NewPassword { get; set; } = string.Empty;
    }
}