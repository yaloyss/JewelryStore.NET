using Reviews.Domain.Entities;

namespace Reviews.Domain.Interfaces
{
	public interface IDiscussionRepository : IRepository<Discussion>
	{
        Task<Discussion> GetByReviewIdAsync(string reviewId, CancellationToken cancellationToken = default);
    }
}

