using MediatR;
using Reviews.Domain.Entities;
using Reviews.Domain.Interfaces;

namespace Reviews.Application.Commands.ReviewCommands.UpdateReview
{
    public class UpdateReviewCommandHandler : IRequestHandler<UpdateReviewCommand, Review>
    {
        private readonly IReviewRepository _reviewRepository;

        public UpdateReviewCommandHandler(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        public async Task<Review> Handle(UpdateReviewCommand request, CancellationToken cancellationToken)
        {
            var review = await _reviewRepository.GetByIdAsync(request.Id, cancellationToken);

            if (review == null)
                throw new EntityNotFoundException(request.Id, "Review");

            review.UpdateReviewText(request.Title, request.Body);
            return await _reviewRepository.UpdateAsync(review, cancellationToken);
        }
    }
}

