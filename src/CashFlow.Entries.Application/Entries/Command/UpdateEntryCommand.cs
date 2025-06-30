using CashFlow.Shared.Dtos;
using MediatR;

namespace CashFlow.Entries.Application.Entries.Command
{
    public record UpdateEntryCommand(
        Guid Id,
        decimal Amount,
        string Description,
        Guid CategoryId
    ) : IRequest<CommandResultDto<bool>>;
}