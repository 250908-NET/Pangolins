using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Pangolivia.API.Services;

namespace Pangolivia.API.Hubs
{
    [Authorize]
    public class GameHub : Hub
    {
        private readonly GameManagerService _gameManager;
        private readonly ILogger<GameHub> _logger;

        public GameHub(GameManagerService gameManager, ILogger<GameHub> logger)
        {
            _gameManager = gameManager;
            _logger = logger;
        }

        private int GetUserIdFromContext()
        {
            var userIdStr = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdStr, out var userId))
            {
                return userId;
            }
            throw new InvalidOperationException("User ID not found in token.");
        }

        public async Task JoinGame(string roomCode)
        {
            var username = Context.User?.FindFirstValue(ClaimTypes.Name) ?? "Guest";
            int userId;
            try
            {
                userId = GetUserIdFromContext();
            }
            catch (Exception ex)
            {
                 _logger.LogWarning(
                    ex,
                    "JoinGame failed: Could not parse UserId from token for connection {ConnectionId}",
                    Context.ConnectionId
                );
                await Clients.Caller.SendAsync("Error", "Invalid authentication token.");
                Context.Abort();
                return;
            }

            _logger.LogInformation(
                "Player {Username} ({UserId}) attempting to join room {RoomCode}",
                username,
                userId,
                roomCode
            );

            bool success = _gameManager.TryAddPlayerToGame(
                roomCode,
                userId,
                username,
                Context.ConnectionId
            );

            if (success)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, roomCode);

                var gameSession = _gameManager.GetGameSession(roomCode);
                if (gameSession != null)
                {
                    var lobbyDetails = new
                    {
                        QuizName = gameSession.Quiz.QuizName,
                        CreatorUsername = gameSession.Quiz.CreatedByUser?.Username ?? "Unknown",
                        HostUsername = gameSession.HostUsername,
                        QuestionCount = gameSession.Quiz.Questions.Count,
                    };
                    await Clients.Caller.SendAsync("ReceiveLobbyDetails", lobbyDetails);

                    var playerList = gameSession
                        .Players.Values.Select(p => new
                        {
                            p.UserId,
                            p.Username,
                            IsHost = false
                        })
                        .ToList();

                    playerList.Add(
                        new
                        {
                            UserId = gameSession.HostUserId,
                            Username = gameSession.HostUsername,
                            IsHost = true
                        }
                    );
                    
                    await Clients.Group(roomCode).SendAsync("UpdatePlayerList", playerList);
                    _logger.LogInformation(
                        "User {Username} successfully joined room {RoomCode}. Sent updated player list to group.",
                        username,
                        roomCode
                    );
                }
            }
            else
            {
                _logger.LogWarning(
                    "Player {Username} failed to join room {RoomCode}: Room not found or invalid.",
                    username,
                    roomCode
                );
                await Clients.Caller.SendAsync(
                    "Error",
                    "Unable to join game. Room not found or has already started."
                );
            }
        }

        public async Task StartGame(string roomCode)
        {
            try
            {
                var userId = GetUserIdFromContext();
                await _gameManager.StartGame(roomCode, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting game for room {RoomCode}", roomCode);
                await Clients.Caller.SendAsync("Error", $"Failed to start game: {ex.Message}");
            }
        }
        
        // *** NEW METHOD ***
        public Task BeginGame(string roomCode)
        {
            try
            {
                var userId = GetUserIdFromContext();
                _logger.LogInformation("Host {UserId} is beginning the game for room {RoomCode}", userId, roomCode);
                _gameManager.TriggerGameLoop(roomCode, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error beginning game loop for room {RoomCode}", roomCode);
                // Optionally notify the caller of the error
                // await Clients.Caller.SendAsync("Error", $"Failed to begin game: {ex.Message}");
            }
            return Task.CompletedTask;
        }

        public async Task SubmitAnswer(string roomCode, string answer)
        {
            try
            {
                var userId = GetUserIdFromContext();
                var gameSession = _gameManager.GetGameSession(roomCode);
                if (gameSession != null && userId == gameSession.HostUserId)
                {
                    _logger.LogWarning("Host {UserId} attempted to submit an answer in room {RoomCode}.", userId, roomCode);
                    await Clients.Caller.SendAsync("Error", "The host cannot submit answers.");
                    return;
                }
                _gameManager.SubmitAnswer(roomCode, userId, answer);
                await Clients.Caller.SendAsync("AnswerSubmitted");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting answer for room {RoomCode}", roomCode);
                 await Clients.Caller.SendAsync("Error", $"Failed to submit answer: {ex.Message}");
            }
        }

        public async Task SkipQuestion(string roomCode)
        {
            try
            {
                var userId = GetUserIdFromContext();
                _gameManager.SkipQuestion(roomCode, userId);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized attempt to skip question in room {RoomCode}", roomCode);
                await Clients.Caller.SendAsync("Error", ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error skipping question for room {RoomCode}", roomCode);
                await Clients.Caller.SendAsync("Error", $"Failed to skip question: {ex.Message}");
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (exception != null)
            {
                _logger.LogWarning(
                    exception,
                    "Client {ConnectionId} disconnected with error.",
                    Context.ConnectionId
                );
            }
            else
            {
                _logger.LogInformation("Client {ConnectionId} disconnected.", Context.ConnectionId);
            }

            await _gameManager.RemovePlayer(Context.ConnectionId);

            await base.OnDisconnectedAsync(exception);
        }
    }
}