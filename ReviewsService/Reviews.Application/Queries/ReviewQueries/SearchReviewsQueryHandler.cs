using MediatR;
using Reviews.Domain.Entities;
using Reviews.Domain.Interfaces;

namespace Reviews.Application.Queries.ReviewQueries
{
    public class SearchReviewsQueryHandler : IRequestHandler<SearchReviewsQuery, List<Review>>
    {
        private readonly IReviewRepository _reviewRepository;

        public SearchReviewsQueryHandler(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        public async Task<List<Review>> Handle(SearchReviewsQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Review> reviews;

            reviews = await _reviewRepository.GetAllAsync(cancellationToken);
            
            if (request.ProductId.HasValue)
            {
                reviews = reviews.Where(r => r.ProductId == request.ProductId.Value);
            }

            if (request.MinRating.HasValue)
            {
                reviews = reviews.Where(r => r.Rating.Score.Value >= request.MinRating.Value);
            }

            if (request.MaxRating.HasValue)
            {
                reviews = reviews.Where(r => r.Rating.Score.Value <= request.MaxRating.Value);
            }

            if (request.StartDate.HasValue)
            {
                reviews = reviews.Where(r => r.CreatedAt >= request.StartDate.Value);
            }

            if (request.EndDate.HasValue)
            {
                reviews = reviews.Where(r => r.CreatedAt <= request.EndDate.Value);
            }

            reviews = reviews
                .OrderByDescending(r => r.CreatedAt)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize);

            return reviews.ToList();
        }
    }
}
