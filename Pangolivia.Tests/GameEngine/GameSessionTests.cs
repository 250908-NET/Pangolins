using Pangolivia.API.DTOs;
using Pangolivia.API.GameEngine;

namespace Pangolivia.Tests.GameEngine;

public class GameSessionTests
{
    [Fact]
    public void Constructor_InitializesProperties()
    {
        // Arrange
        var quiz = new QuizModel { Id = 123, Questions = new List<QuestionModel>() };

        // Act
        var session = new GameSession(id: 1, name: "test-session", hostUserId: 42, "host", quiz: quiz);

        // Assert
        Assert.Equal(1, session.Id);
        Assert.Equal("test-session", session.Name);
        Assert.Equal(42, session.HostUserId);
        Assert.Same(quiz, session.Quiz);
        Assert.False(session.HasGameStarted());
    }

    [Fact]
    public void Constructor_Throws_WhenNameIsNull()
    {
        var quiz = new QuizModel { Id = 1, Questions = new List<QuestionModel>() };
        Assert.Throws<ArgumentNullException>(() => new GameSession(id: 1, name: null!, hostUserId: 1, "host", quiz: quiz));
    }

    [Fact]
    public void Constructor_Throws_WhenQuizIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new GameSession(id: 1, name: "name", hostUserId: 1, "host", quiz: null!));
    }

    [Fact]
    public void HasGameStarted_ReturnsFalse_WhenNoQuestionStarted()
    {
        var quiz = new QuizModel { Id = 1, Questions = new List<QuestionModel>() };
        var session = new GameSession(id: 1, name: "s", hostUserId: 1, "host", quiz: quiz);

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
        var session = new GameSession(id: 1, name: "s", hostUserId: 1, "host", quiz: quiz);

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
        var session = new GameSession(id: 1, name: "s", hostUserId: 1, "host", quiz: quiz);
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
        var session = new GameSession(id: 1, name: "s", hostUserId: 1, "host", quiz: quiz);
        var user = new UserDto { Id = 7, Username = "player7" };
        session.RegisterPlayer(user, connectionId: "conn-1");

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => session.RegisterPlayer(user, connectionId: "conn-2"));
    }

    [Fact]
    public void AdvanceQuestion_Throws_WhenNoQuestions()
    {
        // Arrange
        var quiz = new QuizModel { Id = 1, Questions = new List<QuestionModel>() };
        var session = new GameSession(id: 1, name: "s", hostUserId: 1, "host", quiz: quiz);
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
        var session = new GameSession(id: 1, name: "s", hostUserId: 1, "host", quiz: quiz);
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
        var session = new GameSession(id: 1, name: "s", hostUserId: 1, "host", quiz: quiz);
        session.Start();

        // Act
        QuestionForPlayerDto dto = session.AdvanceQuestion();

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
        var session = new GameSession(id: 1, name: "s", hostUserId: 1, "host", quiz: quiz);
        var user = new UserDto { Id = 1, Username = "u" };
        session.RegisterPlayer(user, connectionId: "c");
        session.EndGameAndGetFinalGameRecord();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => session.AdvanceQuestion());
    }

    [Fact]
    public void AnswerQuestionForPlayer_SetsAnswerSuccessfully()
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
        var session = new GameSession(id: 1, name: "s", hostUserId: 1, "host", quiz: quiz);
        var user1 = new UserDto { Id = 99, Username = "p99" };
        var user2 = new UserDto { Id = 100, Username = "p100" };
        session.RegisterPlayer(user1, connectionId: "c99");
        session.RegisterPlayer(user2, connectionId: "c100");
        session.Start();
        QuestionForPlayerDto dto = session.AdvanceQuestion();

        // Act
        session.AnswerQuestionForPlayer(user1.Id, "4");
        session.AnswerQuestionForPlayer(user2.Id, "5");
        QuestionScoresDto roundResult = session.EndQuestionRound();

        // Assert
        PlayerQuestionScoresDto p1 = roundResult.PlayerScores.Single(ps => ps.UserId == user1.Id);
        PlayerQuestionScoresDto p2 = roundResult.PlayerScores.Single(ps => ps.UserId == user2.Id);
        Assert.Equal(1, p1.Score);
        Assert.Equal(0, p2.Score);
    }

    [Fact]
    public void AnswerQuestionForPlayer_Throws_WhenPlayerNotRegistered()
    {
        // Arrange
        var quiz = new QuizModel { Id = 1, Questions = new List<QuestionModel>() };
        var session = new GameSession(id: 1, name: "s", hostUserId: 1, "host", quiz: quiz);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => session.AnswerQuestionForPlayer(playerId: 999, answer: "x"));
    }

    [Fact]
    public void EndQuestionRound_ReturnsCorrectAnswerAndPlayerScores()
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
        var session = new GameSession(id: 1, name: "s", hostUserId: 1, "host", quiz: quiz);

        var alice = new UserDto { Id = 1, Username = "alice" };
        var bob = new UserDto { Id = 2, Username = "bob" };
        session.RegisterPlayer(alice, connectionId: "a");
        session.RegisterPlayer(bob, connectionId: "b");

        session.Start();
        QuestionForPlayerDto dto = session.AdvanceQuestion();
        session.AnswerQuestionForPlayer(alice.Id, "10");
        session.AnswerQuestionForPlayer(bob.Id, "11");

        // Act
        var result = session.EndQuestionRound();

        // Assert
        Assert.Equal(question.CorrectAnswer, result.Answer);
        var a = result.PlayerScores.Single(ps => ps.UserId == alice.Id);
        var b = result.PlayerScores.Single(ps => ps.UserId == bob.Id);
        Assert.Equal(1, a.Score);
        Assert.Equal(0, b.Score);
    }

    [Fact]
    public void EndQuestionRound_Throws_WhenNoActiveQuestion()
    {
        // Arrange
        var quiz = new QuizModel { Id = 1, Questions = new List<QuestionModel>() };
        var session = new GameSession(id: 1, name: "s", hostUserId: 1, "host", quiz: quiz);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => session.EndQuestionRound());
    }

    [Fact]
    public void EndGameAndGetFinalGameRecord_AwardsPointsAndReturnsFinalScores()
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
        var session = new GameSession(id: 1, name: "s", hostUserId: 1, "host", quiz: quiz);

        var alice = new UserDto { Id = 1, Username = "alice" };
        var bob = new UserDto { Id = 2, Username = "bob" };
        session.RegisterPlayer(alice, connectionId: "a");
        session.RegisterPlayer(bob, connectionId: "b");

        // Round 1
        session.Start();
        QuestionForPlayerDto dto = session.AdvanceQuestion();
        session.AnswerQuestionForPlayer(alice.Id, "5");
        session.AnswerQuestionForPlayer(bob.Id, "4");
        var round1 = session.EndQuestionRound();

        var r1a = round1.PlayerScores.Single(ps => ps.UserId == alice.Id);
        var r1b = round1.PlayerScores.Single(ps => ps.UserId == bob.Id);
        Assert.Equal(1, r1a.Score);
        Assert.Equal(0, r1b.Score);

        // Round 2
        Assert.True(session.HasNextQuestion());
        session.AdvanceQuestion();
        session.AnswerQuestionForPlayer(alice.Id, "6");
        session.AnswerQuestionForPlayer(bob.Id, "6");
        var round2 = session.EndQuestionRound();

        var r2a = round2.PlayerScores.Single(ps => ps.UserId == alice.Id);
        var r2b = round2.PlayerScores.Single(ps => ps.UserId == bob.Id);
        Assert.Equal(1, r2a.Score);
        Assert.Equal(1, r2b.Score);

        // Act
        var final = session.EndGameAndGetFinalGameRecord();

        // Assert
        var fa = final.PlayerScores.Single(p => p.UserId == alice.Id);
        var fb = final.PlayerScores.Single(p => p.UserId == bob.Id);
        Assert.Equal(2, fa.Score);
        Assert.Equal(1, fb.Score);
    }
}
