using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Pangolivia.API.Controllers;
using Pangolivia.API.Services;
using Pangolivia.API.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Reflection;

namespace Pangolivia.Tests.Controllers;

public class PlayerGameRecordControllerTests
{
    private readonly Mock<IPlayerGameRecordService> _serviceMock;
    private readonly PlayerGameRecordController _controller;

    public PlayerGameRecordControllerTests()
    {
        _serviceMock = new Mock<IPlayerGameRecordService>();
        _controller = new PlayerGameRecordController(_serviceMock.Object);
    }

    [Fact]
    public async Task RecordScoreReturnsCreatedAtActionWhenSuccessful()
    {
        CreatePlayerGameRecordDto dto = new CreatePlayerGameRecordDto
        {
            GameRecordId = 10,
            UserId = 1,
            Score = 98.5
        };

        PlayerGameRecordDto created = new PlayerGameRecordDto
        {
            Id = 5,
            UserId = 1,
            Username = "Alice",
            GameRecordId = 10,
            Score = 98.5
        };

        _serviceMock.Setup(s => s.RecordScoreAsync(dto)).ReturnsAsync(created);

        IActionResult result = await _controller.RecordScore(dto);

        CreatedAtActionResult createdResult = Assert.IsType<CreatedAtActionResult>(result);
        PlayerGameRecordDto returnValue = Assert.IsType<PlayerGameRecordDto>(createdResult.Value);
        Assert.Equal("Alice", returnValue.Username);
        Assert.Equal(98.5, returnValue.Score);
    }

    [Fact]
    public async Task RecordScoreReturnsBadRequestOnException()
    {
        CreatePlayerGameRecordDto dto = new CreatePlayerGameRecordDto
        {
            GameRecordId = 99,
            UserId = 3,
            Score = 50
        };

        _serviceMock.Setup(s => s.RecordScoreAsync(dto)).ThrowsAsync(new Exception("Game not found"));

        IActionResult result = await _controller.RecordScore(dto);

        BadRequestObjectResult badRequest = Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task GetLeaderboardReturnsOkWithList()
    {
        List<LeaderboardDto> leaderboard = new List<LeaderboardDto>
        {
            new LeaderboardDto { Username = "Alice", Score = 90, Rank = 1 },
            new LeaderboardDto { Username = "Bob", Score = 80, Rank = 2 }
        };

        _serviceMock.Setup(s => s.GetLeaderboardAsync(10)).ReturnsAsync(leaderboard);

        IActionResult result = await _controller.GetLeaderboard(10);

        OkObjectResult ok = Assert.IsType<OkObjectResult>(result);
        List<LeaderboardDto> returnValue = Assert.IsType<List<LeaderboardDto>>(ok.Value);
        Assert.Equal(2, returnValue.Count);
        Assert.Equal("Alice", returnValue[0].Username);
    }

    [Fact]
    public async Task GetPlayerHistoryReturnsOkWithList()
    {
        List<PlayerGameRecordDto> history = new List<PlayerGameRecordDto>
        {
            new PlayerGameRecordDto { Id = 1, UserId = 5, Username = "Eve", GameRecordId = 11, Score = 77.5 },
            new PlayerGameRecordDto { Id = 2, UserId = 5, Username = "Eve", GameRecordId = 12, Score = 88.0 }
        };

        _serviceMock.Setup(s => s.GetPlayerHistoryAsync(5)).ReturnsAsync(history);

        IActionResult result = await _controller.GetPlayerHistory(5);

        OkObjectResult ok = Assert.IsType<OkObjectResult>(result);
        List<PlayerGameRecordDto> returnValue = Assert.IsType<List<PlayerGameRecordDto>>(ok.Value);
        Assert.Equal(2, returnValue.Count);
        Assert.Equal("Eve", returnValue[0].Username);
    }

    [Fact]
    public async Task GetAverageScoreReturnsOkWithAverage()
    {
        _serviceMock.Setup(s => s.GetAverageScoreByPlayerAsync(2)).ReturnsAsync(85.6);

        IActionResult result = await _controller.GetAverageScore(2);

        OkObjectResult ok = Assert.IsType<OkObjectResult>(result);
        object val = ok.Value!;
        PropertyInfo avgProp = val.GetType().GetProperty("averageScore")!;
        PropertyInfo userProp = val.GetType().GetProperty("userId")!;
        Assert.Equal(2, (int)userProp.GetValue(val)!);
        Assert.Equal(85.6, (double)avgProp.GetValue(val)!);
    }

    [Fact]
    public async Task UpdateScoreReturnsNoContentWhenSuccessful()
    {
        UpdatePlayerGameRecordDto dto = new UpdatePlayerGameRecordDto { Score = 99.5 };
        _serviceMock.Setup(s => s.UpdateScoreAsync(7, dto)).Returns(Task.CompletedTask);

        IActionResult result = await _controller.UpdateScore(7, dto);

        Assert.IsType<NoContentResult>(result);
        _serviceMock.Verify(s => s.UpdateScoreAsync(7, dto), Times.Once);
    }

    [Fact]
    public async Task UpdateScoreReturnsNotFoundOnException()
    {
        UpdatePlayerGameRecordDto dto = new UpdatePlayerGameRecordDto { Score = 10.5 };
        _serviceMock.Setup(s => s.UpdateScoreAsync(99, dto)).ThrowsAsync(new Exception("Record not found"));

        IActionResult result = await _controller.UpdateScore(99, dto);

        NotFoundObjectResult notFound = Assert.IsType<NotFoundObjectResult>(result);
        object val = notFound.Value!;
        PropertyInfo prop = val.GetType().GetProperty("message")!;
        string message = prop.GetValue(val)?.ToString()!;
        Assert.Contains("Record not found", message);
    }

    [Fact]
    public async Task DeleteRecordReturnsNoContentWhenSuccessful()
    {
        _serviceMock.Setup(s => s.DeleteRecordAsync(3)).Returns(Task.CompletedTask);

        IActionResult result = await _controller.DeleteRecord(3);

        Assert.IsType<NoContentResult>(result);
        _serviceMock.Verify(s => s.DeleteRecordAsync(3), Times.Once);
    }

    [Fact]
    public async Task DeleteRecordReturnsNotFoundOnException()
    {
        _serviceMock.Setup(s => s.DeleteRecordAsync(77)).ThrowsAsync(new Exception("Delete failed"));

        IActionResult result = await _controller.DeleteRecord(77);

        NotFoundObjectResult notFound = Assert.IsType<NotFoundObjectResult>(result);
        object val = notFound.Value!;
        PropertyInfo prop = val.GetType().GetProperty("message")!;
        string message = prop.GetValue(val)?.ToString()!;
        Assert.Contains("Delete failed", message);
    }
}

