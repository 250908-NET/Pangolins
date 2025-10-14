namespace Pangolivia.API.Models;

public class Choices
{
    public int ChoiceId { get; set; }
    public int QuestionId { get; set; }
    public string ChoiceText { get; set; }
    public bool IsAnswer { get; set; }
}