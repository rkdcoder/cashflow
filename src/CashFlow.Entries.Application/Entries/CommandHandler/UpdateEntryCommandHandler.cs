using CashFlow.BuildingBlocks.Domain.Caching;
using CashFlow.Entries.Application.Entries.Command;
using CashFlow.Entries.Domain.Categories.Repositories;
using CashFlow.Entries.Domain.Entries.Repositories;
using CashFlow.Shared.Abstractions;
using CashFlow.Shared.Dtos;
using CashFlow.Shared.Exceptions.Application;
using CashFlow.Shared.Utilities;
using CashFlow.Shared.ValueObjects;
using MediatR;

namespace CashFlow.Entries.Application.Entries.CommandHandler
{
    public class UpdateEntryCommandHandler : IRequestHandler<UpdateEntryCommand, CommandResultDto<bool>>
    {
        private readonly IEntryRepository _repository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICurrentUserContext _currentUser;
        private readonly IRedisCacheService _redisCache;

        public UpdateEntryCommandHandler(
            IEntryRepository repository,
            ICategoryRepository categoryRepository,
            ICurrentUserContext currentUser,
            IRedisCacheService redisCache)
        {
            _repository = repository;
            _categoryRepository = categoryRepository;
            _currentUser = currentUser;
            _redisCache = redisCache;
        }

        public async Task<CommandResultDto<bool>> Handle(UpdateEntryCommand request, CancellationToken cancellationToken)
        {
            var requiredRole = UserRoles.Manager;
            if (!UserHierarchyUtils.HasRequiredRole(_currentUser.Roles, requiredRole))
                throw new ForbiddenException($"You do not have sufficient permissions to update entries. Required role: {requiredRole}.");

            if (request.Amount <= 0)
                throw new BadRequestException("Amount must be bigger than 0.");

            var entry = await _repository.GetByIdAsync(request.Id);
            if (entry == null)
                throw new NotFoundException("Entry not found.");

            var category = await _categoryRepository.GetByIdAsync(request.CategoryId);
            if (category == null)
                throw new NotFoundException("Category does not exist.");

            var userId = Guid.TryParse(_currentUser.Subject, out var parsedId) ? parsedId : Guid.Empty;

            entry.Update(
                request.Amount,
                request.Description,
                category.EntryTypeId,
                request.CategoryId,
                userId
            );

            await _repository.UpdateAsync(entry);

            await _redisCache.AddOrUpdateAsync($"daily-entry_{entry.CreatedAt.ToString("yyyy-MM-dd")}_{entry.Id}", entry);

            return CommandResultDto<bool>.SuccessResult("Entry updated successfully.", true);
        }
    }
}