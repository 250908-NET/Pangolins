using Xunit;
using Moq;
using AutoMapper;
using Pangolivia.API.Services;
using Pangolivia.API.Repositories;
using Pangolivia.API.Models;
using Pangolivia.API.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly IMapper _mapper;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _userRepoMock = new Mock<IUserRepository>();

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<UserModel, UserSummaryDto>();
            cfg.CreateMap<UserModel, UserDetailDto>()
               .ForMember(dest => dest.HostedGamesCount, opt => opt.MapFrom(src => src.HostedGameRecords.Count))
               .ForMember(dest => dest.GamesPlayedCount, opt => opt.MapFrom(src => src.PlayerGameRecords.Count));
        });
        _mapper = mapperConfig.CreateMapper();

        _userService = new UserService(_userRepoMock.Object, _mapper);
    }

    [Fact]
    public async Task GetAllUsersAsync_ReturnsMappedUsers()
    {
        // Arrange
        var users = new List<UserModel>
        {
            new UserModel { Id = 1, Username = "Alice" },
            new UserModel { Id = 2, Username = "Bob" }
        };
        _userRepoMock.Setup(repo => repo.getAllUserModels()).ReturnsAsync(users);

        // Act
        var result = await _userService.getAllUsersAsync();

        // Assert
        Assert.Collection(result,
            u => Assert.Equal("Alice", u.Username),
            u => Assert.Equal("Bob", u.Username)
        );
    }

    [Fact]
    public async Task GetUserByIdAsync_UserExists_ReturnsMappedUser()
    {
        // Arrange
        var user = new UserModel
        {
            Id = 1,
            Username = "Alice",
            HostedGameRecords = new List<GameRecordModel>(),
            PlayerGameRecords = new List<PlayerGameRecordModel>()
        };
        _userRepoMock.Setup(repo => repo.getUserModelById(1)).ReturnsAsync(user);

        // Act
        var result = await _userService.getUserByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Alice", result.Username);
        Assert.Equal(0, result.HostedGamesCount);
        Assert.Equal(0, result.GamesPlayedCount);
    }

    [Fact]
    public async Task GetUserByIdAsync_UserDoesNotExist_ReturnsNull()
    {
        // Arrange
        _userRepoMock.Setup(repo => repo.getUserModelById(1)).ReturnsAsync((UserModel?)null);

        // Act
        var result = await _userService.getUserByIdAsync(1);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task FindUserByUsernameAsync_UserExists_ReturnsMappedUser()
    {
        // Arrange
        var user = new UserModel
        {
            Id = 1,
            Username = "Alice",
            HostedGameRecords = new List<GameRecordModel>(),
            PlayerGameRecords = new List<PlayerGameRecordModel>()
        };
        _userRepoMock.Setup(repo => repo.getUserModelByUsername("Alice")).ReturnsAsync(user);

        // Act
        var result = await _userService.findUserByUsernameAsync("Alice");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Alice", result.Username);
    }

    [Fact]
    public async Task DeleteUserAsync_CallsRepositoryRemoveUserModel()
    {
        // Arrange
        _userRepoMock.Setup(repo => repo.removeUserModel(1)).Returns(Task.CompletedTask);

        // Act
        await _userService.deleteUserAsync(1);

        // Assert
        _userRepoMock.Verify(repo => repo.removeUserModel(1), Times.Once);
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


        // Act
        var result = await _userService.getAllUsersAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Equal("Alice", result.First().Username);
        Assert.Equal("Bob", result.Last().Username);

        _userRepoMock.Verify(r => r.getAllUserModels(), Times.Once);
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

        // Act
        var result = await _userService.getUserByIdAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Alice", result!.Username);
        Assert.Equal(userId, result.Id);

        _userRepoMock.Verify(r => r.getUserModelById(userId), Times.Once);

    }



    [Fact]
    public async Task GetUserByIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
    {
        // Arrange
        int userId = 99;
        UserModel? nullUser = null;

        _userRepoMock.Setup(r => r.getUserModelById(userId))
                     .ReturnsAsync(nullUser);



        // Act
        var result = await _userService.getUserByIdAsync(userId);

        // Assert
        Assert.Null(result);
        _userRepoMock.Verify(r => r.getUserModelById(userId), Times.Once);

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




