using Microsoft.AspNetCore.Mvc;
using Pangolivia.API.Models;
using Pangolivia.API.Repositories;

namespace Pangolivia.API.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuizController : ControllerBase
    {
        private readonly IQuizRepository _repo;

        public QuizController(IQuizRepository repo)
        {
            _repo = repo;
        }

        // GET: api/quiz
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetAll()
        {
            var quizzes = await _repo.GetAllAsync();
            // Manual mapping to a lightweight summary payload
            var result = quizzes.Select(q => new {
                id = q.Id,
                name = q.QuizName,
                createdByUserId = q.CreatedByUserId
            });
            return Ok(result);
        }

        // GET: api/quiz/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<object>> GetById(int id)
        {
            var quiz = await _repo.GetByIdWithDetailsAsync(id) ?? await _repo.GetByIdAsync(id);
            if (quiz == null) return NotFound();

            // Manual mapping to a detail payload (adjust properties to your model)
            var result = new {
                id = quiz.Id,
                name = quiz.QuizName,
                createdByUserId = quiz.CreatedByUserId,
                // questions = quiz.Questions?.Select(q => new { q.Id, q.Text }) // uncomment/adjust if you have nav
            };
            return Ok(result);
        }

        // POST: api/quiz
        [HttpPost]
        public async Task<ActionResult<object>> Create([FromBody] CreateQuizRequestDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            // Map dto -> model (adjust property names to match your dto)
            var model = new QuizModel
            {
                QuizName = dto.Name,
                CreatedByUserId = dto.CreatedByUserId
            };

            var created = await _repo.AddAsync(model);
            var saved = await _repo.SaveChangesAsync();
            if (!saved) return StatusCode(500, "Failed to persist quiz.");

            // return 201 with location
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, new {
                id = created.Id,
                name = created.QuizName,
                createdByUserId = created.CreatedByUserId
            });
        }

        // PUT: api/quiz/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateQuizRequestDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return NotFound();

            // Map dto -> existing model
            existing.QuizName = dto.Name ?? existing.QuizName;
            // keep CreatedByUserId as-is unless updating is intended

            _repo.UpdateAsync(existing);
            var saved = await _repo.SaveChangesAsync();
            if (!saved) return StatusCode(500, "Failed to save changes.");

            return NoContent();
        }

        // DELETE: api/quiz/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return NotFound();

            _repo.DeleteAsync(existing);
            var saved = await _repo.SaveChangesAsync();
            if (!saved) return StatusCode(500, "Failed to delete quiz.");

            return NoContent();
        }
    }

    // --- Minimal DTOs used here (adjust if your team already has these) ---
    public class CreateQuizRequestDto
    {
        public string Name { get; set; } = string.Empty;
        public int CreatedByUserId { get; set; }
    }

    public class UpdateQuizRequestDto
    {
        public string? Name { get; set; }
    }
}
