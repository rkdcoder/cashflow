using CashFlow.IdentityAndAccess.Domain.Users.Repositories;

namespace CashFlow.IdentityAndAccess.Application.Users.Validator.Roles
{
    public class RoleExistenceValidatorService : IRoleExistenceValidatorService
    {
        private readonly IRoleRepository _roleRepository;

        public RoleExistenceValidatorService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<(bool Success, List<string> NotFoundRoles)> ValidateRolesExistAsync(IEnumerable<string> roleNames, CancellationToken cancellationToken)
        {
            var roles = (roleNames ?? new List<string>()).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            if (!roles.Any())
                return (true, new List<string>());

            var existingRoles = await _roleRepository.GetByNamesAsync(roles, cancellationToken);
            var existingRoleNames = existingRoles.Select(r => r.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var notFoundRoles = roles.Where(r => !existingRoleNames.Contains(r)).ToList();

            return (!notFoundRoles.Any(), notFoundRoles);
        }
    }
}