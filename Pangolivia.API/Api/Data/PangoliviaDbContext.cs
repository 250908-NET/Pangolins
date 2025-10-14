using Microsoft.EntityFrameworkCore;
using Pangolivia.Models;

namespace Pangolivia.Data
{
    public class PangoliviaDbContext : DbContext
    {
        public PangoliviaDbContext(DbContextOptions<PangoliviaDbContext> options)
            : base(options)
        {
        }

        // DbSets for each entity
        public DbSet<Quiz> Quizzes { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Quiz
            modelBuilder.Entity<Quiz>(entity =>
            {
                entity.ToTable("quiz");
                entity.HasKey(q => q.Id);
                entity.Property(q => q.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();
                entity.Property(q => q.QuizName)
                    .HasColumnName("quiz_name")
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(q => q.CreatedByUserId)
                    .HasColumnName("created_by_user_id");
                      
            });
        }
    }
}
