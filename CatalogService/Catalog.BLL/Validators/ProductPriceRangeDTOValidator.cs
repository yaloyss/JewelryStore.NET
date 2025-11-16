using Catalog.BLL.DTOs.Product;
using FluentValidation;

namespace Catalog.BLL.Validators
{
	public class ProductPriceRangeDTOValidator : AbstractValidator<ProductPriceRangeDTO>
    {
        public ProductPriceRangeDTOValidator()
        {
            RuleFor(x => x)
                .Must(x => x.MinPrice <= x.MaxPrice)
                .WithMessage("Minimum price cannot be greater than maximum price")
                .WithName("PriceRange");

            RuleFor(x => x.MaxPrice)
                .LessThanOrEqualTo(100000)
                .WithMessage("Maximum price cannot exceed 100,000");
        }
    }
}

