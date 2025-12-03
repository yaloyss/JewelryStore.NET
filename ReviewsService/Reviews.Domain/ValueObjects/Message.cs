using MongoDB.Bson.Serialization.Attributes;
using Reviews.Domain.Common;
using Reviews.Domain.Exceptions;

namespace Reviews.Domain.ValueObjects
{
	public class Message : ValueObject
	{
        [BsonElement("text")]
        public string Text { get; private set; }

        [BsonElement("createdAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreatedAt { get; private set; }

        [BsonConstructor]
        public Message() { }

        public Message(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ValidationException("Message text cannot be empty");

            if (text.Length > 500)
                throw new DomainException("Message text cannot exceed 500 characters");

            Text = text.Trim();
            CreatedAt = DateTime.UtcNow;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Text;
            yield return CreatedAt;
        }

        public override string ToString()
        {
            return Text;
        }
    }
}

