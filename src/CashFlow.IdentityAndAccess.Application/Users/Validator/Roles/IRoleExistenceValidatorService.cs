namespace CashFlow.IdentityAndAccess.Application.Users.Validator.Roles
{
    public interface IRoleExistenceValidatorService
    {
        Task<(bool Success, List<string> NotFoundRoles)> ValidateRolesExistAsync(IEnumerable<string> roleNames, CancellationToken cancellationToken);
    }
}