using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SoftMindApi.Entities
{
    public class WellnessMessage
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public bool Active { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<WellnessReadStat> ReadStats { get; set; } = new();
    }

    public class WellnessReadStat
    {
        public string DeviceId { get; set; } = string.Empty;
        public int Count { get; set; } = 0;
        public DateTime LastReadAt { get; set; } = DateTime.MinValue;
    }
}
