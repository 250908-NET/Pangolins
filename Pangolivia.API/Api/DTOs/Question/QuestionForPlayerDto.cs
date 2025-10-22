namespace Pangolivia.API.DTOs;

public record QuestionForPlayerDto
{
    public required string QuestionText { get; init; }
    public required string Answer1 { get; init; }
    public required string Answer2 { get; init; }
    public required string Answer3 { get; init; }
    public required string Answer4 { get; init; }
}
