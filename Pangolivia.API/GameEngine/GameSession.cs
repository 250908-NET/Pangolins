using System.Collections.Concurrent;
using Pangolivia.API.DTOs;
using Pangolivia.API.Models;

namespace Pangolivia.API.GameEngine;

public class GameSession
{
    public int Id { get; }
    public string Name { get; }
    public int HostUserId { get; }
    public QuizModel Quiz { get; }

    private int CurrentQuestionIndex = -1;

    private GameStatus Status { get; set; } = GameStatus.Pending;

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
        if (Status == GameStatus.Ended) return false;

        ICollection<QuestionModel> questions = Quiz.Questions;
        if (questions == null) return false;
        return CurrentQuestionIndex < questions.Count - 1;
    }

    /// <summary>
    /// Advances game to the next question round.
    /// </summary>
    /// <returns>The next question.</returns>
    public QuestionForPlayerDto AdvanceQuestion()
    {
        if (Status == GameStatus.Ended)
        {
            throw new InvalidOperationException("Can not advance the question in a game that has already ended.");
        }
        else if (Status == GameStatus.ActiveQuestion)
        {
            throw new InvalidOperationException("Can not advance the question when the current question round hasn't ended yet.");
        }

        ICollection<QuestionModel> questions = Quiz.Questions;
        if (questions == null || CurrentQuestionIndex >= questions.Count - 1)
        {
            throw new InvalidOperationException("There are no more questions left to advance to.");
        }

        CurrentQuestionIndex++;
        Status = GameStatus.ActiveQuestion;

        QuestionModel currentQuestion = questions.ElementAt(CurrentQuestionIndex);

        var answers = new List<string>
        {
            currentQuestion.CorrectAnswer,
            currentQuestion.Answer2,
            currentQuestion.Answer3,
            currentQuestion.Answer4,
        };

        // Fisher–Yates shuffle
        for (int i = answers.Count - 1; i > 0; i--)
        {
            int j = Random.Shared.Next(i + 1);
            (answers[i], answers[j]) = (answers[j], answers[i]);
        }

        return new QuestionForPlayerDto
        {
            QuestionText = currentQuestion.QuestionText,
            Answer1 = answers[0],
            Answer2 = answers[1],
            Answer3 = answers[2],
            Answer4 = answers[3],
        };
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

    /// <summary>
    /// Returns the correct answer and the score increments for this question round (not the total
    /// score) for each player. Answers for this round are no longer accepted after this method is
    /// called.
    /// </summary>
    ///
    /// <returns>A QuestionScoresDto describing the correct answer and players' score increments
    /// (not total scores).</returns>
    public QuestionScoresDto EndQuestionRound()
    {
        if (Status != GameStatus.ActiveQuestion)
        {
            throw new InvalidOperationException("Cannot end question round when there is no active question.");
        }

        Status = GameStatus.Pending;

        ICollection<QuestionModel> questions = Quiz.Questions;

        QuestionModel currentQuestion = questions.ElementAt(CurrentQuestionIndex);
        string correctAnswer = currentQuestion.CorrectAnswer;

        var playerScores = new List<PlayerQuestionScoresDto>();

        foreach (Player player in Players.Values)
        {
            int scoreIncrement = 0;
            if (player.AnswerToCurrentQuestion != null &&
                player.AnswerToCurrentQuestion.Equals(correctAnswer, StringComparison.OrdinalIgnoreCase))
            {
                // TODO: Factor time into calculating the score increment for a correct answer.
                scoreIncrement = 1;
                player.AddPoints(scoreIncrement);
            }

            playerScores.Add(new PlayerQuestionScoresDto
            {
                UserId = player.UserId,
                Username = player.Username,
                Score = scoreIncrement,
            });

            player.ResetAnswer();
        }

        return new QuestionScoresDto
        {
            Question = currentQuestion.QuestionText,
            Answer = correctAnswer,
            PlayerScores = playerScores,
        };
    }

    /// <summary>
    /// End the game and return the final scores. If the game is already ended,
    /// simply return the final scores.
    /// </summary>
    ///
    /// <returns>A GameRecordDto for the completed game.</returns>
    public CreateGameRecordDto EndGameAndGetFinalGameRecord()
    {
        Status = GameStatus.Ended;

        var gameRecordDto = new CreateGameRecordDto
        {
            QuizId = Quiz.Id,
            HostUserId = HostUserId,
            PlayerScores = Players.Values
                .Select(p => new CreatePlayerGameRecordDto
                {
                    UserId = p.UserId,
                    Score = p.CurrentScore
                })
                .ToList(),
            dateTimeCompleted = DateTime.UtcNow,
        };

        return gameRecordDto;
    }
}

enum GameStatus
{
    Pending,
    ActiveQuestion,
    Ended,
}
