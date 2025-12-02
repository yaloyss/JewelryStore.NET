using Reviews.Domain.Entities;

namespace Reviews.Domain.Interfaces.Services
{
	public interface IDiscussionService
	{
        Task<Discussion> CreateDiscussionForReviewAsync(string reviewId, string initialMessage, CancellationToken cancellationToken = default);
        Task<Discussion> AddMessageToDiscussionAsync(string discussionId, string messageText, CancellationToken cancellationToken = default);
        Task<(Discussion discussion, Review review)> GetDiscussionWithReviewAsync(string discussionId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Discussion>> GetDiscussionsForReviewAsync(string reviewId, CancellationToken cancellationToken = default);
    }
}

