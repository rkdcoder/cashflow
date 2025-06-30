using CashFlow.Shared.Dtos;
using MediatR;

namespace CashFlow.Entries.Application.Categories.Command
{
    public record CreateCategoryCommand(
        string Name,
        Guid EntryTypeId
    ) : IRequest<CommandResultDto<Guid>>;
}