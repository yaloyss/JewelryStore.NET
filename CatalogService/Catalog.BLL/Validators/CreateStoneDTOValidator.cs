using Catalog.BLL.DTOs.Stone;
using FluentValidation;

namespace Catalog.BLL.Validators
{
    public class CreateStoneDTOValidator : AbstractValidator<CreateStoneDTO>
    {
        public CreateStoneDTOValidator()
        {
            RuleFor(x => x.Name)
                .Must(name => !string.IsNullOrWhiteSpace(name))
                .WithMessage("Stone name cannot consist only of whitespace");

            RuleFor(x => x.Name)
                .Matches(@"^[A-Za-zА-Яа-яІіЇїЄєҐґ]")
                .WithMessage("Stone name must start with a letter");
        }
    }
}

