using AutoMapper;
using CashFlow.Entries.Application.Entries.Dto;
using CashFlow.Entries.Application.Entries.Query;
using CashFlow.Entries.Domain.Entries.Repositories;
using CashFlow.Shared.Exceptions.Application;
using MediatR;

namespace CashFlow.Entries.Application.Entries.QueryHandler
{
    public class GetEntryByIdQueryHandler : IRequestHandler<GetEntryByIdQuery, EntryDto>
    {
        private readonly IEntryRepository _repository;
        private readonly IMapper _mapper;

        public GetEntryByIdQueryHandler(IEntryRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<EntryDto> Handle(GetEntryByIdQuery request, CancellationToken cancellationToken)
        {
            var entry = await _repository.GetByIdAsync(request.Id);
            if (entry == null)
                throw new NotFoundException("Entry not found.");

            return _mapper.Map<EntryDto>(entry);
        }
    }
}