using Microsoft.EntityFrameworkCore;
using SoftMindApi.Data;
using SoftMindApi.Entities;
using SoftMindApi.Repositories.Interface;
using System.Collections.Generic;

namespace SoftMindApi.Repositories
{
    public class CategoryQuestionnaireRepository : ICategoryQuestionnaireRepository
    {
        private readonly MongoDbContext _context;

        public CategoryQuestionnaireRepository(MongoDbContext context)
        {
            _context = context;
        }

        //public async Task<CategoryQuestionnaire> AddCategoryQuestionnaire()
        //{

        //}

        public async Task<List<CategoryQuestionnaire>> GetCategoryQuestionnaires()
        {
            return await _context.CategoryQuestionnaire.ToListAsync();
        }
    }
}
