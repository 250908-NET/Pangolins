public record GameRecordDto
{
    public int? Id { get; init; }
    public int QuizId { get; init; }
    public int HostUserId { get; init; }
    public List<PlayerGameRecordDto> PlayerScores { get; init; } = new();
    public DateTimeOffset CompletedAt { get; init; }
}
