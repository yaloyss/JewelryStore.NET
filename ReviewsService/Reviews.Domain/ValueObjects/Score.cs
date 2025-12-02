using MongoDB.Bson.Serialization.Attributes;
using Reviews.Domain.Common;
using Reviews.Domain.Exceptions;

namespace Reviews.Domain.ValueObjects
{
	public class Score : ValueObject
	{
        [BsonElement("value")]
        public int Value { get; private set; }

        private Score() { }

        public Score(int value)
        {
            if (value < 1 || value > 5)
                throw new ValidationException("Score must be between 1 and 5");

            Value = value;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString()
        {
            return $"{Value}/5";
        }
    }
}

