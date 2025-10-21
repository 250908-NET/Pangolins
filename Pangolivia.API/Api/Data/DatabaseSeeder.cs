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

            if (!context.Users.Any())
            {
                // Users
                var newUsers = new List<UserModel>
                {
                    new UserModel { AuthUuid = Guid.NewGuid().ToString(), Username = "Alice" ,PasswordHash = BCrypt.Net.BCrypt.HashPassword("password1") },
                    new UserModel { AuthUuid = Guid.NewGuid().ToString(), Username = "Bob" ,PasswordHash = BCrypt.Net.BCrypt.HashPassword("password2") },
                    new UserModel { AuthUuid = Guid.NewGuid().ToString(), Username = "Charlie" ,PasswordHash = BCrypt.Net.BCrypt.HashPassword("password3") },
                    new UserModel { AuthUuid = Guid.NewGuid().ToString(), Username = "Diana" ,PasswordHash = BCrypt.Net.BCrypt.HashPassword("password4") }
                };
                context.Users.AddRange(newUsers);
                context.SaveChanges();

            }

            var users = context.Users.ToList();

            // --- QUIZZES ---
            if (!context.Quizzes.Any())
            {
                // Quizzes
                var newQuizzes = new List<QuizModel>
                {
                    new QuizModel { QuizName = "General Knowledge", CreatedByUserId = users[0].Id },
                    new QuizModel { QuizName = "Science Quiz", CreatedByUserId = users[1].Id },
                    new QuizModel { QuizName = "History Trivia", CreatedByUserId = users[2].Id }
                };
                context.Quizzes.AddRange(newQuizzes);
                context.SaveChanges();
            }

            var quizzes = context.Quizzes.ToList();

            // --- QUESTIONS ---
            if (!context.Questions.Any())
            {
                var generalQuiz = quizzes.First(q => q.QuizName == "General Knowledge");
                var scienceQuiz = quizzes.First(q => q.QuizName == "Science Quiz");
                var historyQuiz = quizzes.First(q => q.QuizName == "History Trivia");

                var questions = new List<QuestionModel>
                {
                    // General Knowledge
                    new QuestionModel { QuizId = generalQuiz.Id, QuestionText = "What is the capital of France?", CorrectAnswer = "Paris", Answer2 = "London", Answer3 = "Berlin", Answer4 = "Madrid" },
                    new QuestionModel { QuizId = generalQuiz.Id, QuestionText = "Which planet is known as the Red Planet?", CorrectAnswer = "Mars", Answer2 = "Venus", Answer3 = "Jupiter", Answer4 = "Saturn" },
                    new QuestionModel { QuizId = generalQuiz.Id, QuestionText = "What is the primary goal of the player at the start of Stardew Valley?", CorrectAnswer = "Restore and manage a neglected farm inherited from their grandfather", Answer2 = "Build the largest house in Pelican Town", Answer3 = "Defeat monsters in the Skull Cavern", Answer4 = "Become the mayor of Stardew Valley" },
                    new QuestionModel { QuizId = generalQuiz.Id, QuestionText = "What is the largest ocean on Earth?", CorrectAnswer = "Pacific Ocean", Answer2 = "Atlantic Ocean", Answer3 = "Indian Ocean", Answer4 = "Arctic Ocean" },
                    new QuestionModel { QuizId = generalQuiz.Id, QuestionText = "What is the chemical symbol for Gold?", CorrectAnswer = "Au", Answer2 = "Ag", Answer3 = "Fe", Answer4 = "Cu" },

                    // Science Quiz
                    new QuestionModel { QuizId = scienceQuiz.Id, QuestionText = "What is the powerhouse of the cell?", CorrectAnswer = "Mitochondria", Answer2 = "Nucleus", Answer3 = "Ribosome", Answer4 = "Golgi apparatus" },
                    new QuestionModel { QuizId = scienceQuiz.Id, QuestionText = "Which gas do plants primarily take in for photosynthesis?", CorrectAnswer = "Carbon dioxide", Answer2 = "Oxygen", Answer3 = "Nitrogen", Answer4 = "Hydrogen" },
                    new QuestionModel { QuizId = scienceQuiz.Id, QuestionText = "What is the approximate speed of light in vacuum (m/s)?", CorrectAnswer = "299,792,458 m/s", Answer2 = "150,000,000 m/s", Answer3 = "3,000,000 m/s", Answer4 = "1,080,000,000 m/s" },
                    new QuestionModel { QuizId = scienceQuiz.Id, QuestionText = "Which particle has a negative charge?", CorrectAnswer = "Electron", Answer2 = "Proton", Answer3 = "Neutron", Answer4 = "Photon" },
                    new QuestionModel { QuizId = scienceQuiz.Id, QuestionText = "What is H2O commonly known as?", CorrectAnswer = "Water", Answer2 = "Hydrogen peroxide", Answer3 = "Salt", Answer4 = "Ammonia" },

                    // History Trivia
                    new QuestionModel { QuizId = historyQuiz.Id, QuestionText = "Who was the first President of the United States?", CorrectAnswer = "George Washington", Answer2 = "Thomas Jefferson", Answer3 = "John Adams", Answer4 = "Benjamin Franklin" },
                    new QuestionModel { QuizId = historyQuiz.Id, QuestionText = "In which year did World War II end?", CorrectAnswer = "1945", Answer2 = "1939", Answer3 = "1918", Answer4 = "1950" },
                    new QuestionModel { QuizId = historyQuiz.Id, QuestionText = "What was the ancient Egyptian writing system called?", CorrectAnswer = "Hieroglyphs", Answer2 = "Cuneiform", Answer3 = "Latin", Answer4 = "Runes" },
                    new QuestionModel { QuizId = historyQuiz.Id, QuestionText = "Which empire was founded by Genghis Khan?", CorrectAnswer = "Mongol Empire", Answer2 = "Ottoman Empire", Answer3 = "Roman Empire", Answer4 = "Persian Empire" },
                    new QuestionModel { QuizId = historyQuiz.Id, QuestionText = "What was the name of the ship that carried the Pilgrims to America in 1620?", CorrectAnswer = "Mayflower", Answer2 = "Santa Maria", Answer3 = "Golden Hind", Answer4 = "Beagle" }
                };
                context.Questions.AddRange(questions);
                context.SaveChanges();
            }

            // --- GAME RECORDS & PLAYER GAME RECORDS ---
            if (!context.GameRecords.Any() && !context.PlayerGameRecords.Any())
            {
                // Game Records
                var games = new List<GameRecordModel>
                {
                    new GameRecordModel
                    {
                        HostUserId = users[0].Id,
                        QuizId = quizzes[0].Id,
                        dateTimeCompleted = DateTime.UtcNow.AddDays(-1)
                    },
                    new GameRecordModel
                    {
                        HostUserId = users[1].Id,
                        QuizId = quizzes[1].Id,
                        dateTimeCompleted = DateTime.UtcNow.AddHours(-5)
                    },
                    new GameRecordModel
                    {
                        HostUserId = users[2].Id,
                        QuizId = quizzes[2].Id,
                        dateTimeCompleted = DateTime.UtcNow.AddHours(-2)
                    }
                };
                context.GameRecords.AddRange(games);
                context.SaveChanges();

                // Player Game Records
                var playerRecords = new List<PlayerGameRecordModel>
                {
                    new PlayerGameRecordModel { UserId = users[0].Id, GameRecordId = games[0].Id, Score = 85.5 },
                    new PlayerGameRecordModel { UserId = users[1].Id, GameRecordId = games[0].Id, Score = 90.0 },
                    new PlayerGameRecordModel { UserId = users[2].Id, GameRecordId = games[0].Id, Score = 70.0 },

                    new PlayerGameRecordModel { UserId = users[1].Id, GameRecordId = games[1].Id, Score = 92.5 },
                    new PlayerGameRecordModel { UserId = users[3].Id, GameRecordId = games[1].Id, Score = 88.0 },

                    new PlayerGameRecordModel { UserId = users[0].Id, GameRecordId = games[2].Id, Score = 75.0 },
                    new PlayerGameRecordModel { UserId = users[2].Id, GameRecordId = games[2].Id, Score = 82.0 },
                };
                context.PlayerGameRecords.AddRange(playerRecords);
                context.SaveChanges();
            }

        }
    }
}