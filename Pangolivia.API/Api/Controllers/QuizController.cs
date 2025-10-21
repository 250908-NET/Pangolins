using Microsoft.AspNetCore.Mvc;
using Pangolivia.API.Services;
using Pangolivia.API.DTOs;
using System.Net.Http;
using System.Threading;


namespace Pangolivia.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuizController : ControllerBase
    {
        private readonly IQuizService _quizService;
        private readonly IAiQuizService _aiQuizService;
        private readonly ILogger<QuizController> _logger;

        public QuizController(IQuizService quizService, IAiQuizService aiQuizService, ILogger<QuizController> logger)
        {
            _quizService = quizService;
            _aiQuizService = aiQuizService;
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

        // POST: api/Quiz/ai/generate
        [HttpPost("ai/generate")]
        public async Task<ActionResult<List<QuestionDto>>> GenerateQuestionsWithAi([FromBody] GenerateQuizAiRequestDto requestDto, CancellationToken ct)
        {
            if (requestDto.NumberOfQuestions <= 0 || requestDto.NumberOfQuestions > 50)
            {
                return BadRequest("NumberOfQuestions must be between 1 and 50.");
            }

            _logger.LogInformation("Generating {Count} AI questions on topic '{Topic}' (difficulty: {Difficulty}).", requestDto.NumberOfQuestions, requestDto.Topic, requestDto.Difficulty);

            try
            {
                var questions = await _aiQuizService.GenerateQuestionsAsync(requestDto.Topic, requestDto.NumberOfQuestions, requestDto.Difficulty, ct);
                return Ok(questions);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "AI provider request failed");
                return StatusCode(502, "AI provider error");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AI generation failed");
                return StatusCode(500, "Failed to generate questions");
            }
        }
    }
}
