using AutoMapper;
using CashFlow.IdentityAndAccess.Application.Users.Dto;
using CashFlow.IdentityAndAccess.Application.Users.Query;
using CashFlow.IdentityAndAccess.Domain.Users.Repositories;
using MediatR;

namespace CashFlow.IdentityAndAccess.Application.Users.QueryHandler
{
    public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, List<UserListItemDto>>
    {
        private readonly IAppUserQueryRepository _userQueryRepository;
        private readonly IMapper _mapper;

        public GetUsersQueryHandler(IAppUserQueryRepository userQueryRepository, IMapper mapper)
        {
            _userQueryRepository = userQueryRepository;
            _mapper = mapper;
        }

        public async Task<List<UserListItemDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            var users = await _userQueryRepository.GetUsersWithRolesAsync(request.UserId, request.LoginOrEmail, request.Enabled);
            return _mapper.Map<List<UserListItemDto>>(users);
        }
    }
}