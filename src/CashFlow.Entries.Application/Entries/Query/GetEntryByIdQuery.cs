using CashFlow.Entries.Application.Entries.Dto;
using MediatR;

namespace CashFlow.Entries.Application.Entries.Query
{
    public record GetEntryByIdQuery(Guid Id) : IRequest<EntryDto>;
}