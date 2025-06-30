using CashFlow.IdentityAndAccess.Application.Users.Dto;
using CashFlow.Shared.Dtos;
using MediatR;

namespace CashFlow.IdentityAndAccess.Application.Users.Command
{
    public record CreateUserCommand(CreateUserRequestDto User)
        : IRequest<CommandResultDto<SimpleUserDto>>;
}