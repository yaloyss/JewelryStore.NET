using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Reviews.Domain.Common;
using Reviews.Domain.Exceptions;
using Reviews.Domain.ValueObjects;

namespace Reviews.Domain.Entities;

[BsonIgnoreExtraElements]
public class Rating : BaseEntity
{
    [BsonElement("score")]
    public Score Score { get; private set; }

    private Rating() : base() { }

    public Rating(int score) : base()
    {
        Score = new Score(score);
    }

    public Rating(Score score) : base()
    {
        Score = score ?? throw new DomainException("Score is required");
    }

    public void UpdateScore(int newScore)
    {
        if (newScore < 1 || newScore > 5)
            throw new ValidationException("Score must be between 1 and 5");

        Score = new Score(newScore);
        MarkAsUpdated();
    }
}