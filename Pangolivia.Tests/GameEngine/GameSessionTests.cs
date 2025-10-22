using Pangolivia.API.DTOs;
using Pangolivia.API.GameEngine;
using Pangolivia.API.Models; // Add this using directive for QuizModel and QuestionModel

namespace Pangolivia.Tests.GameEngine;

public class GameSessionTests
{
    [Fact]
    public void Constructor_InitializesProperties()
    {
        // Arrange
        var quiz = new QuizModel { Id = 123, Questions = new List<QuestionModel>() };

        // Act
        var session = new GameSession(
            id: 1,
            name: "test-session",
            hostUserId: 42,
            hostUsername: "host",
            quiz: quiz
        );

        // Assert
        Assert.Equal(1, session.Id);
        Assert.Equal("test-session", session.Name);
        Assert.Equal(42, session.HostUserId);
        Assert.Equal("host", session.HostUsername);
        Assert.Same(quiz, session.Quiz);
        Assert.False(session.HasGameStarted());
    }

    [Fact]
    public void Constructor_Throws_WhenNameIsNull()
    {
        var quiz = new QuizModel { Id = 1, Questions = new List<QuestionModel>() };
        Assert.Throws<ArgumentNullException>(
            () => new GameSession(id: 1, name: null!, hostUserId: 1, hostUsername: "host", quiz: quiz)
        );
    }

    [Fact]
    public void Constructor_Throws_WhenQuizIsNull()
    {
        Assert.Throws<ArgumentNullException>(
            () => new GameSession(id: 1, name: "name", hostUserId: 1, hostUsername: "host", quiz: null!)
        );
    }

    [Fact]
    public void HasGameStarted_ReturnsFalse_WhenNoQuestionStarted()
    {
        var quiz = new QuizModel { Id = 1, Questions = new List<QuestionModel>() };
        var session = new GameSession(id: 1, name: "s", hostUserId: 1, hostUsername: "host", quiz: quiz);

        Assert.False(session.HasGameStarted());
    }

    [Fact]
    public void HasGameStarted_ReturnsTrue_WhenGameStarted()
    {
        // Arrange
        var question = new QuestionModel
        {
            Id = 11,
            QuizId = 1,
            QuestionText = "What is 2+2?",
            CorrectAnswer = "4",
            Answer2 = "3",
            Answer3 = "5",
            Answer4 = "22",
        };
        var quiz = new QuizModel { Id = 1, Questions = new List<QuestionModel> { question } };
        var session = new GameSession(id: 1, name: "s", hostUserId: 1, hostUsername: "host", quiz: quiz);

        // Act
        session.Start();

        // Assert
        Assert.True(session.HasGameStarted());
    }

    [Fact]
    public void RegisterPlayer_AddsPlayerSuccessfully()
    {
        // Arrange
        var quiz = new QuizModel { Id = 1, Questions = new List<QuestionModel>() };
        var session = new GameSession(id: 1, name: "s", hostUserId: 1, hostUsername: "host", quiz: quiz);
        var user = new UserDto { Id = 7, Username = "player7" };

        // Act
        var returned = session.RegisterPlayer(user, connectionId: "conn-1");

        // Assert
        Assert.Same(user, returned);
    }

    [Fact]
    public void RegisterPlayer_Throws_WhenDuplicate()
    {
        // Arrange
        var quiz = new QuizModel { Id = 1, Questions = new List<QuestionModel>() };
        var session = new GameSession(id: 1, name: "s", hostUserId: 1, hostUsername: "host", quiz: quiz);
        var user = new UserDto { Id = 7, Username = "player7" };
        session.RegisterPlayer(user, connectionId: "conn-1");

        // Act & Assert
        Assert.Throws<InvalidOperationException>(
            () => session.RegisterPlayer(user, connectionId: "conn-2")
        );
    }

    [Fact]
    public void AdvanceQuestion_Throws_WhenNoQuestions()
    {
        // Arrange
        var quiz = new QuizModel { Id = 1, Questions = new List<QuestionModel>() };
        var session = new GameSession(id: 1, name: "s", hostUserId: 1, hostUsername: "host", quiz: quiz);
        session.Start();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => session.AdvanceQuestion());
    }

    [Fact]
    public void AdvanceQuestion_ReturnsQuestionWithShuffledAnswers()
    {
        // Arrange
        var question = new QuestionModel
        {
            Id = 1,
            QuizId = 1,
            QuestionText = "Capital of France?",
            CorrectAnswer = "Paris",
            Answer2 = "London",
            Answer3 = "Rome",
            Answer4 = "Berlin",
        };
        var quiz = new QuizModel { Id = 1, Questions = new List<QuestionModel> { question } };
        var session = new GameSession(id: 1, name: "s", hostUserId: 1, hostUsername: "host", quiz: quiz);
        session.Start();
        Assert.True(session.HasNextQuestion());

        // Act
        QuestionForPlayerDto dto = session.AdvanceQuestion();

        // Assert
        Assert.True(session.HasGameStarted());
        Assert.Equal(question.QuestionText, dto.QuestionText);
        var answers = new[] { dto.Answer1, dto.Answer2, dto.Answer3, dto.Answer4 };
        Assert.Contains(question.CorrectAnswer, answers);
        Assert.Contains(question.Answer2, answers);
        Assert.Contains(question.Answer3, answers);
        Assert.Contains(question.Answer4, answers);
    }

    [Fact]
    public void AdvanceQuestion_Throws_WhenAlreadyActiveQuestion()
    {
        // Arrange
        var question = new QuestionModel
        {
            Id = 2,
            QuizId = 1,
            QuestionText = "1+1?",
            CorrectAnswer = "2",
            Answer2 = "1",
            Answer3 = "3",
            Answer4 = "4",
        };
        var quiz = new QuizModel { Id = 1, Questions = new List<QuestionModel> { question } };
        var session = new GameSession(id: 1, name: "s", hostUserId: 1, hostUsername: "host", quiz: quiz);
        session.Start();

        // Act
        session.AdvanceQuestion();

        // Assert
        Assert.Throws<InvalidOperationException>(() => session.AdvanceQuestion());
    }

    [Fact]
    public void AdvanceQuestion_Throws_WhenGameEnded()
    {
        // Arrange
        var question = new QuestionModel
        {
            Id = 99,
            QuizId = 1,
            QuestionText = "x",
            CorrectAnswer = "a",
            Answer2 = "b",
            Answer3 = "c",
            Answer4 = "d",
        };
        var quiz = new QuizModel { Id = 1, Questions = new List<QuestionModel> { question } };
        var session = new GameSession(id: 1, name: "s", hostUserId: 1, hostUsername: "host", quiz: quiz);
        var user = new UserDto { Id = 1, Username = "u" };
        session.RegisterPlayer(user, connectionId: "c");
        session.EndGameAndGetFinalGameRecord();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => session.AdvanceQuestion());
    }

    [Fact]
    public async Task AnswerQuestionForPlayer_CalculatesScoreBasedOnTime()
    {
        // Arrange
        var question = new QuestionModel
        {
            Id = 10,
            QuizId = 1,
            QuestionText = "2+2?",
            CorrectAnswer = "4",
            Answer2 = "3",
            Answer3 = "5",
            Answer4 = "22",
        };
        var quiz = new QuizModel { Id = 1, Questions = new List<QuestionModel> { question } };
        var session = new GameSession(id: 1, name: "s", hostUserId: 1, hostUsername: "host", quiz: quiz);
        var user1 = new UserDto { Id = 99, Username = "p99" };
        var user2 = new UserDto { Id = 100, Username = "p100" };
        session.RegisterPlayer(user1, connectionId: "c99");
        session.RegisterPlayer(user2, connectionId: "c100");
        session.Start();
        session.AdvanceQuestion();

        // Act
        await Task.Delay(2000); // Simulate user1 taking ~2 seconds
        session.AnswerQuestionForPlayer(user1.Id, "4");
        session.AnswerQuestionForPlayer(user2.Id, "5"); // Incorrect answer
        QuestionScoresDto roundResult = session.EndQuestionRound();

        // Assert
        PlayerQuestionScoresDto p1 = roundResult.PlayerScores.Single(ps => ps.UserId == user1.Id);
        PlayerQuestionScoresDto p2 = roundResult.PlayerScores.Single(ps => ps.UserId == user2.Id);
        // Score for ~2s delay: 1000 - ((1000-500) * (~2/10)) = ~900
        Assert.InRange(p1.Score, 899, 900);
        Assert.Equal(0, p2.Score);
    }

    [Fact]
    public void AnswerQuestionForPlayer_Throws_WhenPlayerNotRegistered()
    {
        // Arrange
        var quiz = new QuizModel { Id = 1, Questions = new List<QuestionModel>() };
        var session = new GameSession(id: 1, name: "s", hostUserId: 1, hostUsername: "host", quiz: quiz);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(
            () => session.AnswerQuestionForPlayer(playerId: 999, answer: "x")
        );
    }

    [Fact]
    public async Task EndQuestionRound_ReturnsCorrectAnswerAndPlayerScores()
    {
        // Arrange
        var question = new QuestionModel
        {
            Id = 30,
            QuizId = 1,
            QuestionText = "5+5?",
            CorrectAnswer = "10",
            Answer2 = "9",
            Answer3 = "11",
            Answer4 = "12",
        };

        var quiz = new QuizModel { Id = 1, Questions = new List<QuestionModel> { question } };
        var session = new GameSession(id: 1, name: "s", hostUserId: 1, hostUsername: "host", quiz: quiz);

        var alice = new UserDto { Id = 1, Username = "alice" };
        var bob = new UserDto { Id = 2, Username = "bob" };
        session.RegisterPlayer(alice, connectionId: "a");
        session.RegisterPlayer(bob, connectionId: "b");

        session.Start();
        session.AdvanceQuestion();
        await Task.Delay(5000); // Simulate Alice taking ~5 seconds
        session.AnswerQuestionForPlayer(alice.Id, "10");
        session.AnswerQuestionForPlayer(bob.Id, "11");

        // Act
        var result = session.EndQuestionRound();

        // Assert
        Assert.Equal(question.CorrectAnswer, result.Answer);
        var a = result.PlayerScores.Single(ps => ps.UserId == alice.Id);
        var b = result.PlayerScores.Single(ps => ps.UserId == bob.Id);
        // Score for ~5s delay: 1000 - ((1000-500) * (~5/10)) = ~750
        Assert.InRange(a.Score, 749, 750);
        Assert.InRange(a.TotalScore, 749, 750); // Total score after first round
        Assert.Equal(0, b.Score);
        Assert.Equal(0, b.TotalScore);
    }

    [Fact]
    public void EndQuestionRound_Throws_WhenNoActiveQuestion()
    {
        // Arrange
        var quiz = new QuizModel { Id = 1, Questions = new List<QuestionModel>() };
        var session = new GameSession(id: 1, name: "s", hostUserId: 1, hostUsername: "host", quiz: quiz);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => session.EndQuestionRound());
    }

    [Fact]
    public async Task EndGameAndGetFinalGameRecord_AwardsPointsAndReturnsFinalScores()
    {
        // Arrange
        var question1 = new QuestionModel
        {
            Id = 20,
            QuizId = 1,
            QuestionText = "3+2?",
            CorrectAnswer = "5",
            Answer2 = "4",
            Answer3 = "6",
            Answer4 = "7",
        };

        var question2 = new QuestionModel
        {
            Id = 21,
            QuizId = 1,
            QuestionText = "2+4?",
            CorrectAnswer = "6",
            Answer2 = "5",
            Answer3 = "7",
            Answer4 = "8",
        };

        var quiz = new QuizModel { Id = 1, Questions = new List<QuestionModel> { question1, question2 } };
        var session = new GameSession(id: 1, name: "s", hostUserId: 1, hostUsername: "host", quiz: quiz);

        var alice = new UserDto { Id = 1, Username = "alice" };
        var bob = new UserDto { Id = 2, Username = "bob" };
        session.RegisterPlayer(alice, connectionId: "a");
        session.RegisterPlayer(bob, connectionId: "b");

        // --- Round 1 ---
        session.Start();
        session.AdvanceQuestion();
        await Task.Delay(2000); // Alice answers after ~2s
        session.AnswerQuestionForPlayer(alice.Id, "5");
        session.AnswerQuestionForPlayer(bob.Id, "4"); // Bob answers wrong
        var round1 = session.EndQuestionRound();

        var r1a = round1.PlayerScores.Single(ps => ps.UserId == alice.Id);
        var r1b = round1.PlayerScores.Single(ps => ps.UserId == bob.Id);

        // Assert round 1 scores (with tolerance for timing)
        Assert.InRange(r1a.Score, 899, 900); // Score for ~2s delay
        Assert.Equal(0, r1b.Score);
        Assert.Equal(r1a.Score, r1a.TotalScore); // Total score is just round 1 score
        Assert.Equal(0, r1b.TotalScore);

        // --- Round 2 ---
        Assert.True(session.HasNextQuestion());
        session.AdvanceQuestion();
        await Task.Delay(1000); // Bob answers after ~1s
        session.AnswerQuestionForPlayer(bob.Id, "6");
        await Task.Delay(3000); // Alice answers after a total of ~4s
        session.AnswerQuestionForPlayer(alice.Id, "6");
        var round2 = session.EndQuestionRound();

        var r2a = round2.PlayerScores.Single(ps => ps.UserId == alice.Id);
        var r2b = round2.PlayerScores.Single(ps => ps.UserId == bob.Id);

        // Assert round 2 scores (with tolerance for timing)
        Assert.InRange(r2a.Score, 799, 800); // Score for ~4s delay
        Assert.InRange(r2b.Score, 949, 950); // Score for ~1s delay

        // Assert total scores are correctly accumulated
        Assert.Equal(r1a.TotalScore + r2a.Score, r2a.TotalScore);
        Assert.Equal(r1b.TotalScore + r2b.Score, r2b.TotalScore);

        // --- Act ---
        var final = session.EndGameAndGetFinalGameRecord();

        // --- Assert Final Scores ---
        var fa = final.PlayerScores.Single(p => p.UserId == alice.Id);
        var fb = final.PlayerScores.Single(p => p.UserId == bob.Id);

        // The final score should just be the total score from the last round
        Assert.Equal(r2a.TotalScore, fa.Score);
        Assert.Equal(r2b.TotalScore, fb.Score);
    }
}