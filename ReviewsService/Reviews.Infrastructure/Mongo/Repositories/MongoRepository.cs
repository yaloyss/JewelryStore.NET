using MongoDB.Driver;
using Reviews.Domain.Common;
using Reviews.Domain.Interfaces;
using System.Linq.Expressions;
using MongoDB.Bson;

namespace Reviews.Infrastructure.Mongo.Repositories
{
    public class MongoRepository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly IMongoCollection<T> _collection;
        protected readonly MongoDbContext _context;

        public MongoRepository(MongoDbContext context, string collectionName)
        {
            _context = context;
            _collection = context.GetCollection<T>(collectionName);
        }

        public async Task<T> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            var filter = Builders<T>.Filter.Eq(e => e.Id, ObjectId.Parse(id));
            return await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _collection.Find(_ => true).ToListAsync(cancellationToken);
        }

        public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _collection.InsertOneAsync(entity, cancellationToken: cancellationToken);
            return entity;
        }

        public async Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            entity.MarkAsUpdated();
            var filter = Builders<T>.Filter.Eq(e => e.Id, entity.Id);
            await _collection.ReplaceOneAsync(filter, entity, cancellationToken: cancellationToken);
            return entity;
        }

        public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            var filter = Builders<T>.Filter.Eq(e => e.Id, ObjectId.Parse(id));
            var result = await _collection.DeleteOneAsync(filter, cancellationToken);
            return result.DeletedCount > 0;
        }

        public async Task<IEnumerable<T>> FindAsync( Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _collection.Find(predicate).ToListAsync(cancellationToken);
        }

        public async Task<T> FindOneAsync( Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _collection.Find(predicate).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IEnumerable<T>> GetPagedAsync(int pageNumber, int pageSize,CancellationToken cancellationToken = default)
        {
            return await _collection
                .Find(_ => true)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<T>> FindPagedAsync(Expression<Func<T, bool>> predicate, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            return await _collection
                .Find(predicate)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<long> CountAsync(CancellationToken cancellationToken = default)
        {
            return await _collection.CountDocumentsAsync(_ => true, cancellationToken: cancellationToken);
        }

        public async Task<long> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _collection.CountDocumentsAsync(predicate, cancellationToken: cancellationToken);
        }

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            var count = await _collection.CountDocumentsAsync(predicate, new CountOptions { Limit = 1 }, cancellationToken);
            return count > 0;
        }

        public async Task<bool> UpdateWithConcurrencyCheckAsync(T entity, DateTime expectedUpdatedAt, CancellationToken cancellationToken = default)
        {
            entity.MarkAsUpdated();

            // Optimistic Concurrency: оновлюємо тільки якщо UpdatedAt збігається
            var filter = Builders<T>.Filter.And(
                Builders<T>.Filter.Eq(e => e.Id, entity.Id),
                Builders<T>.Filter.Eq(e => e.UpdatedAt, expectedUpdatedAt)
            );

            var result = await _collection.ReplaceOneAsync(filter, entity, cancellationToken: cancellationToken);
            return result.ModifiedCount > 0;
        }
    }
}

