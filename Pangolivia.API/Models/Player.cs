namespace Pangolivia.API.Models;

public class Player
{
    public int Id { get; set; }
    public User User { get; set; } = null!;
    public Room Room { get; set; } = null!;
    public int PlayerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Rank { get; set; }
}