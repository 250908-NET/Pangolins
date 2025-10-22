using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pangolivia.API.DTOs;
// Add this using statement
using Pangolivia.API.Repositories;
using Pangolivia.API.Services;

namespace Pangolivia.API.Controllers
{
    [ApiController]
    [Route("api/games")]
    [Authorize]
    public class GamesController : ControllerBase
    {
        private readonly GameManagerService _gameManager;
        private readonly IQuizRepository _quizRepository; // Inject the quiz repository
        private readonly ILogger<GamesController> _logger; // Inject logger for better diagnostics

        // Update the constructor
        public GamesController(
            GameManagerService gameManager,
            IQuizRepository quizRepository,
            ILogger<GamesController> logger
        )
        {
            _gameManager = gameManager;
            _quizRepository = quizRepository;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateGame([FromBody] CreateGameRequestDto request)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out var userId))
            {
                _logger.LogWarning("CreateGame failed: Could not parse UserId from token.");
                return Unauthorized("Invalid user identifier.");
            }

            var username = User.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrEmpty(username))
            {
                _logger.LogWarning("CreateGame failed: Username not found in token for UserId {UserId}.", userId);
                return Unauthorized("Username not found in token.");
            }

            _logger.LogInformation(
                "User {Username} ({UserId}) is creating a game with QuizId {QuizId}",
                username,
                userId,
                request.QuizId
            );

            try
            {
                var roomCode = await _gameManager.CreateGame(request.QuizId, userId, username);
                return Ok(new { roomCode });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating game for user {Username}", username);
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{roomCode}/details")]
        public IActionResult GetGameDetails(string roomCode)
        {
            var session = _gameManager.GetGameSession(roomCode);
            if (session == null)
            {
                return NotFound("Game not found.");
            }

            return Ok(new { });
        }
    }
}