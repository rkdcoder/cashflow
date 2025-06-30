using AutoMapper;
using CashFlow.Entries.Application.Entries.Dto;
using CashFlow.Entries.Application.Entries.Query;
using CashFlow.Entries.Domain.Entries.Repositories;
using MediatR;

namespace CashFlow.Entries.Application.Entries.QueryHandler
{
    public class GetAllEntryTypesQueryHandler : IRequestHandler<GetAllEntryTypesQuery, List<EntryTypeDto>>
    {
        private readonly IEntryTypeRepository _repository;
        private readonly IMapper _mapper;

        public GetAllEntryTypesQueryHandler(IEntryTypeRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<EntryTypeDto>> Handle(GetAllEntryTypesQuery request, CancellationToken cancellationToken)
        {
            var types = await _repository.GetAllAsync();
            return _mapper.Map<List<EntryTypeDto>>(types);
        }
    }
}