using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Reviews.Domain.Entities;
using Reviews.Infrastructure.Mongo.Cofiguration;

namespace Reviews.Infrastructure.Mongo
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;
        private IClientSessionHandle _session;

        public MongoDbContext(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.DatabaseName);
        }

        public IMongoCollection<Rating> Ratings =>_database.GetCollection<Rating>("ratings");
        public IMongoCollection<Review> Reviews =>_database.GetCollection<Review>("reviews");
        public IMongoCollection<Discussion> Discussions =>_database.GetCollection<Discussion>("discussions");

        public IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            return _database.GetCollection<T>(collectionName);
        }

        public async Task<IClientSessionHandle> StartSessionAsync()
        {
            var client = _database.Client;_session = await client.StartSessionAsync();
            return _session;
        }
        public IClientSessionHandle Session => _session;

        public void StartTransaction()
        {
            _session?.StartTransaction();
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_session != null)
            {
                await _session.CommitTransactionAsync(cancellationToken);
            }
        }

        public async Task AbortTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_session != null)
            {
                await _session.AbortTransactionAsync(cancellationToken);
            }
        }

        public void Dispose()
        {
            _session?.Dispose();
        }
    }
}

