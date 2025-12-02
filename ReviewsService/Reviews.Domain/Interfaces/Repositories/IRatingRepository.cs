using Reviews.Domain.Entities;

namespace Reviews.Domain.Interfaces
{
	public interface IRatingRepository : IRepository<Rating>
    {
        Task<IEnumerable<Rating>> GetByScoreAsync(int score, CancellationToken cancellationToken = default);
        Task<IEnumerable<Rating>> GetHighRatingsAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Rating>> GetLowRatingsAsync(CancellationToken cancellationToken = default);
        Task<double> GetAverageScoreAsync(CancellationToken cancellationToken = default);
    }
}

