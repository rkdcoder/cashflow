using CashFlow.BuildingBlocks.Domain.Caching;
using CashFlow.Entries.Application.Entries.Command;
using CashFlow.Entries.Domain.Categories.Repositories;
using CashFlow.Entries.Domain.Entries.Entities;
using CashFlow.Entries.Domain.Entries.Repositories;
using CashFlow.Shared.Abstractions;
using CashFlow.Shared.Dtos;
using CashFlow.Shared.Exceptions.Application;
using CashFlow.Shared.Utilities;
using CashFlow.Shared.ValueObjects;
using MediatR;

namespace CashFlow.Entries.Application.Entries.CommandHandler
{
    public class CreateEntryCommandHandler : IRequestHandler<CreateEntryCommand, CommandResultDto<Guid>>
    {
        private readonly IEntryRepository _repository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICurrentUserContext _currentUser;
        private readonly IRedisCacheService _redisCache;

        public CreateEntryCommandHandler(
            IEntryRepository entryRepository,
            ICategoryRepository categoryRepository,
            ICurrentUserContext currentUser,
            IRedisCacheService redisCache)
        {
            _repository = entryRepository;
            _categoryRepository = categoryRepository;
            _currentUser = currentUser;
            _redisCache = redisCache;
        }

        public async Task<CommandResultDto<Guid>> Handle(CreateEntryCommand request, CancellationToken cancellationToken)
        {
            var requiredRole = UserRoles.User;
            if (!UserHierarchyUtils.HasRequiredRole(_currentUser.Roles, requiredRole))
                throw new ForbiddenException($"You do not have sufficient permissions to create entries. Required role: {requiredRole}.");

            if (request.Amount <= 0)
                throw new BadRequestException("Amount must be bigger than 0.");

            // Recupera a categoria e seu tipo
            var category = await _categoryRepository.GetByIdAsync(request.CategoryId);
            if (category == null)
                throw new NotFoundException("Category does not exist.");

            var entryTypeId = category.EntryTypeId;

            var userId = Guid.TryParse(_currentUser.Subject, out var parsedId) ? parsedId : Guid.Empty;

            var entry = new Entry(
                request.Amount,
                request.Description ?? string.Empty,
                entryTypeId,
                request.CategoryId,
                userId
            );

            await _repository.AddAsync(entry);

            await _redisCache.AddOrUpdateAsync($"daily-entry_{entry.CreatedAt.ToString("yyyy-MM-dd")}_{entry.Id}", entry);

            return CommandResultDto<Guid>.SuccessResult("Entry created successfully.", entry.Id);
        }
    }
}