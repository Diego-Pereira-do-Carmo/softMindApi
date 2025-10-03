using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace SoftMindApi.Entities
{
    public class Mood
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public string DeviceId { get; set; }
        public DateTime Data { get; set; }
    }
}
