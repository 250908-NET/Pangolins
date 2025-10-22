using Moq;
using Xunit;
using Pangolivia.API.DTOs;
using Pangolivia.API.Models;
using Pangolivia.API.Repositories;
using Pangolivia.API.Services;

namespace Pangolivia.API.Tests.Services
{
    public class PlayerGameRecordServiceTests
    {
        private readonly Mock<IPlayerGameRecordRepository> _mockPlayerGameRecordRepo;
        private readonly Mock<IGameRecordRepository> _mockGameRecordRepo;
        private readonly Mock<IUserRepository> _mockUserRepo;
        private readonly PlayerGameRecordService _service;

        public PlayerGameRecordServiceTests()
        {
            _mockPlayerGameRecordRepo = new Mock<IPlayerGameRecordRepository>();
            _mockGameRecordRepo = new Mock<IGameRecordRepository>();
            _mockUserRepo = new Mock<IUserRepository>();
            _service = new PlayerGameRecordService(
                _mockPlayerGameRecordRepo.Object,
                _mockGameRecordRepo.Object,
                _mockUserRepo.Object
            );
        }

        #region RecordScoreAsync Tests

        [Fact]
        public async Task RecordScoreAsync_WithValidData_ReturnsPlayerGameRecordDto()
        {
            // Arrange
            var dto = new CreatePlayerGameRecordDto
            {
                GameRecordId = 1,
                UserId = 10,
                Score = 85
            };

            var game = new GameRecordModel { Id = 1 };
            var user = new UserModel { Id = 10, Username = "TestUser" };

            _mockGameRecordRepo.Setup(r => r.GetGameRecordByIdAsync(1))
                .ReturnsAsync(game);
            _mockUserRepo.Setup(r => r.getUserModelById(10))
                .ReturnsAsync(user);
            _mockPlayerGameRecordRepo.Setup(r => r.AddAsync(It.IsAny<PlayerGameRecordModel>()))
                .ReturnsAsync((PlayerGameRecordModel m) => { m.Id = 100; return m; });

            // Act
            var result = await _service.RecordScoreAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(100, result.Id);
            Assert.Equal(1, result.GameRecordId);
            Assert.Equal(10, result.UserId);
            Assert.Equal("TestUser", result.Username);
            Assert.Equal(85, result.Score);
            _mockPlayerGameRecordRepo.Verify(r => r.AddAsync(It.IsAny<PlayerGameRecordModel>()), Times.Once);
        }

        [Fact]
        public async Task RecordScoreAsync_WithNullGameRecordId_ThrowsArgumentException()
        {
            // Arrange
            var dto = new CreatePlayerGameRecordDto
            {
                GameRecordId = null,
                UserId = 10,
                Score = 85
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.RecordScoreAsync(dto));
        }

        [Fact]
        public async Task RecordScoreAsync_WithNonExistentGame_ThrowsException()
        {
            // Arrange
            var dto = new CreatePlayerGameRecordDto
            {
                GameRecordId = 999,
                UserId = 10,
                Score = 85
            };

            _mockGameRecordRepo.Setup(r => r.GetGameRecordByIdAsync(999))
                .ReturnsAsync((GameRecordModel?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _service.RecordScoreAsync(dto));
            Assert.Contains("Game record with ID 999 not found", exception.Message);
        }

        [Fact]
        public async Task RecordScoreAsync_WithNonExistentUser_ThrowsException()
        {
            // Arrange
            var dto = new CreatePlayerGameRecordDto
            {
                GameRecordId = 1,
                UserId = 999,
                Score = 85
            };

            var game = new GameRecordModel { Id = 1 };

            _mockGameRecordRepo.Setup(r => r.GetGameRecordByIdAsync(1))
                .ReturnsAsync(game);
            _mockUserRepo.Setup(r => r.getUserModelById(999))
                .ReturnsAsync((UserModel?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _service.RecordScoreAsync(dto));
            Assert.Contains("User with ID 999 not found", exception.Message);
        }

        #endregion

        #region GetLeaderboardAsync Tests

        [Fact]
        public async Task GetLeaderboardAsync_ReturnsOrderedLeaderboard()
        {
            // Arrange
            var gameRecordId = 1;
            var records = new List<PlayerGameRecordModel>
            {
                new PlayerGameRecordModel
                {
                    Id = 1,
                    GameRecordId = gameRecordId,
                    UserId = 10,
                    Score = 75,
                    User = new UserModel { Username = "Player1" }
                },
                new PlayerGameRecordModel
                {
                    Id = 2,
                    GameRecordId = gameRecordId,
                    UserId = 11,
                    Score = 95,
                    User = new UserModel { Username = "Player2" }
                },
                new PlayerGameRecordModel
                {
                    Id = 3,
                    GameRecordId = gameRecordId,
                    UserId = 12,
                    Score = 85,
                    User = new UserModel { Username = "Player3" }
                }
            };

            _mockPlayerGameRecordRepo.Setup(r => r.GetByGameRecordIdAsync(gameRecordId))
                .ReturnsAsync(records);

            // Act
            var result = await _service.GetLeaderboardAsync(gameRecordId);
            var leaderboard = result.ToList();

            // Assert
            Assert.Equal(3, leaderboard.Count);
            Assert.Equal("Player2", leaderboard[0].Username);
            Assert.Equal(95, leaderboard[0].Score);
            Assert.Equal(1, leaderboard[0].Rank);
            Assert.Equal("Player3", leaderboard[1].Username);
            Assert.Equal(85, leaderboard[1].Score);
            Assert.Equal(2, leaderboard[1].Rank);
            Assert.Equal("Player1", leaderboard[2].Username);
            Assert.Equal(75, leaderboard[2].Score);
            Assert.Equal(3, leaderboard[2].Rank);
        }

        [Fact]
        public async Task GetLeaderboardAsync_WithNoRecords_ReturnsEmptyList()
        {
            // Arrange
            var gameRecordId = 1;
            _mockPlayerGameRecordRepo.Setup(r => r.GetByGameRecordIdAsync(gameRecordId))
                .ReturnsAsync(new List<PlayerGameRecordModel>());

            // Act
            var result = await _service.GetLeaderboardAsync(gameRecordId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetLeaderboardAsync_WithNullUser_UsesUnknownAsUsername()
        {
            // Arrange
            var gameRecordId = 1;
            var records = new List<PlayerGameRecordModel>
            {
                new PlayerGameRecordModel
                {
                    Id = 1,
                    GameRecordId = gameRecordId,
                    UserId = 10,
                    Score = 75,
                    User = null
                }
            };

            _mockPlayerGameRecordRepo.Setup(r => r.GetByGameRecordIdAsync(gameRecordId))
                .ReturnsAsync(records);

            // Act
            var result = await _service.GetLeaderboardAsync(gameRecordId);
            var leaderboard = result.ToList();

            // Assert
            Assert.Single(leaderboard);
            Assert.Equal("Unknown", leaderboard[0].Username);
        }

        #endregion

        #region GetPlayerHistoryAsync Tests

        [Fact]
        public async Task GetPlayerHistoryAsync_ReturnsPlayerRecords()
        {
            // Arrange
            var userId = 10;
            var records = new List<PlayerGameRecordModel>
            {
                new PlayerGameRecordModel
                {
                    Id = 1,
                    GameRecordId = 1,
                    UserId = userId,
                    Score = 75,
                    User = new UserModel { Username = "Player1" }
                },
                new PlayerGameRecordModel
                {
                    Id = 2,
                    GameRecordId = 2,
                    UserId = userId,
                    Score = 85,
                    User = new UserModel { Username = "Player1" }
                }
            };

            _mockPlayerGameRecordRepo.Setup(r => r.GetByUserIdAsync(userId))
                .ReturnsAsync(records);

            // Act
            var result = await _service.GetPlayerHistoryAsync(userId);
            var history = result.ToList();

            // Assert
            Assert.Equal(2, history.Count);
            Assert.Equal(1, history[0].Id);
            Assert.Equal(75, history[0].Score);
            Assert.Equal(2, history[1].Id);
            Assert.Equal(85, history[1].Score);
        }

        [Fact]
        public async Task GetPlayerHistoryAsync_WithNoRecords_ReturnsEmptyList()
        {
            // Arrange
            var userId = 10;
            _mockPlayerGameRecordRepo.Setup(r => r.GetByUserIdAsync(userId))
                .ReturnsAsync(new List<PlayerGameRecordModel>());

            // Act
            var result = await _service.GetPlayerHistoryAsync(userId);

            // Assert
            Assert.Empty(result);
        }

        #endregion

        #region GetAverageScoreByPlayerAsync Tests

        [Fact]
        public async Task GetAverageScoreByPlayerAsync_ReturnsCorrectAverage()
        {
            // Arrange
            var userId = 10;
            var records = new List<PlayerGameRecordModel>
            {
                new PlayerGameRecordModel { Score = 80 },
                new PlayerGameRecordModel { Score = 90 },
                new PlayerGameRecordModel { Score = 70 }
            };

            _mockPlayerGameRecordRepo.Setup(r => r.GetByUserIdAsync(userId))
                .ReturnsAsync(records);

            // Act
            var result = await _service.GetAverageScoreByPlayerAsync(userId);

            // Assert
            Assert.Equal(80, result);
        }

        [Fact]
        public async Task GetAverageScoreByPlayerAsync_WithNoRecords_ReturnsZero()
        {
            // Arrange
            var userId = 10;
            _mockPlayerGameRecordRepo.Setup(r => r.GetByUserIdAsync(userId))
                .ReturnsAsync(new List<PlayerGameRecordModel>());

            // Act
            var result = await _service.GetAverageScoreByPlayerAsync(userId);

            // Assert
            Assert.Equal(0, result);
        }

        #endregion

        #region UpdateScoreAsync Tests

        [Fact]
        public async Task UpdateScoreAsync_WithValidData_UpdatesScore()
        {
            // Arrange
            var recordId = 1;
            var dto = new UpdatePlayerGameRecordDto { Score = 95 };
            var record = new PlayerGameRecordModel
            {
                Id = recordId,
                GameRecordId = 1,
                UserId = 10,
                Score = 85
            };

            _mockPlayerGameRecordRepo.Setup(r => r.GetByIdAsync(recordId))
                .ReturnsAsync(record);
            _mockPlayerGameRecordRepo.Setup(r => r.UpdateAsync(It.IsAny<PlayerGameRecordModel>()))
                .ReturnsAsync(record);

            // Act
            await _service.UpdateScoreAsync(recordId, dto);

            // Assert
            Assert.Equal(95, record.Score);
            _mockPlayerGameRecordRepo.Verify(r => r.UpdateAsync(record), Times.Once);
        }

        [Fact]
        public async Task UpdateScoreAsync_WithNonExistentRecord_ThrowsException()
        {
            // Arrange
            var recordId = 999;
            var dto = new UpdatePlayerGameRecordDto { Score = 95 };

            _mockPlayerGameRecordRepo.Setup(r => r.GetByIdAsync(recordId))
                .ReturnsAsync((PlayerGameRecordModel?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _service.UpdateScoreAsync(recordId, dto));
            Assert.Contains("PlayerGameRecord with ID 999 not found", exception.Message);
        }

        #endregion

        #region DeleteRecordAsync Tests

        [Fact]
        public async Task DeleteRecordAsync_CallsRepositoryDelete()
        {
            // Arrange
            var recordId = 1;
            _mockPlayerGameRecordRepo.Setup(r => r.DeleteAsync(recordId))
                .ReturnsAsync(true);

            // Act
            await _service.DeleteRecordAsync(recordId);

            // Assert
            _mockPlayerGameRecordRepo.Verify(r => r.DeleteAsync(recordId), Times.Once);
        }

        #endregion
    }
}