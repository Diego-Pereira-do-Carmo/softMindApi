using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using SoftMindApi.Entities;

namespace SoftMindApi.DTO
{
    public class CategoryQuestionnaireDTO
    {
        public string? Name { get; set; }
        public List<Question>? Questions { get; set; }
    }
}
