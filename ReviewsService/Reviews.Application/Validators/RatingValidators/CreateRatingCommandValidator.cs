using FluentValidation;
using Reviews.Application.Commands.RatingCommands.CreateRating;

namespace Reviews.Application.Validators.RatingValidators
{
	public class CreateRatingCommandValidator : AbstractValidator<CreateRatingCommand>
    {
        public CreateRatingCommandValidator()
        {
            RuleFor(x => x.Score).InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5");
        }
    }
}

