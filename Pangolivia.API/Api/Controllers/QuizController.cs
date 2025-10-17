using Microsoft.AspNetCore.Mvc;
using Pangolivia.API.Models;
using Pangolivia.API.Repositories;
using Pangolivia.API.Services.External;


namespace Pangolivia.API.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuizController : ControllerBase
    {
        private readonly IQuizRepository _repo;

        // public QuizController(IQuizRepository repo)
        // {
        //     _repo = repo;
        // }

        // Constructor with Trivia API client injection

        private readonly ITriviaApiClient _trivia;
        public QuizController(IQuizRepository repo, ITriviaApiClient trivia)
        {
            _repo = repo;
            _trivia = trivia;
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

        // GET: api/quiz/external
        [HttpGet("external")]
        public async Task<ActionResult<object>> GetExternal(
            [FromQuery] int amount = 5,
            [FromQuery] int? category = null,     // e.g., 9 = General Knowledge
            [FromQuery] string? difficulty = null,// "easy" | "medium" | "hard"
            [FromQuery] string type = "multiple", // "multiple" | "boolean"
            CancellationToken ct = default)
        {
            var data = await _trivia.FetchAsync(amount, category, difficulty, type, ct);

            // Minimal transformation so the shape is nice for your frontend
            var result = new
            {
                responseCode = data.ResponseCode,
                items = data.Results.Select((r, idx) => new {
                    id = idx + 1,
                    category = r.Category,
                    difficulty = r.Difficulty,
                    question = r.Question,
                    correct = r.CorrectAnswer,
                    incorrect = r.IncorrectAnswers
                })
            };

            return Ok(result);
        }




    }

    // --- Minimal DTOs used here
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
