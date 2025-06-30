using CashFlow.BuildingBlocks.Domain.Caching;
using CashFlow.Entries.Application.Entries.Command;
using CashFlow.Entries.Domain.Entries.Repositories;
using CashFlow.Shared.Abstractions;
using CashFlow.Shared.Dtos;
using CashFlow.Shared.Exceptions.Application;
using CashFlow.Shared.Utilities;
using CashFlow.Shared.ValueObjects;
using MediatR;

namespace CashFlow.Entries.Application.Entries.CommandHandler
{
    public class DeleteEntryCommandHandler : IRequestHandler<DeleteEntryCommand, CommandResultDto<bool>>
    {
        private readonly IEntryRepository _repository;
        private readonly ICurrentUserContext _currentUser;
        private readonly IRedisCacheService _redisCache;

        public DeleteEntryCommandHandler(IEntryRepository repository,
            ICurrentUserContext currentUser,
            IRedisCacheService redisCache)
        {
            _repository = repository;
            _currentUser = currentUser;
            _redisCache = redisCache;
        }

        public async Task<CommandResultDto<bool>> Handle(DeleteEntryCommand request, CancellationToken cancellationToken)
        {
            var requiredRole = UserRoles.Manager;
            if (!UserHierarchyUtils.HasRequiredRole(_currentUser.Roles, requiredRole))
                throw new ForbiddenException($"You do not have sufficient permissions to delete entries. Required role: {requiredRole}.");

            var entry = await _repository.GetByIdAsync(request.Id);
            if (entry == null)
                throw new NotFoundException("Entry not found.");

            await _repository.DeleteAsync(entry);

            await _redisCache.DeleteAsync($"daily-entry_{entry.CreatedAt.ToString("yyyy-MM-dd")}_{entry.Id}");

            return CommandResultDto<bool>.SuccessResult("Entry deleted successfully.", true);
        }
    }
}