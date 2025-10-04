using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SoftMindApi.Entities
{
    public class ResponseQuestionnaire
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        public string DeviceId { get; set; } = string.Empty;

        public string pergunta { get; set; } = string.Empty;
        public string resposta { get; set; } = string.Empty;
        public DateTime Data { get; set; }
    }
}
