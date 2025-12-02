using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Reviews.Domain.Common;
using Reviews.Domain.Exceptions;
using Reviews.Domain.ValueObjects;

namespace Reviews.Domain.Entities
{
    [BsonIgnoreExtraElements]
    public class Review : BaseEntity
    {
        [BsonElement("productId")]
        public long ProductId { get; private set; }

        [BsonElement("rating")]
        public Rating Rating { get; private set; }

        [BsonElement("reviewText")]
        public ReviewText ReviewText { get; private set; }

        private Review() { }

        public Review(int productId, Rating rating, string title, string body) : base()
        {
            if (productId <= 0)
                throw new DomainException("ProductId must be greater than 0");
            if (rating == null)
                throw new DomainException("Rating is required");
            if (body == null)
                throw new DomainException("Review text is required");

            ProductId = productId;
            Rating = rating;
            ReviewText = new ReviewText(title, body);
        }

        public void UpdateReviewText(string title, string body)
        {
            ReviewText = new ReviewText(title, body);
            MarkAsUpdated();
        }

        public void UpdateRating(Rating newRating)
        {
            if (newRating == null)
                throw new DomainException("Rating is required");

            Rating = newRating;
            MarkAsUpdated();
        }

        public void ChangeProduct(int newProductId)
        {
            if (newProductId <= 0)
                throw new DomainException("ProductId must be greater than 0");

            ProductId = newProductId;
            MarkAsUpdated();
        }
    }
}
