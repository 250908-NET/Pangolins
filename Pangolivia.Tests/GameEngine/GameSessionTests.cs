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
}
