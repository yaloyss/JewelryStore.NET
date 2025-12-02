using Reviews.Domain.Entities;

namespace Reviews.Domain.Interfaces.Services
{
	public interface IReviewService
	{
        Task<Review> CreateReviewWithRatingAsync(int productId, int score, string title, string body, CancellationToken cancellationToken = default);
        Task<IEnumerable<(Review review, Rating rating)>> GetProductReviewsWithRatingsAsync(int productId, CancellationToken cancellationToken = default);
        Task<double> CalculateProductAverageRatingAsync(int productId, CancellationToken cancellationToken = default);
        Task<bool> DeleteReviewWithRatingAsync(string reviewId, CancellationToken cancellationToken = default);
    }
}

