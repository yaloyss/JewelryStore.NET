using MediatR;
using Reviews.Application.Interfaces;
using Reviews.Domain.Entities;

namespace Reviews.Application.Commands.DiscussionCommand
{
    public class AddMessageCommandHandler : IRequestHandler<AddMessageCommand, Discussion>
    {
        private readonly IDiscussionService _discussionService;

        public AddMessageCommandHandler(IDiscussionService discussionService)
        {
            _discussionService = discussionService;
        }

        public async Task<Discussion> Handle(AddMessageCommand request, CancellationToken cancellationToken)
        {
            return await _discussionService.AddMessageToDiscussionAsync(
                request.DiscussionId, request.MessageText, cancellationToken);
        }
    }
}

