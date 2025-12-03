using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Reviews.Domain.ValueObjects;

namespace Reviews.Infrastructure.Mongo.Serializers
{
	public class ScoreBsonSerializer : SerializerBase<Score>
    {
        public override Score Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var type = context.Reader.GetCurrentBsonType();

            if (type == BsonType.Document)
            {
                context.Reader.ReadStartDocument();
                context.Reader.ReadName("value");
                var value = context.Reader.ReadInt32();
                context.Reader.ReadEndDocument();

                return new Score(value);
            }
            else if (type == BsonType.Int32)
            {
                var value = context.Reader.ReadInt32();
                return new Score(value);
            }

            throw new BsonSerializationException($"Cannot deserialize Score from BsonType {type}");
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Score value)
        {
            if (value == null)
            {
                context.Writer.WriteNull();
                return;
            }

            context.Writer.WriteStartDocument();
            context.Writer.WriteName("value");
            context.Writer.WriteInt32(value.Value);
            context.Writer.WriteEndDocument();
        }
    }
}

