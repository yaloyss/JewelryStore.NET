using Reviews.Domain.Entities;
using Reviews.Domain.Exceptions;
using Reviews.Domain.Interfaces;
using Reviews.Domain.Interfaces.Services;

namespace Reviews.Application.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;

        public ReviewService(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        public async Task<Review> CreateReviewWithRatingAsync(int productId, int score, string title, string body, CancellationToken cancellationToken = default)
        {
            var rating = new Rating(score);
            var review = new Review(productId, rating, title, body);

            var createdReview = await _reviewRepository.AddAsync(review, cancellationToken);
            return createdReview;
        }

        public async Task<IEnumerable<(Review review, Rating rating)>> GetProductReviewsWithRatingsAsync(int productId, CancellationToken cancellationToken = default)
        {
            var reviews = await _reviewRepository.GetByProductIdAsync(productId, cancellationToken);

            // Rating вже є всередині Review як embedded document
            return reviews.Select(r => (review: r, rating: r.Rating));
        }

        public async Task<double> CalculateProductAverageRatingAsync(int productId, CancellationToken cancellationToken = default)
        {
            var reviews = await _reviewRepository.GetByProductIdAsync(productId, cancellationToken);
            if (!reviews.Any())
                return 0.0;

            var averageRating = reviews.Average(r => r.Rating.Score.Value);
            return Math.Round(averageRating, 2);
        }

        public async Task<bool> DeleteReviewWithRatingAsync(string reviewId, CancellationToken cancellationToken = default)
        {
            var review = await _reviewRepository.GetByIdAsync(reviewId, cancellationToken);
            if (review == null)
                throw new NotFoundException(reviewId);

            var deleted = await _reviewRepository.DeleteAsync(reviewId, cancellationToken);
            return deleted;
        }
    }
}

