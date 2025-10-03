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
        public DbSet<User> User { get; init; }
        public DbSet<Mood> Mood { get; init; }
        public DbSet<Alert> Alert { get; set; }

        public MongoDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CategoryQuestionnaire>().ToCollection("CategoryQuestionnaire");
            modelBuilder.Entity<CategoryQuestionnaire>().OwnsMany(c => c.Questions);

            modelBuilder.Entity<WellnessMessage>().ToCollection("WellnessMessages");
            modelBuilder.Entity<WellnessMessage>().OwnsMany(w => w.ReadStats);

            modelBuilder.Entity<ResponseQuestionnaire>().ToCollection("ResponseQuestionnaire");
            modelBuilder.Entity<User>().ToCollection("User");
            modelBuilder.Entity<Mood>().ToCollection("Mood");
            modelBuilder.Entity<Alert>().ToCollection("Alert");
        }
    }
}
