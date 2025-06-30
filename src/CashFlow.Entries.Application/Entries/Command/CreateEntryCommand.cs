using CashFlow.Shared.Dtos;
using MediatR;

namespace CashFlow.Entries.Application.Entries.Command
{
    public record CreateEntryCommand(
        decimal Amount,
        string? Description,
        Guid CategoryId
    ) : IRequest<CommandResultDto<Guid>>;
}