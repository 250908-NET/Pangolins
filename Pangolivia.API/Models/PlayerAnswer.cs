namespace Pangolivia.API.Models;

public class PlayerAnswer
{
    public int Id { get; set; }
    public User User { get; set; } = null!;
    public Room Room { get; set; } = null!;
    public int PlayerId { get; set; }
    public int ChoiceId { get; set; }
}