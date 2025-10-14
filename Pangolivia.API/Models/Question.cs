namespace Pangolivia.API.Models;

public class Question
{
    public int QuestionId { get; set; }
    public int QuizId { get; set; }
    public string QuestionText { get; set; }
    public List<Choices> Choices { get; set; }
}