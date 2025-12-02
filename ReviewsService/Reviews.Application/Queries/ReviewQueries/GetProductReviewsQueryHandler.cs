using MediatR;
using Reviews.Domain.Entities;
using Reviews.Domain.Interfaces;

namespace Reviews.Application.Queries.ReviewQueries
{
    public class GetProductReviewsQueryHandler : IRequestHandler<GetProductReviewsQuery, List<Review>>
    {
        private readonly IReviewRepository _reviewRepository;

        public GetProductReviewsQueryHandler(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        public async Task<List<Review>> Handle(GetProductReviewsQuery request, CancellationToken cancellationToken)
        {
            var reviews = await _reviewRepository.GetByProductIdPagedAsync(request.ProductId, request.PageNumber, request.PageSize, cancellationToken);
            return reviews.ToList();
        }
    }
}

