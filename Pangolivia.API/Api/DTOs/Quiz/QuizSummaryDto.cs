namespace Pangolivia.API.DTOs;

public class QuizSummaryDto
{
    public int Id { get; set; }
    public string QuizName { get; set; } = string.Empty;
    public int QuestionCount { get; set; }
    public string CreatorUsername { get; set; } = string.Empty;
}
