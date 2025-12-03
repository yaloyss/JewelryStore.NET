using MongoDB.Bson;
using MongoDB.Driver;
using Reviews.Domain.Entities;
using Reviews.Domain.Interfaces;

namespace Reviews.Infrastructure.Mongo.Repositories
{
    public class DiscussionRepository : MongoRepository<Discussion>, IDiscussionRepository
    {
        public DiscussionRepository(MongoDbContext context) : base(context, "discussions") { }

        public async Task<Discussion> GetByReviewIdAsync(string reviewId, CancellationToken cancellationToken = default)
        {
            var filter = Builders<Discussion>.Filter.Eq(d => d.ReviewId, ObjectId.Parse(reviewId));
            return await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IEnumerable<Discussion>> GetDiscussionsWithMessagesAsync(CancellationToken cancellationToken = default)
        {
            var filter = Builders<Discussion>.Filter.SizeGt("messages", 0);
            return await _collection.Find(filter).ToListAsync(cancellationToken);
        }

        public async Task<long> GetTotalMessageCountAsync(CancellationToken cancellationToken = default)
        {
            // MongoDB Aggregation для підрахунку всіх повідомлень
            var pipeline = new[]
            {
                new BsonDocument("$project", new BsonDocument
                {
                    { "messageCount", new BsonDocument("$size", "$messages") }
                }),
                new BsonDocument("$group", new BsonDocument
                {
                    { "_id", BsonNull.Value },
                    { "total", new BsonDocument("$sum", "$messageCount") }
                })
            };

            var result = await _collection.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync(cancellationToken);
            if (result == null)
                return 0;

            return result.GetValue("total", 0).ToInt64();
        }

        public async Task<IEnumerable<Discussion>> GetDiscussionsInPeriodAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        {
            var filter = Builders<Discussion>.Filter.And(
                Builders<Discussion>.Filter.Gte(d => d.CreatedAt, startDate),
                Builders<Discussion>.Filter.Lte(d => d.CreatedAt, endDate));

            return await _collection.Find(filter).ToListAsync(cancellationToken);
        }
    }
}

