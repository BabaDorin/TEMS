using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace UserManagement.Infrastructure.Serialization;

public class FlexibleStringObjectIdSerializer : SerializerBase<string>
{
    public override string Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        return context.Reader.CurrentBsonType switch
        {
            BsonType.ObjectId => context.Reader.ReadObjectId().ToString(),
            BsonType.String => context.Reader.ReadString(),
            BsonType.Null => ReadNull(context),
            _ => throw new FormatException(
                $"Cannot deserialize a string id from BSON type '{context.Reader.CurrentBsonType}'.")
        };
    }

    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            context.Writer.WriteNull();
            return;
        }

        if (ObjectId.TryParse(value, out var objectId))
        {
            context.Writer.WriteObjectId(objectId);
            return;
        }

        context.Writer.WriteString(value);
    }

    private static string ReadNull(BsonDeserializationContext context)
    {
        context.Reader.ReadNull();
        return string.Empty;
    }
}
