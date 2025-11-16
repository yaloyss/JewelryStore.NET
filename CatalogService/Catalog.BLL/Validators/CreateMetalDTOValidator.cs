using Catalog.BLL.DTOs.Metal;
using FluentValidation;

namespace Catalog.BLL.Validators
{
    public class CreateMetalDTOValidator : AbstractValidator<CreateMetalDTO>
    {
        public CreateMetalDTOValidator()
        {
            RuleFor(x => x.Name)
                .Must(name => !string.IsNullOrWhiteSpace(name))
                .WithMessage("Metal name cannot consist only of whitespace");

            RuleFor(x => x.Name)
                .Matches(@"^[A-Za-zА-Яа-яІіЇїЄєҐґ]")
                .WithMessage("Metal name must start with a letter");

            RuleFor(x => x.Color)
                .Matches(@"^[A-Za-zА-Яа-яІіЇїЄєҐґ]")
                .WithMessage("Color name must start with a letter");
        }
    }
}

