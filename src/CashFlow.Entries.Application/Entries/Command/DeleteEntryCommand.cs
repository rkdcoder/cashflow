using CashFlow.Shared.Dtos;
using MediatR;

namespace CashFlow.Entries.Application.Entries.Command
{
    public record DeleteEntryCommand(Guid Id) : IRequest<CommandResultDto<bool>>;
}