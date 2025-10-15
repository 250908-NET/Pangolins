using Microsoft.EntityFrameworkCore;
using Pangolivia.API.Models;

namespace Pangolivia.API.Data;

public class PangoliviaDbContext : DbContext
{
    public PangoliviaDbContext(DbContextOptions<PangoliviaDbContext> options) : base(options) { }

    // DbSets for each entity
    public DbSet<UserModel> Users { get; set; }
    public DbSet<GameRecordModel> GameRecords { get; set; }
    public DbSet<PlayerGameRecordModel> PlayerGameRecords { get; set; }
    public DbSet<QuizModel> Quizzes { get; set; }
    public DbSet<QuestionModel> Questions { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // modelBuilder.Entity<UserModel>(entity =>
        // {
            
        // });
        modelBuilder.Entity<GameRecordModel>(entity =>
        {
            entity.HasOne(gr => gr.HostUser)
                .WithMany() // If you add a collection navigation property to UserModel, put it here
                .HasForeignKey(gr => gr.HostUserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(gr => gr.Quiz)
                .WithMany() // If you add a collection navigation property to QuizModel, put it here
                .HasForeignKey(gr => gr.QuizId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        modelBuilder.Entity<PlayerGameRecordModel>(entity =>
        {
            entity.HasOne(pgr => pgr.GameRecord)
                .WithMany() // If you add a collection navigation property to QuizModel, put it here
                .HasForeignKey(pgr => pgr.GameRecordId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(pgr => pgr.User)
                .WithMany() // If you add a collection navigation property to UserModel, put it here
                .HasForeignKey(pgr => pgr.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        modelBuilder.Entity<QuizModel>(entity =>
        {
            entity.HasOne(q => q.CreatedByUser)
                .WithMany() // If you add a collection navigation property to UserModel, put it here
                .HasForeignKey(q => q.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        modelBuilder.Entity<QuestionModel>(entity =>
        {
            entity.HasOne(q => q.Quiz)
                .WithMany() // If you add a collection navigation property to UserModel, put it here
                .HasForeignKey(q => q.QuizId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        
        base.OnModelCreating(modelBuilder);
    }
}

