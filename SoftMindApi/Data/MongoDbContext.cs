using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;
using SoftMindApi.Entities;

namespace SoftMindApi.Data
{
    public class MongoDbContext : DbContext
    {
        public DbSet<CategoryQuestionnaire> CategoryQuestionnaire { get; init; }
        public DbSet<ResponseQuestionnaire> ResponseQuestionnaire { get; init; }
        public DbSet<WellnessMessage> WellnessMessages { get; init; }

        public MongoDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CategoryQuestionnaire>().ToCollection("CategoryQuestionnaire");
            modelBuilder.Entity<CategoryQuestionnaire>().OwnsMany(c => c.Questions);

            modelBuilder.Entity<ResponseQuestionnaire>().ToCollection("ResponseQuestionnaire");

            modelBuilder.Entity<WellnessMessage>().ToCollection("WellnessMessages");
            modelBuilder.Entity<WellnessMessage>().OwnsMany(w => w.ReadStats);
        }
    }
}
