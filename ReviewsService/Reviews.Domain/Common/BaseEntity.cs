using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Reviews.Domain.Common
{
	public abstract class BaseEntity
	{
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; private set; }

        [BsonElement("createdAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreatedAt { get; private set; }

        [BsonElement("updatedAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime? UpdatedAt { get; private set; }

        protected BaseEntity()
        {
            Id = ObjectId.GenerateNewId();
            CreatedAt = DateTime.UtcNow;
        }

        protected BaseEntity(ObjectId id)
        {
            if (id == ObjectId.Empty)
                throw new ArgumentException("Id cannot be empty", nameof(id));

            Id = id;
            CreatedAt = DateTime.UtcNow;
        }

        public void MarkAsUpdated()
        {
            UpdatedAt = DateTime.UtcNow;
        }
    }
}

