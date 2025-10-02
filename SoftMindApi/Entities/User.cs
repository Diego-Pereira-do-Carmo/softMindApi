using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace SoftMindApi.Entities
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }
        public string AndroidId { get; set; }
    }
}
