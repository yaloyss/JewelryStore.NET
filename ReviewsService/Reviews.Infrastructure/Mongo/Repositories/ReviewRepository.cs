using MongoDB.Driver;
using Reviews.Domain.Entities;
using Reviews.Domain.Interfaces;

namespace Reviews.Infrastructure.Mongo.Repositories
{
    public class ReviewRepository : MongoRepository<Review>, IReviewRepository
    {
        public ReviewRepository(MongoDbContext context) : base(context, "reviews") { }

        public async Task<IEnumerable<Review>> GetByProductIdAsync(int productId, CancellationToken cancellationToken = default)
        {
            var filter = Builders<Review>.Filter.Eq(r => r.ProductId, productId);
            return await _collection.Find(filter).ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Review>> GetByProductIdPagedAsync(int productId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var filter = Builders<Review>.Filter.Eq(r => r.ProductId, productId);

            return await _collection
                .Find(filter)
                .SortByDescending(r => r.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Review>> GetByScoreAsync(int score, CancellationToken cancellationToken = default)
        {
            var filter = Builders<Review>.Filter.Eq(r => r.Rating.Score.Value, score);
            return await _collection.Find(filter).ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Review>> SearchByTextAsync(string searchText, CancellationToken cancellationToken = default)
        {
            // MongoDB Text Search (потрібен text index)
            var filter = Builders<Review>.Filter.Text(searchText);
            return await _collection.Find(filter).ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Review>> GetRecentReviewsAsync(int count, CancellationToken cancellationToken = default)
        {
            return await _collection
                .Find(_ => true)
                .SortByDescending(r => r.CreatedAt)
                .Limit(count)
                .ToListAsync(cancellationToken);
        }

        public async Task<long> CountByProductIdAsync(int productId, CancellationToken cancellationToken = default)
        {
            var filter = Builders<Review>.Filter.Eq(r => r.ProductId, productId);
            return await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
        }

        public async Task<bool> HasReviewsForProductAsync(int productId, CancellationToken cancellationToken = default)
        {
            var filter = Builders<Review>.Filter.Eq(r => r.ProductId, productId);
            var count = await _collection.CountDocumentsAsync(filter, new CountOptions { Limit = 1 }, cancellationToken);
            return count > 0;
        }

        public async Task<IEnumerable<Review>> GetReviewsInPeriodAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        {
            var filter = Builders<Review>.Filter.And(
                Builders<Review>.Filter.Gte(r => r.CreatedAt, startDate),
                Builders<Review>.Filter.Lte(r => r.CreatedAt, endDate));

            return await _collection.Find(filter).ToListAsync(cancellationToken);
        }
    }
}

