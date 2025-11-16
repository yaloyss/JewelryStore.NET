using Catalog.BLL.DTOs.Category;
using FluentValidation;

namespace Catalog.BLL.Validators
{
    public class CreateCategoryDTOValidator : AbstractValidator<CreateCategoryDTO>
    {
        public CreateCategoryDTOValidator()
        {
            RuleFor(x => x.Name)
                .Must(name => !string.IsNullOrWhiteSpace(name))
                .WithMessage("Category name cannot consist only of whitespace");

            RuleFor(x => x.Name)
                .Matches(@"^[A-Za-zА-Яа-яІіЇїЄєҐґ]")
                .WithMessage("Category name must start with a letter");
        }
    }
}

