namespace Pangolivia.API.DTOs;

public class PlayerGameRecordDto
{
    public int? Id { get; init; }
    public int UserId { get; init; }
    public string? Username { get; set; }
    public int? GameRecordId { get; init; }
    public double Score { get; init; }
}
