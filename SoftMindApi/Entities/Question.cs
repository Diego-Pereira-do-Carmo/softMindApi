using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace SoftMindApi.Entities
{
    public class Question
    {
        public string? QuestionText { get; set; }
        public List<string>? ResponseOptions { get; set; }
    }
}
