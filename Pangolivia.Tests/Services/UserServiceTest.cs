using Pangolivia.API.Services;
using Pangolivia.API.DTOs;
using AutoMapper;

namespace Pangolivia.Tests.Services;

public class UserServiceTest
{
    // private readonly Mock<IUserService> _userServiceMock;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly UserService _userService;

    public UserServiceTest()
    {
        _userRepoMock = new Mock<IUserRepository>();
        _mapperMock = new Mock<IMapper>();
        _userService = new UserService(_userRepoMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task GetAllUsersAsync_ShouldReturnMappedUserSummaryDtos()
    {
        // Arrange
        var userModels = new List<UserModel>
            {
                new UserModel { Id = 1, Username = "Alice"},
                new UserModel { Id = 2, Username = "Bob" }
            };

        var expectedDtos = new List<UserSummaryDto>
            {
                new UserSummaryDto { Id = 1, Username = "Alice" },
                new UserSummaryDto { Id = 2, Username = "Bob" }
            };

        _userRepoMock
            .Setup(r => r.getAllUserModels())
            .ReturnsAsync(userModels);

        _mapperMock
            .Setup(m => m.Map<IEnumerable<UserSummaryDto>>(userModels))
            .Returns(expectedDtos);

        // Act
        var result = await _userService.getAllUsersAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Equal("Alice", result.First().Username);
        Assert.Equal("Bob", result.Last().Username);

        _userRepoMock.Verify(r => r.getAllUserModels(), Times.Once);
        _mapperMock.Verify(m => m.Map<IEnumerable<UserSummaryDto>>(userModels), Times.Once);
    }

    [Fact]
    public async Task GetUserByIdAsync_ShouldReturnMappedUserDetailDto_WhenUserExists()
    {
        // Arrange
        int userId = 1;
        var userModel = new UserModel
        {
            Id = userId,
            Username = "Alice",
            AuthUuid = "uuid-123"
        };

        var expectedDto = new UserDetailDto
        {
            Id = userId,
            Username = "Alice"
        };

        _userRepoMock.Setup(r => r.getUserModelById(userId))
                     .ReturnsAsync(userModel);

        _mapperMock.Setup(m => m.Map<UserDetailDto>(userModel))
                   .Returns(expectedDto);

        // Act
        var result = await _userService.getUserByIdAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Alice", result!.Username);
        Assert.Equal(userId, result.Id);

        _userRepoMock.Verify(r => r.getUserModelById(userId), Times.Once);
        _mapperMock.Verify(m => m.Map<UserDetailDto>(userModel), Times.Once);
    }

    [Fact]
    public async Task GetUserByIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
    {
        // Arrange
        int userId = 99;
        UserModel? nullUser = null;

        _userRepoMock.Setup(r => r.getUserModelById(userId))
                     .ReturnsAsync(nullUser);

        _mapperMock.Setup(m => m.Map<UserDetailDto>(nullUser))
                   .Returns((UserDetailDto?)null);

        // Act
        var result = await _userService.getUserByIdAsync(userId);

        // Assert
        Assert.Null(result);
        _userRepoMock.Verify(r => r.getUserModelById(userId), Times.Once);
        _mapperMock.Verify(m => m.Map<UserDetailDto>(nullUser), Times.Once);
    }

    [Fact]
    public async Task DeleteUserAsync_ShouldCallRepositoryOnce()
    {
        // Arrange
        int userId = 42;

        _userRepoMock.Setup(r => r.removeUserModel(userId))
                     .Returns(Task.CompletedTask);

        // Act
        await _userService.deleteUserAsync(userId);

        // Assert
        _userRepoMock.Verify(r => r.removeUserModel(userId), Times.Once);
    }

    [Fact]
    public async Task DeleteUserAsync_ShouldThrow_WhenRepositoryThrows()
    {
        // Arrange
        int userId = 99;
        _userRepoMock.Setup(r => r.removeUserModel(userId))
                     .ThrowsAsync(new Exception("Database failure"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() =>
            _userService.deleteUserAsync(userId)
        );

        Assert.Equal("Database failure", exception.Message);
        _userRepoMock.Verify(r => r.removeUserModel(userId), Times.Once);
    }

}
