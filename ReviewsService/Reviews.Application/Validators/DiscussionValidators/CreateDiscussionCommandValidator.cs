using System;
using FluentValidation;
using Reviews.Application.Commands.DiscussionCommand;

namespace Reviews.Application.Validators.DiscussionValidators
{
	public class CreateDiscussionCommandValidator : AbstractValidator<CreateDiscussionCommand>
    {
        public CreateDiscussionCommandValidator()
        {
            RuleFor(x => x.ReviewId).NotEmpty().WithMessage("Review Id cannot be empty");

            RuleFor(x => x.InitialMessage).NotEmpty()
                .WithMessage("Message cannot be empty")
                .MaximumLength(500)
                .WithMessage("Message cannot exceed 500 characters");
        }
    }
}

