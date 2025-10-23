using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pangolivia.API.DTOs;
using Pangolivia.API.Models;
using Pangolivia.API.Services;

namespace Pangolivia.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuizController : ControllerBase
    {
        private readonly IQuizService _quizService;
        private readonly IAiQuizService _aiQuizService;
        private readonly ILogger<QuizController> _logger;
        private readonly UserService _userService;

        public QuizController(
            IQuizService quizService,
            IAiQuizService aiQuizService,
            ILogger<QuizController> logger,
            UserService userService
        )
        {
            _quizService = quizService;
            _aiQuizService = aiQuizService;
            _logger = logger;
            _userService = userService;
        }

        // POST: api/Quiz
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<QuizDetailDto>> CreateQuiz(
            [FromBody] CreateQuizRequestDto requestDto
        )
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

            var creatorUserId = userModel.Id;
            _logger.LogInformation("Creating a new quiz by user {UserId}.", creatorUserId);
            var result = await _quizService.CreateQuizAsync(requestDto, creatorUserId);
            return CreatedAtAction(nameof(GetQuizById), new { quizId = result.Id }, result);
        }

        // PUT: api/Quiz/{quizId}
        [HttpPut("{quizId}")]
        [Authorize]
        public async Task<ActionResult<QuizDetailDto>> UpdateQuiz(
            int quizId,
            [FromBody] UpdateQuizRequestDto requestDto
        )
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

            var currentUserId = userModel.Id;

            _logger.LogInformation(
                "Updating quiz {QuizId} by user {UserId}.",
                quizId,
                currentUserId
            );
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
                _logger.LogWarning(
                    "User {UserId} not authorized to update quiz {QuizId}.",
                    currentUserId,
                    quizId
                );
                return Forbid();
            }
        }

        // DELETE: api/Quiz/{id}
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteQuiz(int id)
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

            var currentUserId = userModel.Id;

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
                _logger.LogWarning(
                    "User {UserId} not authorized to delete quiz {QuizId}.",
                    currentUserId,
                    id
                );
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
        public async Task<ActionResult<List<QuizSummaryDto>>> FindQuizzesByName(
            [FromQuery] string query
        )
        {
            _logger.LogInformation("Searching quizzes by name: {Query}.", query);
            var result = await _quizService.FindQuizzesByNameAsync(query);
            return Ok(result);
        }

        // POST: api/Quiz/ai/generate
        [HttpPost("ai/generate")]
        public async Task<ActionResult<List<QuestionDto>>> GenerateQuestionsWithAi(
            [FromBody] GenerateQuizAiRequestDto requestDto,
            CancellationToken ct
        )
        {
            if (requestDto.NumberOfQuestions <= 0 || requestDto.NumberOfQuestions > 50)
            {
                return BadRequest("NumberOfQuestions must be between 1 and 50.");
            }

            _logger.LogInformation(
                "Generating {Count} AI questions on topic '{Topic}' (difficulty: {Difficulty}).",
                requestDto.NumberOfQuestions,
                requestDto.Topic,
                requestDto.Difficulty
            );

            try
            {
                var questions = await _aiQuizService.GenerateQuestionsAsync(
                    requestDto.Topic,
                    requestDto.NumberOfQuestions,
                    requestDto.Difficulty,
                    ct
                );
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
