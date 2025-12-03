using MongoDB.Bson;
using MongoDB.Driver;
using Reviews.Domain.Entities;
using Reviews.Domain.Interfaces;

namespace Reviews.Infrastructure.Mongo.Repositories
{
    public class RatingRepository : MongoRepository<Rating>, IRatingRepository
    {
        public RatingRepository(MongoDbContext context) : base(context, "ratings") { }

        public async Task<IEnumerable<Rating>> GetByScoreAsync(int score, CancellationToken cancellationToken = default)
        {
            var filter = Builders<Rating>.Filter.Eq("score.value", score);
            return await _collection.Find(filter).ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Rating>> GetHighRatingsAsync(CancellationToken cancellationToken = default)
        {
            var filter = Builders<Rating>.Filter.Gte("score.value", 4);
            return await _collection.Find(filter).ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Rating>> GetLowRatingsAsync(CancellationToken cancellationToken = default)
        {
            var filter = Builders<Rating>.Filter.Lte("score.value", 2);
            return await _collection.Find(filter).ToListAsync(cancellationToken);
        }

        public async Task<double> GetAverageScoreAsync(CancellationToken cancellationToken = default)
        {
            // MongoDB Aggregation Pipeline для середньої оцінки
            var pipeline = new[]
            {
                new BsonDocument("$group", new BsonDocument
                {
                    { "_id", BsonNull.Value },
                    { "avgScore", new BsonDocument("$avg", "$score.value") }
                })
            };

            var result = await _collection.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync(cancellationToken);
            if (result == null)
                return 0.0;

            return result.GetValue("avgScore", 0.0).AsDouble;
        }
    }
}

