using Microsoft.AspNetCore.Mvc;
using Pangolivia.API.Services;
using Pangolivia.API.DTOs;


namespace Pangolivia.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuizController : ControllerBase
    {
        private readonly IQuizService _quizService;
        private readonly ILogger<QuizController> _logger;

        public QuizController(IQuizService quizService, ILogger<QuizController> logger)
        {
            _quizService = quizService;
            _logger = logger;
        }

        // POST: api/Quiz
        [HttpPost]
        public async Task<ActionResult<QuizDetailDto>> CreateQuiz([FromBody] CreateQuizRequestDto requestDto, [FromQuery] int creatorUserId)
        {
            _logger.LogInformation("Creating a new quiz by user {UserId}.", creatorUserId);
            var result = await _quizService.CreateQuizAsync(requestDto, creatorUserId);
            return CreatedAtAction(nameof(GetQuizById), new { quizId = result.Id }, result);
        }

        // PUT: api/Quiz/{quizId}
        [HttpPut("{quizId}")]
        public async Task<ActionResult<QuizDetailDto>> UpdateQuiz(int quizId, [FromBody] UpdateQuizRequestDto requestDto, [FromQuery] int currentUserId)
        {
            _logger.LogInformation("Updating quiz {QuizId} by user {UserId}.", quizId, currentUserId);
            try
            {
                var result = await _quizService.UpdateQuizAsync(quizId, requestDto, currentUserId);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                _logger.LogWarning("Quiz {QuizId} not found for update.", quizId);
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                _logger.LogWarning("User {UserId} not authorized to update quiz {QuizId}.", currentUserId, quizId);
                return Forbid();
            }
        }

        // DELETE: api/Quiz/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuiz(int id, [FromQuery] int currentUserId)
        {
            _logger.LogInformation("Deleting quiz {QuizId} by user {UserId}.", id, currentUserId);
            try
            {
                await _quizService.DeleteQuizAsync(id, currentUserId);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                _logger.LogWarning("Quiz {QuizId} not found for deletion.", id);
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                _logger.LogWarning("User {UserId} not authorized to delete quiz {QuizId}.", currentUserId, id);
                return Forbid();
            }
        }

        // GET: api/Quiz
        [HttpGet]
        public async Task<ActionResult<List<QuizSummaryDto>>> GetAllQuizzes()
        {
            _logger.LogInformation("Fetching all quizzes.");
            var result = await _quizService.GetAllQuizzesAsync();
            return Ok(result);
        }

        // GET: api/Quiz/{quizId}
        [HttpGet("{quizId}")]
        public async Task<ActionResult<QuizDetailDto>> GetQuizById(int quizId)
        {
            _logger.LogInformation("Fetching quiz with ID {QuizId}.", quizId);
            var result = await _quizService.GetQuizByIdAsync(quizId);
            if (result == null)
            {
                _logger.LogWarning("Quiz with ID {QuizId} not found.", quizId);
                return NotFound();
            }
            return Ok(result);
        }

        // GET: api/Quiz/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<QuizSummaryDto>>> GetQuizzesByUserId(int userId)
        {
            _logger.LogInformation("Fetching quizzes for user {UserId}.", userId);
            var result = await _quizService.GetQuizzesByUserIdAsync(userId);
            return Ok(result);
        }

        // GET: api/Quiz/search
        [HttpGet("search")]
        public async Task<ActionResult<List<QuizSummaryDto>>> FindQuizzesByName([FromQuery] string query)
        {
            _logger.LogInformation("Searching quizzes by name: {Query}.", query);
            var result = await _quizService.FindQuizzesByNameAsync(query);
            return Ok(result);
        }
    }
}