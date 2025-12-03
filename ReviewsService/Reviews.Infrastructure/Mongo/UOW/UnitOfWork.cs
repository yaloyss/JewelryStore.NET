using Reviews.Domain.Interfaces;

namespace Reviews.Infrastructure.Mongo.UOW
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MongoDbContext _context;
        private bool _disposed;

        public IRatingRepository Ratings { get; }
        public IReviewRepository Reviews { get; }
        public IDiscussionRepository Discussions { get; }

        public UnitOfWork(
            MongoDbContext context,
            IRatingRepository ratingRepository,
            IReviewRepository reviewRepository,
            IDiscussionRepository discussionRepository)
        {
            _context = context;
            Ratings = ratingRepository;
            Reviews = reviewRepository;
            Discussions = discussionRepository;
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            await _context.StartSessionAsync();
            _context.StartTransaction();
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await _context.CommitTransactionAsync(cancellationToken);
                return 1; // Success
            }
            catch
            {
                await _context.AbortTransactionAsync(cancellationToken);
                throw;
            }
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            await _context.AbortTransactionAsync(cancellationToken);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _context.Dispose();
            }
            _disposed = true;
        }
    }
}

