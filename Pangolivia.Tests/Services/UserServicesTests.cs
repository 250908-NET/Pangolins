using FluentAssertions;
using Moq;
using Pangolivia.API.DTOs;
using Pangolivia.API.Models;
using Pangolivia.API.Repositories;
using Pangolivia.API.Services;
using Xunit;

namespace Pangolivia.Tests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _mockRepository;
        private readonly UserService _service;

        public UserServiceTests()
        {
            _mockRepository = new Mock<IUserRepository>();
            _service = new UserService(_mockRepository.Object);
        }

        #region GetAllUsersAsync Tests

        [Fact]
        public async Task GetAllUsersAsync_WhenNoUsersExist_ReturnsEmptyList()
        {
            // Arrange
            _mockRepository.Setup(r => r.getAllUserModels())
                .ReturnsAsync(new List<UserModel>());

            // Act
            var result = await _service.getAllUsersAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            _mockRepository.Verify(r => r.getAllUserModels(), Times.Once);
        }

        [Fact]
        public async Task GetAllUsersAsync_WhenUsersExist_ReturnsAllUsers()
        {
            // Arrange
            var users = new List<UserModel>
            {
                new UserModel { Id = 1, AuthUuid = "uuid1", Username = "user1" },
                new UserModel { Id = 2, AuthUuid = "uuid2", Username = "user2" },
                new UserModel { Id = 3, AuthUuid = "uuid3", Username = "user3" }
            };
            _mockRepository.Setup(r => r.getAllUserModels())
                .ReturnsAsync(users);

            // Act
            var result = await _service.getAllUsersAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.Should().BeEquivalentTo(users);
            _mockRepository.Verify(r => r.getAllUserModels(), Times.Once);
        }

        #endregion

        #region GetUserByIdAsync Tests

        [Fact]
        public async Task GetUserByIdAsync_WhenUserExists_ReturnsUser()
        {
            // Arrange
            var userId = 1;
            var expectedUser = new UserModel 
            { 
                Id = userId, 
                AuthUuid = "test-uuid", 
                Username = "testuser" 
            };
            _mockRepository.Setup(r => r.getUserModelById(userId))
                .ReturnsAsync(expectedUser);

            // Act
            var result = await _service.getUserByIdAsync(userId);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(userId);
            result.Username.Should().Be("testuser");
            result.AuthUuid.Should().Be("test-uuid");
            _mockRepository.Verify(r => r.getUserModelById(userId), Times.Once);
        }

        [Fact]
        public async Task GetUserByIdAsync_WhenUserDoesNotExist_ReturnsNull()
        {
            // Arrange
            var userId = 999;
            _mockRepository.Setup(r => r.getUserModelById(userId))
                .ReturnsAsync((UserModel?)null);

            // Act
            var result = await _service.getUserByIdAsync(userId);

            // Assert
            result.Should().BeNull();
            _mockRepository.Verify(r => r.getUserModelById(userId), Times.Once);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(42)]
        [InlineData(100)]
        public async Task GetUserByIdAsync_WithDifferentIds_CallsRepositoryWithCorrectId(int userId)
        {
            // Arrange
            _mockRepository.Setup(r => r.getUserModelById(userId))
                .ReturnsAsync(new UserModel { Id = userId });

            // Act
            await _service.getUserByIdAsync(userId);

            // Assert
            _mockRepository.Verify(r => r.getUserModelById(userId), Times.Once);
        }

        #endregion

        #region FindUserByUsernameAsync Tests

        [Fact]
        public async Task FindUserByUsernameAsync_WhenUserExists_ReturnsUser()
        {
            // Arrange
            var username = "existinguser";
            var expectedUser = new UserModel 
            { 
                Id = 1, 
                AuthUuid = "uuid", 
                Username = username 
            };
            _mockRepository.Setup(r => r.getUserModelByUsername(username))
                .ReturnsAsync(expectedUser);

            // Act
            var result = await _service.findUserByUsernameAsync(username);

            // Assert
            result.Should().NotBeNull();
            result!.Username.Should().Be(username);
            result.Id.Should().Be(1);
            _mockRepository.Verify(r => r.getUserModelByUsername(username), Times.Once);
        }

        [Fact]
        public async Task FindUserByUsernameAsync_WhenUserDoesNotExist_ReturnsNull()
        {
            // Arrange
            var username = "nonexistentuser";
            _mockRepository.Setup(r => r.getUserModelByUsername(username))
                .ReturnsAsync((UserModel?)null);

            // Act
            var result = await _service.findUserByUsernameAsync(username);

            // Assert
            result.Should().BeNull();
            _mockRepository.Verify(r => r.getUserModelByUsername(username), Times.Once);
        }

        [Theory]
        [InlineData("user1")]
        [InlineData("admin")]
        [InlineData("testuser123")]
        public async Task FindUserByUsernameAsync_WithDifferentUsernames_CallsRepositoryCorrectly(string username)
        {
            // Arrange
            _mockRepository.Setup(r => r.getUserModelByUsername(username))
                .ReturnsAsync(new UserModel { Username = username });

            // Act
            await _service.findUserByUsernameAsync(username);

            // Assert
            _mockRepository.Verify(r => r.getUserModelByUsername(username), Times.Once);
        }

        [Fact]
        public async Task FindUserByUsernameAsync_WithEmptyString_CallsRepository()
        {
            // Arrange
            var username = "";
            _mockRepository.Setup(r => r.getUserModelByUsername(username))
                .ReturnsAsync((UserModel?)null);

            // Act
            var result = await _service.findUserByUsernameAsync(username);

            // Assert
            result.Should().BeNull();
            _mockRepository.Verify(r => r.getUserModelByUsername(username), Times.Once);
        }

        #endregion

        #region UpdateUserAsync Tests

        [Fact]
        public async Task UpdateUserAsync_WithPlayerGameRecordDto_CallsCorrectRepositoryMethod()
        {
            // Arrange
            var userId = 1;
            var pgrDto = new PlayerGameRecordDto 
            { 
                GameRecordId = 10,
                Score = 100,
                Rank = 1
            };
            var updatedUser = new UserModel 
            { 
                Id = userId, 
                Username = "testuser" 
            };
            
            _mockRepository.Setup(r => r.updateUserModelPlayerGameRecord(userId, pgrDto))
                .ReturnsAsync(updatedUser);

            // Act
            var result = await _service.updateUserAsync(userId, pgrDto);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(userId);
            _mockRepository.Verify(r => r.updateUserModelPlayerGameRecord(userId, pgrDto), Times.Once);
        }

        [Fact]
        public async Task UpdateUserAsync_WithGameRecordModel_CallsCorrectRepositoryMethod()
        {
            // Arrange
            var userId = 1;
            var gameRecord = new GameRecordModel 
            { 
                Id = 5,
                HostUserId = userId,
                QuizId = 10,
                datetimeCompleted = DateTime.UtcNow
            };
            var updatedUser = new UserModel 
            { 
                Id = userId, 
                Username = "testuser" 
            };
            
            _mockRepository.Setup(r => r.updateUserModelHostedGameRecord(userId, gameRecord))
                .ReturnsAsync(updatedUser);

            // Act
            var result = await _service.updateUserAsync(userId, gameRecord);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(userId);
            _mockRepository.Verify(r => r.updateUserModelHostedGameRecord(userId, gameRecord), Times.Once);
        }

        [Fact]
        public async Task UpdateUserAsync_WithQuizModel_CallsCorrectRepositoryMethod()
        {
            // Arrange
            var userId = 1;
            var quiz = new QuizModel 
            { 
                Id = 7,
                QuizName = "Test Quiz",
                CreatedByUserId = userId
            };
            var updatedUser = new UserModel 
            { 
                Id = userId, 
                Username = "testuser" 
            };
            
            _mockRepository.Setup(r => r.updateUserModelCreatedQuizzes(userId, quiz))
                .ReturnsAsync(updatedUser);

            // Act
            var result = await _service.updateUserAsync(userId, quiz);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(userId);
            _mockRepository.Verify(r => r.updateUserModelCreatedQuizzes(userId, quiz), Times.Once);
        }

        [Fact]
        public async Task UpdateUserAsync_WithUnknownType_ThrowsArgumentException()
        {
            // Arrange
            var userId = 1;
            var unknownObject = new { SomeProperty = "value" };

            // Act
            Func<Task> act = async () => await _service.updateUserAsync(userId, unknownObject);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Unknown Type: should be type:PlayerGameRecordDto, GameRecordModel, QuizModel");
        }

        [Fact]
        public async Task UpdateUserAsync_WithNull_ThrowsException()
        {
            // Arrange
            var userId = 1;

            // Act
            Func<Task> act = async () => await _service.updateUserAsync(userId, null!);

            // Assert
            await act.Should().ThrowAsync<Exception>();
        }

        [Fact]
        public async Task UpdateUserAsync_WithStringType_ThrowsArgumentException()
        {
            // Arrange
            var userId = 1;
            var invalidObject = "just a string";

            // Act
            Func<Task> act = async () => await _service.updateUserAsync(userId, invalidObject);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Unknown Type: should be type:PlayerGameRecordDto, GameRecordModel, QuizModel");
        }

        #endregion

        #region DeleteUserAsync Tests

        [Fact]
        public async Task DeleteUserAsync_CallsRepositoryRemoveMethod()
        {
            // Arrange
            var userId = 1;
            _mockRepository.Setup(r => r.removeUserModel(userId))
                .Returns(Task.CompletedTask);

            // Act
            await _service.deleteUserAsync(userId);

            // Assert
            _mockRepository.Verify(r => r.removeUserModel(userId), Times.Once);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(42)]
        [InlineData(999)]
        public async Task DeleteUserAsync_WithDifferentIds_CallsRepositoryWithCorrectId(int userId)
        {
            // Arrange
            _mockRepository.Setup(r => r.removeUserModel(userId))
                .Returns(Task.CompletedTask);

            // Act
            await _service.deleteUserAsync(userId);

            // Assert
            _mockRepository.Verify(r => r.removeUserModel(userId), Times.Once);
        }

        [Fact]
        public async Task DeleteUserAsync_WhenRepositoryThrowsException_PropagatesException()
        {
            // Arrange
            var userId = 1;
            _mockRepository.Setup(r => r.removeUserModel(userId))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            Func<Task> act = async () => await _service.deleteUserAsync(userId);

            // Assert
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("Database error");
        }

        [Fact]
        public async Task DeleteUserAsync_DoesNotReturnValue()
        {
            // Arrange
            var userId = 1;
            _mockRepository.Setup(r => r.removeUserModel(userId))
                .Returns(Task.CompletedTask);

            // Act
            var result = _service.deleteUserAsync(userId);

            // Assert
            await result; // Should complete without returning a value
            result.Should().NotBeNull();
            result.IsCompletedSuccessfully.Should().BeTrue();
        }

        #endregion

        #region Edge Cases and Integration

        [Fact]
        public async Task ServiceMethods_WhenRepositoryReturnsNull_HandlesGracefully()
        {
            // Arrange
            _mockRepository.Setup(r => r.getUserModelById(It.IsAny<int>()))
                .ReturnsAsync((UserModel?)null);

            // Act
            var result = await _service.getUserByIdAsync(1);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task ServiceMethods_WhenCalledMultipleTimes_EachCallIsIndependent()
        {
            // Arrange
            var user1 = new UserModel { Id = 1, Username = "user1" };
            var user2 = new UserModel { Id = 2, Username = "user2" };
            
            _mockRepository.Setup(r => r.getUserModelById(1)).ReturnsAsync(user1);
            _mockRepository.Setup(r => r.getUserModelById(2)).ReturnsAsync(user2);

            // Act
            var result1 = await _service.getUserByIdAsync(1);
            var result2 = await _service.getUserByIdAsync(2);

            // Assert
            result1!.Id.Should().Be(1);
            result2!.Id.Should().Be(2);
            _mockRepository.Verify(r => r.getUserModelById(1), Times.Once);
            _mockRepository.Verify(r => r.getUserModelById(2), Times.Once);
        }

        #endregion
    }
}