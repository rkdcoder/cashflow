using CashFlow.Entries.Application.Categories.Command;
using FluentValidation;

namespace CashFlow.Entries.Application.Categories.Validator
{
    namespace CashFlow.Entries.Application.Categories.Validator
    {
        public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
        {
            public CreateCategoryCommandValidator()
            {
                RuleFor(x => x.Name)
                    .NotEmpty().WithMessage("Name is required.")
                    .MaximumLength(100).WithMessage("Name must have at most 100 characters.");

                RuleFor(x => x.EntryTypeId)
                    .NotEmpty().WithMessage("EntryTypeId is required.");
            }
        }
    }
}