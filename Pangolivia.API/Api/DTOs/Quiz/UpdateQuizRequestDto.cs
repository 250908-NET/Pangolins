using System.Collections.Generic;

namespace Pangolivia.API.DTOs;

public class UpdateQuizRequestDto
{
    public string QuizName { get; set; } = string.Empty;
    public List<QuestionDto> Questions { get; set; } = new();
}
