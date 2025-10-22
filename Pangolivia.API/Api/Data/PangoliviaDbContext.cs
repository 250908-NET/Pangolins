using Microsoft.EntityFrameworkCore;
using Pangolivia.API.Models;

namespace Pangolivia.API.Data;

public class PangoliviaDbContext : DbContext
{
    public PangoliviaDbContext(DbContextOptions<PangoliviaDbContext> options)
        : base(options) { }

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
            entity
                .HasOne(gr => gr.HostUser)
                .WithMany(u => u.HostedGameRecords) // This points to the ICollection in UserModel
                .HasForeignKey(gr => gr.HostUserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasOne(gr => gr.Quiz)
                .WithMany(q => q.GameRecords) // This points to the ICollection in QuizModel
                .HasForeignKey(gr => gr.QuizId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        modelBuilder.Entity<PlayerGameRecordModel>(entity =>
        {
            entity
                .HasOne(pgr => pgr.GameRecord)
                .WithMany(gr => gr.PlayerGameRecords) // This points to the ICollection in GameRecordModel
                .HasForeignKey(pgr => pgr.GameRecordId)
                .OnDelete(DeleteBehavior.Cascade);

            entity
                .HasOne(pgr => pgr.User)
                .WithMany(u => u.PlayerGameRecords) // This points to the ICollection in UserModel
                .HasForeignKey(pgr => pgr.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        modelBuilder.Entity<QuizModel>(entity =>
        {
            entity
                .HasOne(q => q.CreatedByUser)
                .WithMany(u => u.CreatedQuizzes) // This points to the ICollection in UserModel
                .HasForeignKey(q => q.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        modelBuilder.Entity<QuestionModel>(entity =>
        {
            entity
                .HasOne(q => q.Quiz)
                .WithMany(qz => qz.Questions) // This points to the ICollection in QuizModel
                .HasForeignKey(q => q.QuizId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // The following block was redundant and contained a conflicting OnDelete behavior.
        // It has been removed to resolve the "multiple cascade paths" error.
        // modelBuilder.Entity<PlayerGameRecordModel>()
        //     .HasKey(pgr => pgr.Id);

        // modelBuilder.Entity<PlayerGameRecordModel>()
        //     .HasOne(pgr => pgr.User)
        //     .WithMany(u => u.PlayerGameRecords)
        //     .HasForeignKey(pgr => pgr.UserId)
        //     .OnDelete(DeleteBehavior.Cascade);

        // modelBuilder.Entity<PlayerGameRecordModel>()
        //     .HasOne(pgr => pgr.GameRecord)
        //     .WithMany(gr => gr.PlayerGameRecords)
        //     .HasForeignKey(pgr => pgr.GameRecordId)
        //     .OnDelete(DeleteBehavior.Cascade);

        base.OnModelCreating(modelBuilder);
    }
}
