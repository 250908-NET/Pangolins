using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Xunit;
using Pangolivia.API.Models;
using Pangolivia.API.DTOs;
using Pangolivia.API.Repositories;
using Pangolivia.API.Services;

namespace Pangolivia.Tests.Services
{
    public class GameRecordServiceTests
    {
        private readonly Mock<IGameRecordRepository> _gameRecordRepoMock;
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly Mock<IQuizRepository> _quizRepoMock;
        private readonly GameRecordService _service;

        public GameRecordServiceTests()
        {
            _gameRecordRepoMock = new Mock<IGameRecordRepository>();
            _userRepoMock = new Mock<IUserRepository>();
            _quizRepoMock = new Mock<IQuizRepository>();

            _service = new GameRecordService(
                _gameRecordRepoMock.Object,
                _userRepoMock.Object,
                _quizRepoMock.Object
            );
        }

        [Fact]
        public async Task CreateGameAsync_ShouldCreate_WhenUserAndQuizExist()
        {
            // Arrange
            var hostUser = new UserModel { Id = 1, Username = "admin" };
            var quiz = new QuizModel { Id = 2, QuizName = "Math Quiz" };
            var newRecord = new GameRecordModel { Id = 10, HostUserId = 1, QuizId = 2 };

            _userRepoMock.Setup(r => r.getUserModelById(1)).ReturnsAsync((UserModel)hostUser);

            _quizRepoMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync((QuizModel?)quiz);

            _gameRecordRepoMock.Setup(r => r.CreateGameRecordAsync(It.IsAny<GameRecordModel>()))
                .ReturnsAsync(newRecord);

            var dto = new CreateGameRecordDto { HostUserId = 1, QuizId = 2 };

            // Act
            var result = await _service.CreateGameAsync(dto);

            // Assert
            Assert.Equal(10, result.Id);
            Assert.Equal("Math Quiz", result.QuizName);
            _gameRecordRepoMock.Verify(r => r.CreateGameRecordAsync(It.IsAny<GameRecordModel>()), Times.Once);
        }

        [Fact]
        public async Task CreateGameAsync_ShouldThrow_WhenQuizNotFound()
        {
            _userRepoMock.Setup(r => r.getUserModelById(1))
                .ReturnsAsync(new UserModel());

            _quizRepoMock.Setup(r => r.GetByIdAsync(2))
                .ReturnsAsync((QuizModel?)null);

            var dto = new CreateGameRecordDto { HostUserId = 1, QuizId = 2 };

            await Assert.ThrowsAsync<Exception>(() => _service.CreateGameAsync(dto));
        }

        [Fact]
        public async Task GetAllGamesAsync_ShouldReturnMappedDtos()
        {
            var games = new List<GameRecordModel>
            {
                new GameRecordModel { Id = 1, HostUserId = 1, QuizId = 2, dateTimeCompleted = DateTime.UtcNow }
            };
            _gameRecordRepoMock.Setup(r => r.GetAllGameRecordsAsync())
                .ReturnsAsync(games);

            var result = await _service.GetAllGamesAsync();

            Assert.Single(result);
            Assert.Equal(1, result.First().Id);
        }

        [Fact]
        public async Task GetGameByIdAsync_ShouldReturnGame_WhenFound()
        {
            var game = new GameRecordModel { Id = 5, HostUserId = 2, QuizId = 3, dateTimeCompleted = DateTime.UtcNow };
            _gameRecordRepoMock.Setup(r => r.GetGameRecordByIdAsync(5))
                .ReturnsAsync(game);

            var result = await _service.GetGameByIdAsync(5);

            Assert.NotNull(result);
            Assert.Equal(5, result!.Id);
        }

        [Fact]
        public async Task GetGameByIdAsync_ShouldReturnNull_WhenNotFound()
        {
            _gameRecordRepoMock.Setup(r => r.GetGameRecordByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((GameRecordModel?)null);

            var result = await _service.GetGameByIdAsync(1);

            Assert.Null(result);
        }

        [Fact]
        public async Task CompleteGameAsync_ShouldUpdate_WhenFound()
        {
            var game = new GameRecordModel { Id = 1, HostUserId = 2, QuizId = 3 };

            _gameRecordRepoMock.Setup(r => r.GetGameRecordByIdAsync(1))
                .ReturnsAsync(game);

            _gameRecordRepoMock.Setup(r => r.CreateGameRecordAsync(It.IsAny<GameRecordModel>()))
                .ReturnsAsync(game);

            var result = await _service.CompleteGameAsync(1);

            Assert.NotNull(result);
            Assert.Equal(1, result!.Id);
            _gameRecordRepoMock.Verify(r => r.CreateGameRecordAsync(It.IsAny<GameRecordModel>()), Times.Once);
        }

        [Fact]
        public async Task CompleteGameAsync_ShouldThrow_WhenNotFound()
        {
            _gameRecordRepoMock.Setup(r => r.GetGameRecordByIdAsync(1))
                .ReturnsAsync((GameRecordModel?)null);

            await Assert.ThrowsAsync<Exception>(() => _service.CompleteGameAsync(1));
        }

        [Fact]
        public async Task DeleteGameAsync_ShouldCallRepo_WhenSuccessful()
        {
            _gameRecordRepoMock.Setup(r => r.DeleteGameRecordAsync(1))
                .ReturnsAsync(true);

            await _service.DeleteGameAsync(1);

            _gameRecordRepoMock.Verify(r => r.DeleteGameRecordAsync(1), Times.Once);
        }

        [Fact]
        public async Task DeleteGameAsync_ShouldThrow_WhenDeleteFails()
        {
            _gameRecordRepoMock.Setup(r => r.DeleteGameRecordAsync(1))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<Exception>(() => _service.DeleteGameAsync(1));
        }

        [Fact]
        public async Task GetGamesByHostAsync_ShouldFilterByHost()
        {
            var games = new List<GameRecordModel>
            {
                new GameRecordModel { Id = 1, HostUserId = 10, QuizId = 3 },
                new GameRecordModel { Id = 2, HostUserId = 99, QuizId = 3 }
            };
            _gameRecordRepoMock.Setup(r => r.GetAllGameRecordsAsync())
                .ReturnsAsync(games);

            var result = await _service.GetGamesByHostAsync(10);

            Assert.Single(result);
            Assert.Equal(10, result.First().HostUserId);
        }

        [Fact]
        public async Task GetGamesByQuizAsync_ShouldFilterByQuiz()
        {
            var games = new List<GameRecordModel>
            {
                new GameRecordModel { Id = 1, HostUserId = 10, QuizId = 3 },
                new GameRecordModel { Id = 2, HostUserId = 10, QuizId = 5 }
            };
            _gameRecordRepoMock.Setup(r => r.GetAllGameRecordsAsync())
                .ReturnsAsync(games);

            var result = await _service.GetGamesByQuizAsync(3);

            Assert.Single(result);
            Assert.Equal(3, result.First().QuizId);
        }
    }
}
