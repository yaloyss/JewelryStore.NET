using Reviews.Domain.Entities;

namespace Reviews.Domain.Interfaces
{
	public interface IReviewRepository : IRepository<Review>
	{
        Task<IEnumerable<Review>> GetByProductIdAsync(int productId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Review>> GetByScoreAsync(int score, CancellationToken cancellationToken = default);
        Task<IEnumerable<Review>> GetByProductIdPagedAsync(int productId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
        Task<long> CountByProductIdAsync(int productId, CancellationToken cancellationToken = default);         //number of reviews on a product

    }
}

