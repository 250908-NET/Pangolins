using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Pangolivia.API.Controllers;
using Pangolivia.API.Services;
using Pangolivia.API.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace Pangolivia.Tests.Controllers;

public class QuizControllerTests
{
    private readonly Mock<IQuizService> _quizServiceMock;
    private readonly Mock<IAiQuizService> _aiQuizServiceMock;
    private readonly Mock<ILogger<QuizController>> _loggerMock;
    private readonly QuizController _controller;

    public QuizControllerTests()
    {
        _quizServiceMock = new Mock<IQuizService>();
        _aiQuizServiceMock = new Mock<IAiQuizService>();
        _loggerMock = new Mock<ILogger<QuizController>>();

        _controller = new QuizController(
            _quizServiceMock.Object,
            _aiQuizServiceMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task GetAllQuizzesReturnsOk_WithList()
    {
        
        var quizzes = new List<QuizSummaryDto>
        {
            new QuizSummaryDto { Id = 1, QuizName = "QuizWithId1" }
        };
        _quizServiceMock.Setup(s => s.GetAllQuizzesAsync()).ReturnsAsync(quizzes);
    
        var result = await _controller.GetAllQuizzes();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<List<QuizSummaryDto>>(okResult.Value);
        Assert.Single(returnValue);
        Assert.Equal("QuizWithId1", returnValue[0].QuizName);
    }

    [Fact]
    public async Task GetQuizByIdReturnsOkWhenFound()
    {
        var quiz = new QuizDetailDto { Id = 10, QuizName = "QuizWithId10" };
        _quizServiceMock.Setup(s => s.GetQuizByIdAsync(10)).ReturnsAsync(quiz);

        var result = await _controller.GetQuizById(10);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<QuizDetailDto>(okResult.Value);
        Assert.Equal(10, returnValue.Id);
        Assert.Equal("QuizWithId10", returnValue.QuizName);
    }

    [Fact]
    public async Task GetQuizByIdReturnsNotFoundWhenNull()
    {
        _quizServiceMock.Setup(s => s.GetQuizByIdAsync(99)).ReturnsAsync((QuizDetailDto?)null);

        var result = await _controller.GetQuizById(99);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task CreateQuizReturnsCreatedAtAction()
    {
        var createDto = new CreateQuizRequestDto { QuizName = "QuizWithId5" };
        var createdQuiz = new QuizDetailDto { Id = 5, QuizName = "QuizWithId5" };

        _quizServiceMock.Setup(s => s.CreateQuizAsync(createDto, 1)).ReturnsAsync(createdQuiz);

        var result = await _controller.CreateQuiz(createDto, 1);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnValue = Assert.IsType<QuizDetailDto>(createdResult.Value);
        Assert.Equal(5, returnValue.Id);
        Assert.Equal("QuizWithId5", returnValue.QuizName);
    }

    [Fact]
    public async Task UpdateQuizReturnsOkgiWhenSuccess()
    {
        var updateDto = new UpdateQuizRequestDto { QuizName = "UpdatedQuiz" };
        var updatedQuiz = new QuizDetailDto { Id = 3, QuizName = "UpdatedQuiz" };

        _quizServiceMock.Setup(s => s.UpdateQuizAsync(3, updateDto, 1)).ReturnsAsync(updatedQuiz);

        var result = await _controller.UpdateQuiz(3, updateDto, 1);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<QuizDetailDto>(okResult.Value);
        Assert.Equal("UpdatedQuiz", returnValue.QuizName);
    }

    [Fact]
    public async Task UpdateQuizReturnsNotFoundWhenKeyNotFound()
    {
        var dto = new UpdateQuizRequestDto();

        _quizServiceMock.Setup(s => s.UpdateQuizAsync(3, dto, 1)).ThrowsAsync(new KeyNotFoundException());

        var result = await _controller.UpdateQuiz(3, dto, 1);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task UpdateQuizReturnsForbidWhenUnauthorized()
    {
        var dto = new UpdateQuizRequestDto();

        _quizServiceMock.Setup(s => s.UpdateQuizAsync(3, dto, 1)).ThrowsAsync(new UnauthorizedAccessException());

        var result = await _controller.UpdateQuiz(3, dto, 1);

        Assert.IsType<ForbidResult>(result.Result);
    }

    [Fact]
    public async Task DeleteQuizReturnsNoContentWhenSuccess()
    {
        _quizServiceMock.Setup(s => s.DeleteQuizAsync(5, 1)).Returns(Task.CompletedTask);

        var result = await _controller.DeleteQuiz(5, 1);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteQuizReturnsNotFoundWhenKeyNotFound()
    {
        _quizServiceMock.Setup(s => s.DeleteQuizAsync(5, 1)).ThrowsAsync(new KeyNotFoundException());

        var result = await _controller.DeleteQuiz(5, 1);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteQuizReturnsForbidWhenUnauthorized()
    {
        _quizServiceMock.Setup(s => s.DeleteQuizAsync(5, 1)).ThrowsAsync(new UnauthorizedAccessException());

        var result = await _controller.DeleteQuiz(5, 1);

        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task GetQuizzesByUserIdReturnsOkWithList()
    {
        var quizzes = new List<QuizSummaryDto> { new QuizSummaryDto { Id = 1, QuizName = "Biology Quiz" } };
        _quizServiceMock.Setup(s => s.GetQuizzesByUserIdAsync(10)).ReturnsAsync(quizzes);

        var result = await _controller.GetQuizzesByUserId(10);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<List<QuizSummaryDto>>(okResult.Value);
        Assert.Single(returnValue);
    }

    [Fact]
    public async Task FindQuizzesByNameReturnsOkWithList()
    {
        var quizzes = new List<QuizSummaryDto> { new QuizSummaryDto { Id = 2, QuizName = "AI Fundamentals" } };
        _quizServiceMock.Setup(s => s.FindQuizzesByNameAsync("AI")).ReturnsAsync(quizzes);

        var result = await _controller.FindQuizzesByName("AI");

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<List<QuizSummaryDto>>(okResult.Value);
        Assert.Single(returnValue);
        Assert.Equal("AI Fundamentals", returnValue[0].QuizName);
    }
}

