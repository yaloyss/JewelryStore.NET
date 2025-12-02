using FluentValidation;
using Reviews.Application.Commands.RatingCommands.DeleteRating;

namespace Reviews.Application.Validators.RatingValidators
{
	public class DeleteRatingCommandValidator : AbstractValidator<DeleteRatingCommand>
    {
        public DeleteRatingCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Id cannot be empty");
        }
    }
}

