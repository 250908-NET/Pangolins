public class PlayerGameRecordDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string? Username { get; set; }
    public int GameRecordId { get; set; }
    public DateTime? GameCompletedAt { get; set; }
    public double Score { get; set; }
}