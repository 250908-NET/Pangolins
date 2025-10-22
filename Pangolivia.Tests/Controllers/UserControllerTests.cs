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
using System.Reflection;

namespace Pangolivia.Tests.Controllers;

public class UserControllerTests
{
    private readonly Mock<IUserService> _userServiceMock;
    private readonly Mock<ILogger<UserController>> _loggerMock;
    private readonly UserController _controller;

    public UserControllerTests()
    {
        _userServiceMock = new Mock<IUserService>();
        _loggerMock = new Mock<ILogger<UserController>>();
        _controller = new UserController(_userServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetAllUsersReturnsOkWithList()
    {
        List<UserSummaryDto> users = new List<UserSummaryDto>
        {
            new UserSummaryDto { Id = 1, Username = "Alice" },
            new UserSummaryDto { Id = 2, Username = "Bob" }
        };
        _userServiceMock.Setup(s => s.getAllUsersAsync()).ReturnsAsync(users);

        ActionResult<IEnumerable<UserSummaryDto>> result = await _controller.GetAllUsers();

        OkObjectResult ok = Assert.IsType<OkObjectResult>(result.Result);
        List<UserSummaryDto> returnValue = Assert.IsType<List<UserSummaryDto>>(ok.Value);
        Assert.Equal(2, returnValue.Count);
    }

    [Fact]
    public async Task GetUserByIdReturnsOkWhenFound()
    {
        UserDetailDto user = new UserDetailDto { Id = 5, Username = "Charlie" };
        _userServiceMock.Setup(s => s.getUserByIdAsync(5)).ReturnsAsync(user);

        ActionResult<UserDetailDto> result = await _controller.GetUserByID(5);

        OkObjectResult ok = Assert.IsType<OkObjectResult>(result.Result);
        UserDetailDto returnValue = Assert.IsType<UserDetailDto>(ok.Value);
        Assert.Equal("Charlie", returnValue.Username);
    }

    [Fact]
    public async Task GetUserByIdReturnsNotFoundWhenMissing()
    {
        _userServiceMock.Setup(s => s.getUserByIdAsync(9)).ReturnsAsync((UserDetailDto?)null);

        ActionResult<UserDetailDto> result = await _controller.GetUserByID(9);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetUserByUsernameReturnsOkWhenFound()
    {
        UserDetailDto user = new UserDetailDto { Id = 1, Username = "Alice" };
        _userServiceMock.Setup(s => s.findUserByUsernameAsync("Alice")).ReturnsAsync(user);

        ActionResult<UserDetailDto> result = await _controller.GetUserByUsername("Alice");

        OkObjectResult ok = Assert.IsType<OkObjectResult>(result.Result);
        UserDetailDto returnValue = Assert.IsType<UserDetailDto>(ok.Value);
        Assert.Equal("Alice", returnValue.Username);
    }

    [Fact]
    public async Task GetUserByUsernameReturnsNotFoundWhenMissing()
    {
        _userServiceMock.Setup(s => s.findUserByUsernameAsync("Ghost")).ReturnsAsync((UserDetailDto?)null);

        ActionResult<UserDetailDto> result = await _controller.GetUserByUsername("Ghost");

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task DeleteUserReturnsOkWhenSuccessful()
    {
        _userServiceMock.Setup(s => s.deleteUserAsync(5)).Returns(Task.CompletedTask);

        IActionResult result = await _controller.deleteUser(5);

        OkObjectResult ok = Assert.IsType<OkObjectResult>(result);
        var val = ok.Value!;
        Assert.NotNull(val);

    }

    [Fact]
    public async Task DeleteUserReturnsNotFoundWhenUserDoesNotExist()
    {
        _userServiceMock.Setup(s => s.deleteUserAsync(99)).ThrowsAsync(new KeyNotFoundException());

        IActionResult result = await _controller.deleteUser(99);

        NotFoundObjectResult notFound = Assert.IsType<NotFoundObjectResult>(result);
        var val = notFound.Value!;
        Assert.NotNull(val);
    }
}

