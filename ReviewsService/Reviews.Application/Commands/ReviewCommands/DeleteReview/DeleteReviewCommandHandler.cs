using MediatR;
using Reviews.Domain.Interfaces.Services;

namespace Reviews.Application.Commands.ReviewCommands.DeleteReview
{
    public class DeleteReviewCommandHandler : IRequestHandler<DeleteReviewCommand, bool>
    {
        private readonly IReviewService _reviewService;

        public DeleteReviewCommandHandler(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        public async Task<bool> Handle(DeleteReviewCommand request, CancellationToken cancellationToken)
        {
            return await _reviewService.DeleteReviewWithRatingAsync(request.Id, cancellationToken);
        }
    }
}

