using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace SoftMindApi.Entities
{
    public class CategoryQuestionnaire
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }
        public string? Name { get; set; }
        public List<Question>? Questions { get; set; }
    }
}
