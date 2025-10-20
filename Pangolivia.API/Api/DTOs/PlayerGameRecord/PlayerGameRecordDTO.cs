namespace Pangolivia.API.DTOs
{
    public class PlayerGameRecordDto
    {
        public int Id { get; set; }
        public int GameRecordId { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public double score { get; set; }
        public DateTime? GameCompletedAt { get; set; }
    }

    public class CreatePlayerGameRecordDto
    {
        public int GameRecordId { get; set; }
        public int UserId { get; set; }
        public double score { get; set; }
    }

    public class UpdatePlayerGameRecordDto
    {
        public double score { get; set; }
    }

    public class LeaderboardDto
    {
        public string Username { get; set; } = string.Empty;
        public double score { get; set; }
        public int Rank { get; set; }
    }
}