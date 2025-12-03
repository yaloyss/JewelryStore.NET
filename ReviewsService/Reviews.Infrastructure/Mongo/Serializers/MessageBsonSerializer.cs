using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Reviews.Domain.ValueObjects;

namespace Reviews.Infrastructure.Mongo.Serializers
{
	public class MessageBsonSerializer : SerializerBase<Message>
    {
        public override Message Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            context.Reader.ReadStartDocument();

            string text = null;
            DateTime createdAt = DateTime.UtcNow;

            while (context.Reader.ReadBsonType() != BsonType.EndOfDocument)
            {
                var name = context.Reader.ReadName();

                switch (name)
                {
                    case "message":
                    case "text":
                        text = context.Reader.ReadString();
                        break;
                    case "createdAt":
                        var ms = context.Reader.ReadDateTime();
                        createdAt = DateTimeOffset.FromUnixTimeMilliseconds(ms).UtcDateTime;
                        break;
                    default:
                        context.Reader.SkipValue();
                        break;
                }
            }
            context.Reader.ReadEndDocument();
            var message = new Message(text);
            return message;
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Message value)
        {
            if (value == null)
            {
                context.Writer.WriteNull();
                return;
            }

            context.Writer.WriteStartDocument();
            context.Writer.WriteName("message");
            context.Writer.WriteString(value.Text);
            context.Writer.WriteName("createdAt");
            context.Writer.WriteDateTime((long)(value.CreatedAt.ToUniversalTime() - DateTime.UnixEpoch).TotalMilliseconds);
            context.Writer.WriteEndDocument();
        }
    }
}

