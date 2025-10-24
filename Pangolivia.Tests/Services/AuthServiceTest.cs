using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Moq;
using Pangolivia.API.DTOs;
using Pangolivia.API.Models;
using Pangolivia.API.Repositories;
using Pangolivia.API.Services;
using Xunit;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _configurationMock = new Mock<IConfiguration>();

        // Correct 256-bit key (32 characters)
        _configurationMock.Setup(c => c["Jwt:Key"]).Returns("12345678901234567890123456789012");
        _configurationMock.Setup(c => c["Jwt:Issuer"]).Returns("TestIssuer");
        _configurationMock.Setup(c => c["Jwt:Audience"]).Returns("TestAudience");

        _authService = new AuthService(_userRepositoryMock.Object, _configurationMock.Object);
    }

    [Fact]
    public async Task RegisterAsync_CreatesNewUser_WhenUsernameIsUnique()
    {
        // Arrange
        var registerDto = new UserRegisterDto
        {
            Username = "testuser",
            Password = "password123"
        };

        _userRepositoryMock.Setup(r => r.getUserModelByUsername("testuser"))
            .ReturnsAsync((UserModel?)null);

        _userRepositoryMock.Setup(r => r.createUserModel(It.IsAny<UserModel>()))
            .ReturnsAsync((UserModel u) => u);

        // Act
        var user = await _authService.RegisterAsync(registerDto);

        // Assert
        Assert.NotNull(user);
        Assert.Equal("testuser", user.Username);
        Assert.False(string.IsNullOrEmpty(user.PasswordHash));
        _userRepositoryMock.Verify(r => r.createUserModel(It.IsAny<UserModel>()), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_ThrowsException_WhenUsernameExists()
    {
        // Arrange
        var existingUser = new UserModel { Username = "existing" };
        _userRepositoryMock.Setup(r => r.getUserModelByUsername("existing"))
            .ReturnsAsync(existingUser);

        var registerDto = new UserRegisterDto
        {
            Username = "existing",
            Password = "password"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _authService.RegisterAsync(registerDto));
    }

    [Fact]
    public async Task LoginAsync_ReturnsToken_WhenCredentialsAreValid()
    {
        // Arrange
        var password = "password123";
        var hashed = BCrypt.Net.BCrypt.HashPassword(password);
        var user = new UserModel { Id = 1, Username = "loginuser", PasswordHash = hashed };

        _userRepositoryMock.Setup(r => r.getUserModelByUsername("loginuser"))
            .ReturnsAsync(user);

        var loginDto = new UserLoginDto
        {
            Username = "loginuser",
            Password = password
        };

        // Act
        var result = await _authService.LoginAsync(loginDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Id, result!.UserId);
        Assert.Equal(user.Username, result.Username);
        Assert.False(string.IsNullOrEmpty(result.Token));
    }

    [Fact]
    public async Task LoginAsync_ReturnsNull_WhenCredentialsAreInvalid()
    {
        // Arrange
        _userRepositoryMock.Setup(r => r.getUserModelByUsername("wronguser"))
            .ReturnsAsync((UserModel?)null);

        var loginDto = new UserLoginDto
        {
            Username = "wronguser",
            Password = "password"
        };

        // Act
        var result = await _authService.LoginAsync(loginDto);

        // Assert
        Assert.Null(result);
    }
}
