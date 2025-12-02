using MongoDB.Bson.Serialization.Attributes;
using Reviews.Domain.Common;
using Reviews.Domain.Exceptions;

namespace Reviews.Domain.ValueObjects
{
	public class ReviewText : ValueObject
	{
        [BsonElement("title")]
        public string Title { get; private set; }

        [BsonElement("body")]
        public string Body { get; private set; }

        private ReviewText() { }

        public ReviewText(string title, string body)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new DomainException("Title cannot be empty.");

            if (title.Length > 200)
                throw new DomainException("Title cannot exceed 200 characters.");

            if (string.IsNullOrWhiteSpace(body))
                throw new DomainException("Review body cannot be empty.");

            if (body.Length < 10)
                throw new DomainException("Review body must be at least 10 characters long.");

            if (body.Length > 2000)
                throw new DomainException("Review body cannot exceed 2000 characters.");

            Title = title.Trim();
            Body = body.Trim();
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Title;
            yield return Body;
        }

        public override string ToString() => $"{Title}: {Body.Substring(0, Math.Min(50, Body.Length))}...";
    }
}

