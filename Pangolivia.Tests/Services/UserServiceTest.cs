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
}
