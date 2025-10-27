using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pangolivia.API.DTOs;
using Pangolivia.API.Models;
using Pangolivia.API.Repositories;
using Pangolivia.API.Services;

namespace Pangolivia.API.Controllers
{
    [ApiController]
    [Route("api/games")]
    [Authorize]
    public class GamesController : ControllerBase
    {
        private readonly IGameManagerService _gameManager;
        private readonly IQuizRepository _quizRepository; // Inject the quiz repository
        private readonly ILogger<GamesController> _logger; // Inject logger for better diagnostics
        private readonly UserService _userService;

        // Update the constructor
        public GamesController(
            IGameManagerService gameManager,
            IQuizRepository quizRepository,
            ILogger<GamesController> logger,
            UserService userService
        )
        {
            _gameManager = gameManager;
            _quizRepository = quizRepository;
            _logger = logger;
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateGame([FromBody] CreateGameRequestDto request)
        {
            var auth0sub = User.FindFirstValue(ClaimTypes.NameIdentifier);
            UserModel userModel = null;
            try
            {
                if (auth0sub is null) throw new Exception();
                userModel = await _userService.getOrCreateUser(auth0sub);
            }
            catch
            {
                return Unauthorized();
            }

            try
            {
                var roomCode = await _gameManager.CreateGame(request.QuizId, userModel.Id, userModel.Username);
                return Ok(new { roomCode });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating game for user {Username}", userModel.Username);
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
