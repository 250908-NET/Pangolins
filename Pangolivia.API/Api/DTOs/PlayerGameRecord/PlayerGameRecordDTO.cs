namespace Pangolivia.API.DTOs
{
    public record PlayerGameRecordDto
    {
        public int? Id { get; init; }
        public int? UserId { get; init; }
        public required string Username { get; set; }
        public int? GameRecordId { get; init; }
        public double Score { get; init; }
        // public int? Rank { get; init; }
    }

    public record CreatePlayerGameRecordDto
    {
        public int? GameRecordId { get; init; }
        public int UserId { get; init; }
        public double Score { get; init; }
    }

    public record UpdatePlayerGameRecordDto
    {
        public double Score { get; set; }
    }

    public record LeaderboardDto
    {
        public string Username { get; set; } = string.Empty;
        public double Score { get; set; }
        public int Rank { get; set; }
    }
}
