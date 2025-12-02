using MediatR;
using Reviews.Domain.Entities;
using Reviews.Domain.Exceptions;
using Reviews.Domain.Interfaces;

namespace Reviews.Application.Queries.ReviewQueries
{
    public class GetReviewByIdQueryHandler : IRequestHandler<GetReviewByIdQuery, Review>
    {
        private readonly IReviewRepository _reviewRepository;

        public GetReviewByIdQueryHandler(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        public async Task<Review> Handle(GetReviewByIdQuery request, CancellationToken cancellationToken)
        {
            var review = await _reviewRepository.GetByIdAsync(request.Id, cancellationToken);

            if (review == null)
                throw new NotFoundException(request.Id);
            return review;
        }
    }
}

