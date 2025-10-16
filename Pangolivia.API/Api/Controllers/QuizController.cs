using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pangolivia.API.DTOs;
using Pangolivia.API.Services;

namespace Pangolivia.API.Controllers;

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

    
    // GET /api/quizzes?name={quiz_name}
    [HttpGet]
    public async Task<ActionResult<List<QuizSummaryDto>>> GetAll([FromQuery] string? name)
    {
        var quizzes = await _quizService.FindQuizzesByNameAsync(name ?? string.Empty);
        return Ok(quizzes);
    }


    // GET /api/quizzes/{quiz_id}
    [HttpGet("{quizId:int}")]
    public async Task<ActionResult<QuizDetailDto>> GetById(int quizId)
    {
        var quiz = await _quizService.GetQuizByIdAsync(quizId);
        if (quiz == null)
            return NotFound("Quiz not found.");

        return Ok(quiz);
    }


    // GET /api/users/{user_id}/quizzes
    [HttpGet("/api/users/{userId:int}/quizzes")]
    public async Task<ActionResult<List<QuizSummaryDto>>> GetByUserId(int userId)
    {
        var quizzes = await _quizService.GetQuizzesByUserIdAsync(userId);
        return Ok(quizzes);
    }

    // POST /api/quizzes
    [HttpPost]
    //[Authorize]
    public async Task<ActionResult<QuizDetailDto>> Create([FromBody] CreateQuizRequestDto dto)
    {
        try
        {
            var created = await _quizService.CreateQuizAsync(dto, userId);
            return CreatedAtAction(nameof(GetById), new { quizId = created.Id }, created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating quiz");
            return BadRequest(ex.Message);
        }
    }

    // PUT /api/quizzes/{quiz_id}
    [HttpPut("{quizId:int}")]
    //[Authorize]
    public async Task<ActionResult<QuizDetailDto>> Update(int quizId, [FromBody] UpdateQuizRequestDto dto)
    {
        try
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
            if (userIdClaim == null)
                return Unauthorized("User ID not found in token.");

            int userId = int.Parse(userIdClaim);
            var updated = await _quizService.UpdateQuizAsync(quizId, dto, userId);
            return Ok(updated);
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Quiz not found.");
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating quiz");
            return BadRequest(ex.Message);
        }
    }


    // DELETE /api/quizzes/{quiz_id}
    [HttpDelete("{quizId:int}")]
    //[Authorize]
    public async Task<IActionResult> Delete(int quizId)
    {
        try
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
            if (userIdClaim == null)
                return Unauthorized("User ID not found in token.");

            int userId = int.Parse(userIdClaim);
            await _quizService.DeleteQuizAsync(quizId, userId);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Quiz not found.");
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting quiz");
            return BadRequest(ex.Message);
        }
    }
}
