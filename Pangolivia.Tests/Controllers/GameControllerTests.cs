using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Pangolivia.API.Controllers;
using Pangolivia.API.DTOs;
using Pangolivia.API.Repositories;
using Pangolivia.API.Services;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Pangolivia.API.GameEngine;
using Pangolivia.API.Models;
using System.Collections.Generic;

namespace Pangolivia.Tests.Controllers
{
    public class GamesControllerTests
    {
        private readonly Mock<IGameManagerService> _gameManagerMock;
        private readonly Mock<IQuizRepository> _quizRepositoryMock;
        private readonly Mock<ILogger<GamesController>> _loggerMock;
        private readonly GamesController _controller;

        public GamesControllerTests()
        {
            _gameManagerMock = new Mock<IGameManagerService>();
            _quizRepositoryMock = new Mock<IQuizRepository>();
            _loggerMock = new Mock<ILogger<GamesController>>();

            _controller = new GamesController(
                _gameManagerMock.Object,
                _quizRepositoryMock.Object,
                _loggerMock.Object
            );

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }

        private void SetUserClaims(string userId, string username)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, username)
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var user = new ClaimsPrincipal(identity);
            _controller.ControllerContext.HttpContext.User = user;
        }

        [Fact]
        public async Task CreateGame_WithValidRequest_ReturnsOkWithRoomCode()
        {
            // Arrange
            SetUserClaims("42", "TestUser");
            var request = new CreateGameRequestDto { QuizId = 7 };
            var expectedRoomCode = "ROOM123";

            _gameManagerMock
                .Setup(s => s.CreateGame(request.QuizId, 42, "TestUser"))
                .ReturnsAsync(expectedRoomCode);

            // Act
            var result = await _controller.CreateGame(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = okResult.Value;
            Assert.NotNull(value);

            // *** FIX: Use reflection to safely access the property ***
            var roomCodeProperty = value.GetType().GetProperty("roomCode");
            Assert.NotNull(roomCodeProperty); // Ensure the property exists
            var roomCodeValue = roomCodeProperty.GetValue(value);
            Assert.Equal(expectedRoomCode, roomCodeValue);
        }

        [Fact]
        public async Task CreateGame_WithInvalidUserIdClaim_ReturnsUnauthorized()
        {
            // Arrange
            SetUserClaims("invalid-id", "TestUser");
            var request = new CreateGameRequestDto { QuizId = 1 };

            // Act
            var result = await _controller.CreateGame(request);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Invalid user identifier.", unauthorizedResult.Value);
        }

        [Fact]
        public async Task CreateGame_WithMissingUsernameClaim_ReturnsUnauthorized()
        {
            // Arrange
            var claims = new[] { new Claim(ClaimTypes.NameIdentifier, "42") };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var user = new ClaimsPrincipal(identity);
            _controller.ControllerContext.HttpContext.User = user;

            var request = new CreateGameRequestDto { QuizId = 1 };

            // Act
            var result = await _controller.CreateGame(request);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Username not found in token.", unauthorizedResult.Value);
        }

        [Fact]
        public async Task CreateGame_WhenServiceThrowsException_ReturnsBadRequest()
        {
            // Arrange
            SetUserClaims("7", "AnotherUser");
            var request = new CreateGameRequestDto { QuizId = 10 };
            var exceptionMessage = "Quiz not found.";

            _gameManagerMock
                .Setup(s => s.CreateGame(request.QuizId, 7, "AnotherUser"))
                .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.CreateGame(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var value = badRequestResult.Value;
            Assert.NotNull(value);

            // *** FIX: Use reflection to safely access the property ***
            var messageProperty = value.GetType().GetProperty("message");
            Assert.NotNull(messageProperty); // Ensure the property exists
            var messageValue = messageProperty.GetValue(value) as string;
            Assert.Equal(exceptionMessage, messageValue);
        }

        [Fact]
        public void GetGameDetails_WhenSessionExists_ReturnsOk()
        {
            // Arrange
            var roomCode = "ABC123";
            var mockQuiz = new QuizModel { Id = 10, QuizName = "Math Quiz" };
            var session = new GameSession(1, "Math Quiz", 5, "HostUser", mockQuiz);

            _gameManagerMock.Setup(s => s.GetGameSession(roomCode)).Returns(session);

            // Act
            var result = _controller.GetGameDetails(roomCode);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public void GetGameDetails_WhenSessionIsMissing_ReturnsNotFound()
        {
            // Arrange
            var roomCode = "NOPE999";
            _gameManagerMock.Setup(s => s.GetGameSession(roomCode)).Returns((GameSession?)null);

            // Act
            var result = _controller.GetGameDetails(roomCode);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Game not found.", notFoundResult.Value);
        }
    }
}