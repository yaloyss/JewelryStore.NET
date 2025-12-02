using FluentValidation;
using Reviews.Application.Commands.ReviewCommands.UpdateReview;

namespace Reviews.Application.Validators.ReviewValidators
{
	public class UpdateReviewCommandValidator : AbstractValidator<UpdateReviewCommand>
    {
        public UpdateReviewCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Id cannot be empty");

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

