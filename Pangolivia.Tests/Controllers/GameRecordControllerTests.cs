using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Pangolivia.API.Controllers;
using Pangolivia.API.Services;
using Pangolivia.API.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pangolivia.Tests.Controllers;

public class GameRecordControllerTests
{
    private readonly Mock<IGameRecordService> _serviceMock;
    private readonly Mock<ILogger<GameRecordController>> _loggerMock;
    private readonly GameRecordController _controller;

    public GameRecordControllerTests()
    {
        _serviceMock = new Mock<IGameRecordService>();
        _loggerMock = new Mock<ILogger<GameRecordController>>();
        _controller = new GameRecordController(_serviceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task CreateGameReturnsCreatedAtActionWhenSuccessful()
    {
        var dto = new CreateGameRecordDto
        {
            HostUserId = 1,
            QuizId = 2,
            dateTimeCompleted = DateTime.UtcNow
        };

        var created = new GameRecordDto
        {
            Id = 10,
            HostUserId = 1,
            HostUsername = "Alice",
            QuizId = 2,
            QuizName = "MathQuiz",
            dateTimeCompleted = DateTime.UtcNow
        };

        _serviceMock.Setup(s => s.CreateGameAsync(dto)).ReturnsAsync(created);


        var result = await _controller.CreateGame(dto);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        var returnValue = Assert.IsType<GameRecordDto>(createdResult.Value);
        Assert.Equal(10, returnValue.Id);
        Assert.Equal("MathQuiz", returnValue.QuizName);
    }

    [Fact]
    public async Task CreateGameReturnsBadRequestOnException()
    {
        var dto = new CreateGameRecordDto { HostUserId = 1, QuizId = 2 };

        _serviceMock.Setup(s => s.CreateGameAsync(dto)).ThrowsAsync(new InvalidOperationException("Game creation failed"));

        var result = await _controller.CreateGame(dto);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Game creation failed", badRequest.Value);
    }

    [Fact]
    public async Task GetAllGamesReturnsOkWithList()
    {
        var games = new List<GameRecordDto>
        {
            new GameRecordDto
            {
                Id = 1,
                HostUserId = 10,
                HostUsername = "John",
                QuizId = 100,
                QuizName = "History Quiz",
                dateTimeCompleted = DateTime.UtcNow
            },
            new GameRecordDto
            {
                Id = 2,
                HostUserId = 11,
                HostUsername = "Jane",
                QuizId = 200,
                QuizName = "Science Quiz",
                dateTimeCompleted = DateTime.UtcNow
            }
        };

        _serviceMock.Setup(s => s.GetAllGamesAsync()).ReturnsAsync(games);

        var result = await _controller.GetAllGames();

        var ok = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<List<GameRecordDto>>(ok.Value);
        Assert.Equal(2, returnValue.Count);
    }

    [Fact]
    public async Task GetGameByIdReturnsOkWhenFound()
    {
        var game = new GameRecordDto
        {
            Id = 5,
            HostUserId = 1,
            HostUsername = "Sam",
            QuizId = 20,
            QuizName = "Geography Quiz",
            dateTimeCompleted = DateTime.UtcNow
        };

        _serviceMock.Setup(s => s.GetGameByIdAsync(5)).ReturnsAsync(game);

        var result = await _controller.GetGameById(5);

        var ok = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<GameRecordDto>(ok.Value);
        Assert.Equal("Sam", returnValue.HostUsername);
    }

    [Fact]
    public async Task GetGameByIdReturnsNotFound_WhenNull()
    {
        _serviceMock.Setup(s => s.GetGameByIdAsync(9)).ReturnsAsync((GameRecordDto?)null);

        var result = await _controller.GetGameById(9);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Game with ID 9 not found.", notFound.Value);
    }

    [Fact]
    public async Task CompleteGameReturnsOkWhenSuccessful()
    {
        var completed = new GameRecordDto
        {
            Id = 5,
            HostUserId = 3,
            HostUsername = "Eve",
            QuizId = 9,
            QuizName = "Art Quiz",
            dateTimeCompleted = DateTime.UtcNow
        };

        _serviceMock.Setup(s => s.CompleteGameAsync(5)).ReturnsAsync(completed);

        var result = await _controller.CompleteGame(5);

        var ok = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<GameRecordDto>(ok.Value);
        Assert.Equal("Art Quiz", returnValue.QuizName);
    }

    [Fact]
    public async Task CompleteGameReturnsBadRequestOnError()
    {
        _serviceMock.Setup(s => s.CompleteGameAsync(42)).ThrowsAsync(new Exception("Game already completed"));

        var result = await _controller.CompleteGame(42);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Game already completed", badRequest.Value);
    }

    [Fact]
    public async Task DeleteGameReturnsNoContent_WhenSuccessful()
    {
        _serviceMock.Setup(s => s.DeleteGameAsync(3)).Returns(Task.CompletedTask);

        var result = await _controller.DeleteGame(3);

        Assert.IsType<NoContentResult>(result);
        _serviceMock.Verify(s => s.DeleteGameAsync(3), Times.Once);
    }

    [Fact]
    public async Task DeleteGameReturnsNotFoundOnException()
    {
        _serviceMock.Setup(s => s.DeleteGameAsync(9)).ThrowsAsync(new Exception("Game not found"));

        var result = await _controller.DeleteGame(9);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Game not found", notFound.Value);
    }

    [Fact]
    public async Task GetGamesByHostReturnsOkWithList()
    {
        var games = new List<GameRecordDto>
        {
            new GameRecordDto
            {
                Id = 1,
                HostUserId = 2,
                HostUsername = "Alice",
                QuizId = 1,
                QuizName = "Math Quiz",
                dateTimeCompleted = DateTime.UtcNow
            }
        };

        _serviceMock.Setup(s => s.GetGamesByHostAsync(2)).ReturnsAsync(games);

        var result = await _controller.GetGamesByHost(2);

        var ok = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<List<GameRecordDto>>(ok.Value);
        Assert.Single(returnValue);
        Assert.Equal("Alice", returnValue[0].HostUsername);
    }

    [Fact]
    public async Task GetGamesByQuizReturnsOkWithList()
    {
        var games = new List<GameRecordDto>
        {
            new GameRecordDto
            {
                Id = 99,
                HostUserId = 3,
                HostUsername = "Bob",
                QuizId = 7,
                QuizName = "Science Quiz",
                dateTimeCompleted = DateTime.UtcNow
            }
        };

        _serviceMock.Setup(s => s.GetGamesByQuizAsync(7)).ReturnsAsync(games);

        var result = await _controller.GetGamesByQuiz(7);

        var ok = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<List<GameRecordDto>>(ok.Value);
        Assert.Single(returnValue);
        Assert.Equal("Bob", returnValue[0].HostUsername);
    }
}

