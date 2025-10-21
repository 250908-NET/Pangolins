using System;
using System.Linq;
using System.Collections.Generic;
using Pangolivia.API.Models;
using Pangolivia.API.Data; 

namespace Pangolivia.API.Data
{
    public static class DbSeeder
    {
        public static void Seed(PangoliviaDbContext context)
        {
            if (context.Users.Any()) return;

            // Users
            var users = new List<UserModel>
            {
                new UserModel { AuthUuid = Guid.NewGuid().ToString(), Username = "Alice" ,PasswordHash = BCrypt.Net.BCrypt.HashPassword("password1") },
                new UserModel { AuthUuid = Guid.NewGuid().ToString(), Username = "Bob" ,PasswordHash = BCrypt.Net.BCrypt.HashPassword("password2") },
                new UserModel { AuthUuid = Guid.NewGuid().ToString(), Username = "Charlie" ,PasswordHash = BCrypt.Net.BCrypt.HashPassword("password3") },
                new UserModel { AuthUuid = Guid.NewGuid().ToString(), Username = "Diana" ,PasswordHash = BCrypt.Net.BCrypt.HashPassword("password4") }
            };
            context.Users.AddRange(users);
            context.SaveChanges();

            // Quizzes
            var quizzes = new List<QuizModel>
            {
                new QuizModel { QuizName = "General Knowledge", CreatedByUserId = users[0].Id },
                new QuizModel { QuizName = "Science Quiz", CreatedByUserId = users[1].Id },
                new QuizModel { QuizName = "History Trivia", CreatedByUserId = users[2].Id }
            };
            context.Quizzes.AddRange(quizzes);
            context.SaveChanges();

            // Game Records
            var games = new List<GameRecordModel>
            {
                new GameRecordModel
                {
                    HostUserId = users[0].Id,
                    QuizId = quizzes[0].Id,
                    datetimeCompleted = DateTime.UtcNow.AddDays(-1)
                },
                new GameRecordModel
                {
                    HostUserId = users[1].Id,
                    QuizId = quizzes[1].Id,
                    datetimeCompleted = DateTime.UtcNow.AddHours(-5)
                },
                new GameRecordModel
                {
                    HostUserId = users[2].Id,
                    QuizId = quizzes[2].Id,
                    datetimeCompleted = DateTime.UtcNow.AddHours(-2)
                }
            };
            context.GameRecords.AddRange(games);
            context.SaveChanges();

            // Player Game Records
            var playerRecords = new List<PlayerGameRecordModel>
            {
                new PlayerGameRecordModel { UserId = users[0].Id, GameRecordId = games[0].Id, score = 85.5 },
                new PlayerGameRecordModel { UserId = users[1].Id, GameRecordId = games[0].Id, score = 90.0 },
                new PlayerGameRecordModel { UserId = users[2].Id, GameRecordId = games[0].Id, score = 70.0 },

                new PlayerGameRecordModel { UserId = users[1].Id, GameRecordId = games[1].Id, score = 92.5 },
                new PlayerGameRecordModel { UserId = users[3].Id, GameRecordId = games[1].Id, score = 88.0 },

                new PlayerGameRecordModel { UserId = users[0].Id, GameRecordId = games[2].Id, score = 75.0 },
                new PlayerGameRecordModel { UserId = users[2].Id, GameRecordId = games[2].Id, score = 82.0 },
            };
            context.PlayerGameRecords.AddRange(playerRecords);
            context.SaveChanges();
        }
    }
}
