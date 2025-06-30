using AutoMapper;
using CashFlow.Entries.Application.Categories.Dto;
using CashFlow.Entries.Application.Categories.Query;
using CashFlow.Entries.Domain.Categories.Repositories;
using MediatR;

namespace CashFlow.Entries.Application.Categories.QueryHandler
{
    public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, List<CategoryDto>>
    {
        private readonly ICategoryRepository _repository;
        private readonly IMapper _mapper;

        public GetAllCategoriesQueryHandler(ICategoryRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<CategoryDto>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
        {
            var categories = await _repository.GetAllWithEntryTypeAsync();
            return _mapper.Map<List<CategoryDto>>(categories);
        }
    }
}