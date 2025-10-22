using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Pangolivia.API.Controllers;
using Pangolivia.API.DTOs;
using Pangolivia.API.Repositories;
using Pangolivia.API.Services;
using Pangolivia.API.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace Pangolivia.Tests.Controllers;

public class GamesControllerTests
{
    private readonly Mock<IGameManagerService> _gameManagerMock;
    private readonly Mock<IQuizRepository> _quizRepositoryMock;
    private readonly GamesController _controller;

    public GamesControllerTests()
    {
        _gameManagerMock = new Mock<IGameManagerService>();
        _quizRepositoryMock = new Mock<IQuizRepository>();

        _controller = new GamesController(_gameManagerMock.Object, _quizRepositoryMock.Object);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
    }

    private void SetUserClaim(string userId)
    {
        ClaimsIdentity identity = new ClaimsIdentity(
            new[] { new Claim(ClaimTypes.NameIdentifier, userId) },
            "TestAuth"
        );

        ClaimsPrincipal user = new ClaimsPrincipal(identity);
        _controller.ControllerContext.HttpContext.User = user;
    }

    [Fact]
    public async Task CreateGameReturnsOkWithRoomCodeWhenSuccessful()
    {
        SetUserClaim("42");
        CreateGameRequestDto request = new CreateGameRequestDto { QuizId = 7 };

        _gameManagerMock
            .Setup(s => s.CreateGame(request.QuizId, 42))
            .ReturnsAsync("ROOM123");

        IActionResult result = await _controller.CreateGame(request);

        OkObjectResult ok = Assert.IsType<OkObjectResult>(result);
        object val = ok.Value!;
        string? roomCode = val.GetType().GetProperty("roomCode")?.GetValue(val)?.ToString();
        Assert.Equal("ROOM123", roomCode);
    }

    [Fact]
    public async Task CreateGameReturnsUnauthorizedWhenUserClaimInvalid()
    {
        SetUserClaim("invalid_id");
        CreateGameRequestDto request = new CreateGameRequestDto { QuizId = 1 };

        IActionResult result = await _controller.CreateGame(request);

        UnauthorizedObjectResult unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Equal("Invalid user identifier.", unauthorized.Value);
    }

    [Fact]
    public async Task CreateGameReturnsBadRequestWhenServiceThrows()
    {
        SetUserClaim("7");
        CreateGameRequestDto request = new CreateGameRequestDto { QuizId = 10 };

        _gameManagerMock
            .Setup(s => s.CreateGame(request.QuizId, 7))
            .ThrowsAsync(new Exception("Quiz not found."));

        IActionResult result = await _controller.CreateGame(request);

        BadRequestObjectResult badRequest = Assert.IsType<BadRequestObjectResult>(result);
        object val = badRequest.Value!;
        string? message = val.GetType().GetProperty("message")?.GetValue(val)?.ToString();
        Assert.Contains("Quiz not found", message);
    }

    [Fact]
    public void GetGameDetailsReturnsOkWhenSessionExists()
    {
        string roomCode = "ABC123";
        GameSession session = new GameSession(1, 10, "Math Quiz", "HostUser", 5);
        _gameManagerMock.Setup(s => s.GetGameSession(roomCode)).Returns(session);

        IActionResult result = _controller.GetGameDetails(roomCode);

        OkObjectResult ok = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(ok.Value);
    }

    [Fact]
    public void GetGameDetailsReturnsNotFoundWhenSessionMissing()
    {
        string roomCode = "NOPE999";
        _gameManagerMock.Setup(s => s.GetGameSession(roomCode)).Returns((GameSession?)null);

        IActionResult result = _controller.GetGameDetails(roomCode);

        NotFoundObjectResult notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Game not found.", notFound.Value);
    }
}

