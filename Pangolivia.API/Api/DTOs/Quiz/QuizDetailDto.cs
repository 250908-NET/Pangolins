using System.Collections.Generic;

namespace Pangolivia.API.DTOs;

public class QuizDetailDto
{
    public int Id { get; set; }
    public string QuizName { get; set; } = string.Empty;
    public int CreatedByUserId { get; set; }
    public string CreatorUsername { get; set; } = string.Empty;
    public List<QuestionDto> Questions { get; set; } = new();
}
