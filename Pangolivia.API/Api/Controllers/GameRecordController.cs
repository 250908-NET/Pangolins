namespace Pangolivia.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameRecordController : ControllerBase
    {
        private readonly GameRecordService _gameRecordService;
        private readonly ILogger<GameRecordController> _logger;

        public GameRecordController(GameRecordService gameRecordService, ILogger<GameRecordController> logger)
        {
            _gameRecordService = gameRecordService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateGame([FromBody] CreateGameRecordDto dto)
        {
            try
            {
                var result = await _gameRecordService.CreateGameAsync(dto);
                return CreatedAtAction(nameof(GetGameById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating game");
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllGames()
        {
            var games = await _gameRecordService.GetAllGamesAsync();
            return Ok(games);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetGameById(int id)
        {
            var game = await _gameRecordService.GetGameByIdAsync(id);
            if (game == null)
                return NotFound($"Game with ID {id} not found.");

            return Ok(game);
        }

        [HttpPut("{id:int}/complete")]
        public async Task<IActionResult> CompleteGame(int id)
        {
            try
            {
                var result = await _gameRecordService.CompleteGameAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing game {GameId}", id);
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteGame(int id)
        {
            try
            {
                await _gameRecordService.DeleteGameAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting game {GameId}", id);
                return NotFound(ex.Message);
            }
        }

        [HttpGet("host/{hostUserId:int}")]
        public async Task<IActionResult> GetGamesByHost(int hostUserId)
        {
            var games = await _gameRecordService.GetGamesByHostAsync(hostUserId);
            return Ok(games);
        }

        [HttpGet("quiz/{quizId:int}")]
        public async Task<IActionResult> GetGamesByQuiz(int quizId)
        {
            var games = await _gameRecordService.GetGamesByQuizAsync(quizId);
            return Ok(games);
        }
    }
}
