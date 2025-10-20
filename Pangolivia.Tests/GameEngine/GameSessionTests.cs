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
        var session = new GameSession(id: 1, name: "test-session", hostUserId: 42, quiz: quiz);

        // Assert
        Assert.Equal(1, session.Id);
        Assert.Equal("test-session", session.Name);
        Assert.Equal(42, session.HostUserId);
        Assert.Same(quiz, session.Quiz);
        Assert.False(session.HasGameStarted());
    }

    [Fact]
    public void Constructor_ThrowsWhenNameIsNull()
    {
        var quiz = new QuizModel { Id = 1, Questions = new List<QuestionModel>() };
        Assert.Throws<ArgumentNullException>(() => new GameSession(id: 1, name: null!, hostUserId: 1, quiz: quiz));
    }

    [Fact]
    public void Constructor_ThrowsWhenQuizIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new GameSession(id: 1, name: "name", hostUserId: 1, quiz: null!));
    }

    [Fact]
    public void HasGameStarted_ReturnsFalse_WhenNoQuestionStarted()
    {
        var quiz = new QuizModel { Id = 1, Questions = new List<QuestionModel>() };
        var session = new GameSession(id: 1, name: "s", hostUserId: 1, quiz: quiz);

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
        var session = new GameSession(id: 1, name: "s", hostUserId: 1, quiz: quiz);
        Assert.True(session.HasNextQuestion());

        // Act
        var q = session.AdvanceQuestion();

        // Assert
        Assert.True(session.HasGameStarted());
        Assert.NotNull(q);
        Assert.Equal(question.QuestionText, q.QuestionText);
        Assert.False(session.HasNextQuestion());
    }

    [Fact]
    public void RegisterPlayer_AddsPlayerSuccessfully()
    {
        // Arrange
        var quiz = new QuizModel { Id = 1, Questions = new List<QuestionModel>() };
        var session = new GameSession(id: 1, name: "s", hostUserId: 1, quiz: quiz);
        var user = new UserDto { Id = 7, Username = "player7" };

        // Act
        var returned = session.RegisterPlayer(user, connectionId: "conn-1");

        // Assert
        Assert.Same(user, returned);
    }

    [Fact]
    public void RegisterPlayer_ThrowsWhenDuplicate()
    {
        // Arrange
        var quiz = new QuizModel { Id = 1, Questions = new List<QuestionModel>() };
        var session = new GameSession(id: 1, name: "s", hostUserId: 1, quiz: quiz);
        var user = new UserDto { Id = 7, Username = "player7" };
        session.RegisterPlayer(user, connectionId: "conn-1");

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => session.RegisterPlayer(user, connectionId: "conn-2"));
    }

    [Fact]
    public void AdvanceQuestion_ThrowsWhenNoQuestions()
    {
        // Arrange
        var quiz = new QuizModel { Id = 1, Questions = new List<QuestionModel>() };
        var session = new GameSession(id: 1, name: "s", hostUserId: 1, quiz: quiz);

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
        var session = new GameSession(id: 1, name: "s", hostUserId: 1, quiz: quiz);

        // Precondition
        Assert.True(session.HasNextQuestion());

        // Act
        var dto = session.AdvanceQuestion();

        // Assert
        Assert.True(session.HasGameStarted());
        Assert.Equal(question.QuestionText, dto.QuestionText);
        var answers = new[] { dto.Answer1, dto.Answer2, dto.Answer3, dto.Answer4 };
        // All four original answers must be present after shuffle
        Assert.Contains(question.CorrectAnswer, answers);
        Assert.Contains(question.Answer2, answers);
        Assert.Contains(question.Answer3, answers);
        Assert.Contains(question.Answer4, answers);
    }

    [Fact]
    public void AdvanceQuestion_ThrowsWhenActiveQuestion()
    {
        // Arrange: single question quiz
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
        var session = new GameSession(id: 1, name: "s", hostUserId: 1, quiz: quiz);

        // Act: advance to start the active question
        var dto = session.AdvanceQuestion();

        // Attempting to advance again while active should throw
        Assert.Throws<InvalidOperationException>(() => session.AdvanceQuestion());
    }
}
