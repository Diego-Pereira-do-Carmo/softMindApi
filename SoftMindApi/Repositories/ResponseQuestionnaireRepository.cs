using SoftMindApi.Data;
using SoftMindApi.Entities;
using SoftMindApi.Repositories.Interface;

namespace SoftMindApi.Repositories
{
    public class ResponseQuestionnaireRepository : IResponseQuestionnaireRepository
    {
        private readonly MongoDbContext _context;

        public ResponseQuestionnaireRepository(MongoDbContext context)
        {
            _context = context;
        }

        public async Task AddRangeAsync(IEnumerable<ResponseQuestionnaire> responses)
        {
            await _context.ResponseQuestionnaire.AddRangeAsync(responses);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
