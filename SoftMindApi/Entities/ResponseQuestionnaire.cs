using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SoftMindApi.Entities
{
    public class ResponseQuestionnaire
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        public string UserId { get; set; }

        public string pergunta { get; set; }
        public string resposta { get; set; }
        public DateTime Data { get; set; }
    }
}
