using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Pangolivia.Services;
using Pangolivia.Models;
using Pangolivia.DTOs;

namespace Pangolivia.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuizController : ControllerBase
    {
        private readonly IQuizService _service;
        private readonly IMapper _mapper;
        private readonly ILogger<QuizController> _logger;

        public QuizController(IQuizService service, IMapper mapper, ILogger<QuizController> logger)
        {
            _service = service;
            _mapper = mapper;
            _logger = logger;
        }

        // GET: api/quiz
        [HttpGet(Name = "GetAllQuizzes")]
        public async Task<ActionResult<IEnumerable<QuizDto>>> GetAll()
        {
            _logger.LogInformation("Fetching all quizzes.");
            var quizzes = await _service.GetAllAsync();
            var quizDtos = _mapper.Map<IEnumerable<QuizDto>>(quizzes);
            return Ok(quizDtos);
        }

        // GET: api/quiz/{id}
        [HttpGet("{id}", Name = "GetQuizById")]
        public async Task<ActionResult<QuizDto>> GetById(int id)
        {
            _logger.LogInformation("Fetching quiz with ID {id}", id);
            var quiz = await _service.GetByIdAsync(id);

            if (quiz == null)
            {
                _logger.LogWarning("Quiz with ID {id} not found.", id);
                return NotFound("Quiz not found");
            }

            var dto = _mapper.Map<QuizDto>(quiz);
            return Ok(dto);
        }

        // POST: api/quiz
        [HttpPost(Name = "CreateQuiz")]
        public async Task<ActionResult<QuizDto>> Create([FromBody] CreateQuizDto dto)
        {
            _logger.LogInformation("Creating new quiz: {quizName}", dto.QuizName);
            var quiz = _mapper.Map<Quiz>(dto);
            var created = await _service.CreateAsync(quiz);
            var result = _mapper.Map<QuizDto>(created);

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        // PUT: api/quiz/{id}
        [HttpPut("{id}", Name = "UpdateQuiz")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateQuizDto dto)
        {
            _logger.LogInformation("Updating quiz with ID {id}", id);
            var quiz = _mapper.Map<Quiz>(dto);
            var success = await _service.UpdateAsync(id, quiz);

            if (!success)
            {
                _logger.LogWarning("Quiz with ID {id} not found for update.", id);
                return NotFound("Quiz not found");
            }

            return NoContent();
        }

        // DELETE: api/quiz/{id}
        [HttpDelete("{id}", Name = "DeleteQuiz")]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("Deleting quiz with ID {id}", id);
            var success = await _service.DeleteAsync(id);

            if (!success)
            {
                _logger.LogWarning("Quiz with ID {id} not found for deletion.", id);
                return NotFound("Quiz not found");
            }

            return NoContent();
        }
    }
}
