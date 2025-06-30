using CashFlow.IdentityAndAccess.Application.Roles.Dto;
using MediatR;

namespace CashFlow.IdentityAndAccess.Application.Roles.Query
{
    public record GetRolesQuery() : IRequest<List<SimpleRoleDto>>;
}