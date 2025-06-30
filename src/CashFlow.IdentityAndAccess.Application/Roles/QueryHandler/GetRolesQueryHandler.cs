using AutoMapper;
using CashFlow.IdentityAndAccess.Application.Roles.Dto;
using CashFlow.IdentityAndAccess.Application.Roles.Query;
using CashFlow.IdentityAndAccess.Domain.Roles.Repositories;
using MediatR;

namespace CashFlow.IdentityAndAccess.Application.Roles.QueryHandler
{
    public class GetRolesQueryHandler : IRequestHandler<GetRolesQuery, List<SimpleRoleDto>>
    {
        private readonly IRoleQueryRepository _roleQueryRepository;
        private readonly IMapper _mapper;

        public GetRolesQueryHandler(IRoleQueryRepository roleQueryRepository, IMapper mapper)
        {
            _roleQueryRepository = roleQueryRepository;
            _mapper = mapper;
        }

        public async Task<List<SimpleRoleDto>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
        {
            var roles = await _roleQueryRepository.GetAllRolesAsync();
            return _mapper.Map<List<SimpleRoleDto>>(roles);
        }
    }
}