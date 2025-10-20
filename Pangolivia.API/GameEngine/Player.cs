using Pangolivia.API.DTOs;

/*
 * Stores player state for active GameSessions.
*/
public class Player
{
    public int UserId { get; }
    public string Username { get; }
    public string ConnectionId { get; set; }
    public string? AnswerToCurrentQuestion { get; set; }
    public int CurrentScore { get; private set; }

    public Player(UserDto user, string connectionId)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));
        if (connectionId == null) throw new ArgumentNullException(nameof(connectionId));

        UserId = user.Id;
        Username = user.Username;
        ConnectionId = connectionId;
        AnswerToCurrentQuestion = null;
        CurrentScore = 0;
    }

    public void ResetAnswer() => AnswerToCurrentQuestion = null;
    public void AddPoints(int points) => CurrentScore += points;
}
