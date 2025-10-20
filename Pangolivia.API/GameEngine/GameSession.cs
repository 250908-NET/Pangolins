using System.Collections.Concurrent;
using Pangolivia.API.DTOs;
using Pangolivia.API.Models;

namespace Pangolivia.API.GameEngine;

public class GameSession
{
    // immutable after construction
    public int Id { get; }
    public string Name { get; }
    public int HostUserId { get; }
    public QuizModel Quiz { get; }

    private int CurrentQuestionIndex = -1;

    public bool HasEnded { get; private set; } = false;

    private readonly ConcurrentDictionary<int, Player> Players;

    public GameSession(int id, string name, int hostUserId, QuizModel quiz)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(quiz);

        Id = id;
        Name = name;
        HostUserId = hostUserId;
        Quiz = quiz;
        Players = new ConcurrentDictionary<int, Player>();
    }

    /// <summary>
    /// Has the game started?
    /// </summary>
    public bool HasGameStarted()
    {
        return CurrentQuestionIndex >= 0;
    }

    /// <summary>
    /// Registers a user as a player in the game.
    /// </summary>
    ///
    /// <param name="user">A UserDto describing the user.</param>
    /// <param name="connectionId">The user's SignalR connection ID.</param>
    /// <returns>A UserDto for the registered user.</returns>
    public UserDto RegisterPlayer(UserDto user, string connectionId)
    {
        if (Players.ContainsKey(user.Id))
        {
            throw new InvalidOperationException("User is already registered as a player in this game session.");
        }

        var player = new Player(user, connectionId);
        if (!Players.TryAdd(user.Id, player))
        {
            throw new InvalidOperationException("Failed to register player. A player with the same id may already exist.");
        }
        return user;
    }

    /// <summary>
    /// Is there another question left in the quiz?
    /// </summary>
    public bool HasNextQuestion()
    {
        if (HasEnded) return false;

        ICollection<QuestionModel> questions = Quiz.Questions;
        if (questions == null) return false;
        return CurrentQuestionIndex < questions.Count - 1;
    }

    // public QuestionForPlayerDto NextQuestion() { }

    /// <summary>
    /// Register the player's chosen answer.
    /// </summary>
    ///
    /// <param name="playerId">The userId of the player.</param>
    /// <param name="answer">The answer they chose.</param>
    public void AnswerQuestionForPlayer(int playerId, string answer)
    {
        if (!Players.TryGetValue(playerId, out Player? player))
        {
            throw new InvalidOperationException("Player not found in this game session.");
        }

        if (player == null)
        {
            throw new InvalidOperationException("Player entry is null for this game session.");
        }

        player.AnswerToCurrentQuestion = answer;
    }

    // TODO: Implement EndQuestionRound
    // public QuestionScoresDto EndQuestionRound() { }

    /// <summary>
    /// End the game and return the final scores. If the game is already ended,
    /// simply return the final scores.
    /// </summary>
    ///
    /// <returns>A GameRecordDto for the completed game.</returns>
    public GameRecordDto EndGameAndGetFinalGameRecord()
    {
        HasEnded = true;

        if (Players == null || Players.IsEmpty)
            throw new InvalidOperationException("Cannot end game with no players.");

        var gameRecordDto = new GameRecordDto
        {
            QuizId = Quiz.Id,
            HostUserId = HostUserId,
            PlayerScores = Players.Values
                .Select(p => new PlayerGameRecordDto
                {
                    UserId = p.UserId,
                    Score = p.CurrentScore
                })
                .ToList(),
            CompletedAt = DateTimeOffset.UtcNow,
        };

        return gameRecordDto;
    }
}
