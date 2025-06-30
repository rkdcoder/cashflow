using CashFlow.Entries.Application.Categories.Command;
using CashFlow.Entries.Domain.Categories.Repositories;
using CashFlow.Entries.Domain.Entries.Entities;
using CashFlow.Entries.Domain.Entries.Repositories;
using CashFlow.Shared.Abstractions;
using CashFlow.Shared.Dtos;
using CashFlow.Shared.Exceptions.Application;
using CashFlow.Shared.Utilities;
using CashFlow.Shared.ValueObjects;
using MediatR;

namespace CashFlow.Entries.Application.Categories.CommandHandler
{
    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CommandResultDto<Guid>>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IEntryTypeRepository _entryTypeRepository;
        private readonly ITextSanitizerService _sanitizer;
        private readonly ICurrentUserContext _currentUser;

        public CreateCategoryCommandHandler(
            ICategoryRepository categoryRepository,
            IEntryTypeRepository entryTypeRepository,
            ITextSanitizerService sanitizer,
            ICurrentUserContext currentUser)
        {
            _categoryRepository = categoryRepository;
            _entryTypeRepository = entryTypeRepository;
            _sanitizer = sanitizer;
            _currentUser = currentUser;
        }

        public async Task<CommandResultDto<Guid>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            var requiredRole = UserRoles.Manager;
            if (!UserHierarchyUtils.HasRequiredRole(_currentUser.Roles, requiredRole))
                throw new ForbiddenException($"You do not have sufficient permissions to create categories. Required role: {requiredRole}.");

            var categoryName = _sanitizer.Normalize(request.Name);

            var existing = await _categoryRepository.GetByNameAsync(categoryName);
            if (existing != null)
                throw new ConflictException("This category name is already in use.");

            var entryType = await _entryTypeRepository.GetByIdAsync(request.EntryTypeId);
            if (entryType == null)
                throw new BadRequestException("EntryType does not exist.");

            var category = new Category(Guid.NewGuid(), categoryName, request.EntryTypeId);
            await _categoryRepository.AddAsync(category);

            return CommandResultDto<Guid>.SuccessResult("Category created successfully.", category.Id);
        }
    }
}