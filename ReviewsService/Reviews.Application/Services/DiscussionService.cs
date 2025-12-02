using MongoDB.Bson;
using Reviews.Domain.Entities;
using Reviews.Domain.Exceptions;
using Reviews.Domain.Interfaces;
using Reviews.Domain.Interfaces.Services;

namespace Reviews.Application.Services
{
    public class DiscussionService : IDiscussionService
    {
        private readonly IDiscussionRepository _discussionRepository;
        private readonly IReviewRepository _reviewRepository;

        public DiscussionService(IDiscussionRepository discussionRepository, IReviewRepository reviewRepository)
        {
            _discussionRepository = discussionRepository;
            _reviewRepository = reviewRepository;
        }

        public async Task<Discussion> CreateDiscussionForReviewAsync(string reviewId, string initialMessage, CancellationToken cancellationToken = default)
        {
            if (!ObjectId.TryParse(reviewId, out var reviewObjectId))
                throw new DomainException("Invalid reviewId format");

            var review = await _reviewRepository.GetByIdAsync(reviewObjectId.ToString(), cancellationToken);
            if (review == null)
                throw new NotFoundException(reviewId);

            var discussion = new Discussion(reviewObjectId);
            discussion.AddMessage(initialMessage);
            return await _discussionRepository.AddAsync(discussion, cancellationToken);
        }

        public async Task<Discussion> AddMessageToDiscussionAsync(string discussionId, string messageText, CancellationToken cancellationToken = default)
        {
            var discussion = await _discussionRepository.GetByIdAsync(discussionId, cancellationToken);

            if (discussion == null)
                throw new NotFoundException(discussionId);

            // Додаємо повідомлення через доменний метод
            discussion.AddMessage(messageText);
            return await _discussionRepository.UpdateAsync(discussion, cancellationToken);
        }

        public async Task<(Discussion discussion, Review review)> GetDiscussionWithReviewAsync(string discussionId, CancellationToken cancellationToken = default)
        {
            // Отримуємо дискусію
            var discussion = await _discussionRepository.GetByIdAsync(discussionId, cancellationToken);

            if (discussion == null)
                throw new NotFoundException(discussionId);

            // Отримуємо відгук
            var review = await _reviewRepository.GetByIdAsync(discussion.ReviewId.ToString(), cancellationToken);
            return (discussion, review);
        }

        public async Task<IEnumerable<Discussion>> GetDiscussionsForReviewAsync(string reviewId, CancellationToken cancellationToken = default)
        {
            var discussion = await _discussionRepository.GetByReviewIdAsync(reviewId, cancellationToken);

            if (discussion == null)
                return Enumerable.Empty<Discussion>();

            return new[] { discussion };
        }
    }
}

