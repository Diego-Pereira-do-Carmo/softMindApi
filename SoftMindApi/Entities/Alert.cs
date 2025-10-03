using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace SoftMindApi.Entities
{
    public class Alert
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }   

        [BsonElement("DeviceId")]
        public string DeviceId { get; set; } = string.Empty;

        [BsonElement("Message")]
        public string Message { get; set; } = string.Empty;

        [BsonElement("Category")]
        public string Category { get; set; } = string.Empty;

        [BsonElement("CreatedAt")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("IsRead")]
        public bool IsRead { get; set; } = false;
    }
}
