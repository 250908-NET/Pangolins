namespace Pangolivia.API.DTOs
{
    public record GameRecordDto
    {
        public int? Id { get; init; }
        public int? HostUserId { get; init; }
        public required string HostUsername { get; init; }
        public int? QuizId { get; init; }
        public required string QuizName { get; init; }
        public List<PlayerGameRecordDto> PlayerScores { get; init; } = new();
        public DateTime dateTimeCompleted { get; init; }
    }

    public record CreateGameRecordDto
    {
        public int HostUserId { get; init; }
        public int QuizId { get; init; }
        public List<CreatePlayerGameRecordDto> PlayerScores { get; init; } = new();
        public DateTime dateTimeCompleted { get; init; }
    }
}
