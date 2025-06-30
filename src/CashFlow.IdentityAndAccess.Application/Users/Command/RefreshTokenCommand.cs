using CashFlow.IdentityAndAccess.Application.Users.Dto;
using MediatR;

namespace CashFlow.IdentityAndAccess.Application.Users.Command
{
    public record RefreshTokenCommand(RefreshTokenRequestDto Request) : IRequest<LoginResponseDto>;
}