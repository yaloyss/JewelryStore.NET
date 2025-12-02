using FluentValidation;
using Reviews.Application.Commands.DiscussionCommand;

namespace Reviews.Application.Validators.DiscussionValidators
{
	public class AddMessageCommandValidator : AbstractValidator<AddMessageCommand>
    {
        public AddMessageCommandValidator()
        {
            RuleFor(x => x.DiscussionId).NotEmpty().WithMessage("DiscussionId не може бути порожнім");

            RuleFor(x => x.MessageText)
                .NotEmpty()
                .WithMessage("Message cannot be empty")
                .MaximumLength(500)
                .WithMessage("Message cannot exceed 500 characters");
        }
    }
}

