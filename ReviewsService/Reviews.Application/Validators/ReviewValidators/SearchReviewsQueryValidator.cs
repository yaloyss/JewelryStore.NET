using FluentValidation;
using Reviews.Application.Queries.ReviewQueries;

namespace Reviews.Application.Validators.RatingValidators
{
	public class SearchReviewsQueryValidator : AbstractValidator<SearchReviewsQuery>
    {
        public SearchReviewsQueryValidator()
        {
            RuleFor(x => x.SearchText).NotEmpty()
                .WithMessage("Search text is required")
                .MinimumLength(3)
                .WithMessage("Search text must be at least 3 characters long")
                .MaximumLength(100)
                .WithMessage("Search text cannot exceed 100 characters");

            RuleFor(x => x.ProductId).GreaterThan(0).When(x => x.ProductId.HasValue)
                .WithMessage("Product Id must be greater than 0");

            RuleFor(x => x.MinRating).InclusiveBetween(1, 5).When(x => x.MinRating.HasValue)
                .WithMessage("Min Rating must be between 1 and 5");

            RuleFor(x => x.MaxRating).InclusiveBetween(1, 5).When(x => x.MaxRating.HasValue)
                .WithMessage("Max Rating must be between 1 and 5");

            RuleFor(x => x)
                .Must(x => !x.MinRating.HasValue || !x.MaxRating.HasValue || x.MinRating.Value <= x.MaxRating.Value)
                .WithMessage("Min Rating cannot be greater than Max Rating");

            RuleFor(x => x.PageNumber).GreaterThan(0).WithMessage("Page Number must be greater than 0");

            RuleFor(x => x.PageSize).InclusiveBetween(1, 100).WithMessage("PageSize must be between 1 and 100");

            RuleFor(x => x)
                .Must(x => !x.StartDate.HasValue || !x.EndDate.HasValue || x.StartDate.Value <= x.EndDate.Value)
                .WithMessage("Start Date cannot be after End Date");
        }
    }
}

