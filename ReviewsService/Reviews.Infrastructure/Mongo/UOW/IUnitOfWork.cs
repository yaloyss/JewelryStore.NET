using Reviews.Domain.Interfaces;

namespace Reviews.Infrastructure.Mongo.UOW
{
    public interface IUnitOfWork : IDisposable
    {
        IRatingRepository Ratings { get; }
        IReviewRepository Reviews { get; }
        IDiscussionRepository Discussions { get; }

        Task BeginTransactionAsync(CancellationToken cancellationToken = default);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    }
}

