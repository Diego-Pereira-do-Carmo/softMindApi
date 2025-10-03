using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SoftMindApi.Entities
{
    public class AlertTemplate
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("Message")]
        public string Message { get; set; } = string.Empty;

        [BsonElement("Category")]
        public string Category { get; set; } = string.Empty;
    }
}
