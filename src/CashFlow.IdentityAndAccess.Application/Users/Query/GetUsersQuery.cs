using CashFlow.IdentityAndAccess.Application.Users.Dto;
using MediatR;

namespace CashFlow.IdentityAndAccess.Application.Users.Query
{
    public record GetUsersQuery(
        Guid? UserId,
        string? LoginOrEmail,
        bool? Enabled
    ) : IRequest<List<UserListItemDto>>;
}