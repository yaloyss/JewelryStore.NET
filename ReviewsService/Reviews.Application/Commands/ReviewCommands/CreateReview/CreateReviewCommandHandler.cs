using MediatR;
using Reviews.Application.Interfaces;
using Reviews.Domain.Entities;

namespace Reviews.Application.Commands.ReviewCommands.CreateReview
{
    public class CreateReviewCommandHandler : IRequestHandler<CreateReviewCommand, Review>
    {
        private readonly IReviewService _reviewService;

        public CreateReviewCommandHandler(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        public async Task<Review> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
        {
            return await _reviewService.CreateReviewWithRatingAsync(
                request.ProductId, request.Score, request.Title,
                request.Body, cancellationToken);
        }
    }
}

