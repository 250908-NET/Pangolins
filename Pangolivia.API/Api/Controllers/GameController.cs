using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pangolivia.API.DTOs;
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
        private readonly IQuizRepository _quizRepository;

        public GamesController(IGameManagerService gameManager, IQuizRepository quizRepository)
        {
            _gameManager = gameManager;
            _quizRepository = quizRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateGame([FromBody] CreateGameRequestDto request)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out var userId))
            {
                return Unauthorized("Invalid user identifier.");
            }

            try
            {
                var roomCode = await _gameManager.CreateGame(request.QuizId, userId);
                return Ok(new { roomCode });
            }
            catch (Exception ex)
            {
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
