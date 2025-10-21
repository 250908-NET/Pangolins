using Microsoft.AspNetCore.Mvc;
using Pangolivia.API.Services;
using Pangolivia.API.DTOs;

namespace Pangolivia.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserSummaryDto>>> GetAllUsers()
        {
            _logger.LogInformation("Get all Users from database");
            var result = await _userService.getAllUsersAsync();
            return Ok(result);
        }
        
        [HttpGet("ById/{id}")]
        public async Task<ActionResult<UserDetailDto>> GetUserByID(int id)
        {
            _logger.LogInformation($"Get User with id:{id} from database");
            var result = await _userService.getUserByIdAsync(id);
            if (result == null)
            {
                _logger.LogWarning("User with ID {id} not found.", id);
                return NotFound();
            }
            return Ok(result);
        }
        
        [HttpGet("{username}")]
        public async Task<ActionResult<UserDetailDto>> GetUserByUsername(string username)
        {
            _logger.LogInformation($"Get User with username:{username} from database");
            var result = await _userService.findUserByUsernameAsync(username);
            if (result == null)
            {
                _logger.LogInformation("User with username {username} not found.", username);
                return NotFound();
            }
            return Ok(result);
        }

        [HttpDelete("{userID}")]
        public async Task<IActionResult> deleteUser(int userID)
        {
            _logger.LogInformation($"Removing user with ID:{userID}");
            try
            {
                await _userService.deleteUserAsync(userID);
                return Ok(new { message = $"Removed User with id:{userID}" });
            }
            catch (KeyNotFoundException)
            {
                _logger.LogWarning("Attempted to delete non-existent user with ID {userID}", userID);
                return NotFound(new { message = $"User with id:{userID} not found." });
            }
        }
    }
}