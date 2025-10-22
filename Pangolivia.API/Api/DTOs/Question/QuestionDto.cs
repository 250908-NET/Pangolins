namespace Pangolivia.API.DTOs;

public record QuestionDto
{
    public int? Id { get; init; }
    public required string QuestionText { get; init; }
    public required string CorrectAnswer { get; init; }
    public required string Answer2 { get; init; }
    public required string Answer3 { get; init; }
    public required string Answer4 { get; init; }
}
