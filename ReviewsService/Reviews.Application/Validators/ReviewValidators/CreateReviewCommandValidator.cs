using FluentValidation;
using Reviews.Application.Commands.ReviewCommands.CreateReview;

namespace Reviews.Application.Validators.ReviewValidators
{
	public class CreateReviewCommandValidator : AbstractValidator<CreateReviewCommand>
    {
        public CreateReviewCommandValidator()
        {
            RuleFor(x => x.ProductId).GreaterThan(0).WithMessage("Product Id must be greater than 0");

            RuleFor(x => x.Score).InclusiveBetween(1, 5).WithMessage("Raiting must be between 1 and 5");

            RuleFor(x => x.Title).NotEmpty()
                .WithMessage("Title cannot be empty")
                .MaximumLength(200)
                .WithMessage("Title cannot exceed 200 characters");

            RuleFor(x => x.Body).NotEmpty()
                .WithMessage("Review body cannot be empty")
                .MinimumLength(10)
                .WithMessage("Review body must contain at least 10 characters")
                .MaximumLength(2000)
                .WithMessage("Review body cannot exceed 2000 characters");
        }
    }
}

