using Microsoft.AspNetCore.Mvc;
using Pangolivia.API.DTOs;
using Pangolivia.API.Services;

namespace Pangolivia.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto userRegisterDto)
        {
            try
            {
                // Use the alternative register method as it's more complete
                var user = await _authService.RegisterAsync(userRegisterDto);
                _logger.LogInformation("User {Username} registered successfully.", user.Username);
                return Ok(new { Message = "Registration successful" });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Registration failed for user {Username}: {Message}", userRegisterDto.Username, ex.Message);
                return Conflict(new { Message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] UserLoginDto userLoginDto)
        {
            var loginResponse = await _authService.LoginAsync(userLoginDto);

            if (loginResponse == null)
            {
                _logger.LogWarning("Login failed for user {Username}.", userLoginDto.Username);
                return Unauthorized(new { Message = "Invalid username or password." });
            }
            _logger.LogInformation("User {Username} logged in successfully.", userLoginDto.Username);
            return Ok(loginResponse);
        }
    }
}