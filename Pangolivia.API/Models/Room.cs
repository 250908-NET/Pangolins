namespace Pangolivia.API.Models;

public class Room
{
    public int Id { get; set; }
    public List<Player> Players { get; set; } = new List<Player>();
    public string GameState { get; set; } = "lobby"; // lobby, in-game, finished
    public string Status { get; set; } = "waiting"; // waiting, active, ended
    public int CurrentQuestionNumber { get; set; } = 0;
    public int? EndTime { get; set; }
    public int CurrentQuestion { get; set; } = 0;
    public string LeaderBoard { get; set; } = string.Empty;
}