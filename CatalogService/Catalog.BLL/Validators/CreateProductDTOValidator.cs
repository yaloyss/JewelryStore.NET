using Catalog.BLL.DTOs.Product;
using FluentValidation;

namespace Catalog.BLL.Validators
{
    public class CreateProductDTOValidator : AbstractValidator<CreateProductDTO>
    {
        public CreateProductDTOValidator()
        {
            //check duplicate stoneids
            RuleFor(x => x.StoneIds)
                .Must(stoneIds => stoneIds.Count == stoneIds.Distinct().Count())
                .When(x => x.StoneIds.Any())
                .WithMessage("Stone list cannot contain duplicates");

            RuleFor(x => x.StoneIds)
                .Must(stoneIds => stoneIds.All(id => id > 0))
                .When(x => x.StoneIds.Any())
                .WithMessage("All stone IDs must be greater than 0");

            RuleFor(x => x.MetalId)
                .GreaterThan(0)
                .When(x => x.MetalId.HasValue)
                .WithMessage("Metal ID must be greater than 0");

            RuleFor(x => x.Size)
                .LessThanOrEqualTo(70)
                .When(x => x.Size.HasValue)
                .WithMessage("Size cannot exceed 70");

            RuleFor(x => x.Weight)
                .LessThanOrEqualTo(300)
                .WithMessage("Weight cannot exceed 300 grams");

            RuleFor(x => x.Price)
                .LessThanOrEqualTo(100000)
                .WithMessage("Price cannot exceed 100,000");
        }
    }
}

