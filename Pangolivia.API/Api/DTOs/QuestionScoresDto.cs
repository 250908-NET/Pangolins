namespace Pangolivia.API.DTOs;

public record QuestionScoresDto
{
    public required string Question { get; init; }
    public required string Answer { get; init; }
    public List<PlayerQuestionScoresDto> PlayerScores { get; init; } = new();
}

public record PlayerQuestionScoresDto
{
    public int UserId { get; init; }
    public required string Username { get; init; }
    public double Score { get; init; }
    public int TotalScore { get; init; }
}