using Microsoft.AspNetCore.Mvc;
using Pangolivia.API.Services;
using Pangolivia.API.DTOs;
using Pangolivia.API.Models;


namespace Pangolivia.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<QuizController> _logger;

        public UserController(IUserService userService, ILogger<QuizController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        // POST: api/Quiz
        [HttpPost]
        public async Task<ActionResult<CreateUserDTO>> CreateUser([FromBody] UserModel User)
        {
            _logger.LogInformation($"Creating a new user. with username {User.Username}", User.Username);
            UserModel result = await _userService.createUserAsync(User);
            return CreatedAtAction(nameof(result.Username), User);
        }

        [HttpGet]
        public async Task<ActionResult<CreateUserDTO>> GetAllUsers()
        {
            _logger.LogInformation("Get all Users from database");
            List<UserModel> result = await _userService.getAllUsersAsync();
            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<UserModel>> GetUserByID(int id)
        {
            _logger.LogInformation($"Get Users with id:{id} from database");
            UserModel result = await _userService.getUserByIdAsync(id);
            return Ok(result);
        }
        [HttpGet("{username}")]
        public async Task<ActionResult<UserModel>> GetUserByUsername(string username)
        {
            _logger.LogInformation($"Get Users with username:{username} from database");
            UserModel result = await _userService.findUserByUsernameAsync(username);
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);
        }

        [HttpPatch("{userID}")]
        public async Task<ActionResult<UserModel>> updateUser(int userID, [FromBody] object OBJ)
        {
            _logger.LogInformation($"Updating userID:{userID}");
            UserModel result = await _userService.updateUserAsync(userID, OBJ);
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);
        }


        [HttpDelete("{userID}")]
        public async Task<IActionResult> deleteUser(int userID)
        {
            _logger.LogInformation($"removing  userID:{userID}");
            await _userService.deleteUserAsync(userID);
            return Ok(new { message = $"removed User with id:{userID}" });
        }










    }
}
