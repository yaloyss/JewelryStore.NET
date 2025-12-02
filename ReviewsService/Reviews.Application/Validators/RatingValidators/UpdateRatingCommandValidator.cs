using FluentValidation;
using Reviews.Application.Commands.RatingCommands.UpdateRating;

namespace Reviews.Application.Validators.RatingValidators
{
	public class UpdateRatingCommandValidator : AbstractValidator<UpdateRatingCommand>
    {
        public UpdateRatingCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Id cannot be empty");
            RuleFor(x => x.Score).InclusiveBetween(1, 5).WithMessage("Rating score must be between 1 and 5");
        }
    }
}

