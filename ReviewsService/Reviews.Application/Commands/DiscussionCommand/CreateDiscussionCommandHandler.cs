using MediatR;
using Reviews.Domain.Entities;
using Reviews.Domain.Interfaces.Services;

namespace Reviews.Application.Commands.DiscussionCommand
{
    public class CreateDiscussionCommandHandler : IRequestHandler<CreateDiscussionCommand, Discussion>
    {
        private readonly IDiscussionService _discussionService;

        public CreateDiscussionCommandHandler(IDiscussionService discussionService)
        {
            _discussionService = discussionService;
        }

        public async Task<Discussion> Handle(CreateDiscussionCommand request, CancellationToken cancellationToken)
        {
            return await _discussionService.CreateDiscussionForReviewAsync(
                request.ReviewId, request.InitialMessage, cancellationToken);
        }
    }
}

