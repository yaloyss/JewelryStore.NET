using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Reviews.Domain.Common;
using Reviews.Domain.Exceptions;
using Reviews.Domain.ValueObjects;

namespace Reviews.Domain.Entities
{
    [BsonIgnoreExtraElements]
    public class Discussion : BaseEntity
    {
        private readonly List<Message> _messages;

        [BsonElement("reviewId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId ReviewId { get; private set; }

        [BsonElement("messages")]
        public IReadOnlyList<Message> Messages => _messages.AsReadOnly();

        private Discussion()
        {
            _messages = new List<Message>();
        }

        public Discussion(ObjectId reviewId) : base()
        {
            if (reviewId == ObjectId.Empty)
                throw new ValidationException("ReviewId cannot be empty");

            ReviewId = reviewId;
            _messages = new List<Message>();
        }

        public void AddMessage(string messageText)
        {
            var message = new Message(messageText);
            _messages.Add(message);
            MarkAsUpdated();
        }

        public void RemoveLastMessage()
        {
            if (_messages.Count == 0)
                throw new ValidationException("Cannot remove message from empty discussion");

            _messages.RemoveAt(_messages.Count - 1);
            MarkAsUpdated();
        }

        public int GetMessagesCount() => _messages.Count;

        public bool HasMessages() => _messages.Any();
    }
}

