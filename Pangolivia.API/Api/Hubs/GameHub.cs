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

        public async Task JoinGame(string roomCode)
        {
            var userIdStr = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            var username = Context.User?.FindFirstValue(ClaimTypes.Name) ?? "Guest";

            if (!int.TryParse(userIdStr, out var userId))
            {
                _logger.LogWarning(
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
                        gameSession.QuizName,
                        gameSession.CreatorUsername,
                        gameSession.QuestionCount,
                    };
                    await Clients.Caller.SendAsync("ReceiveLobbyDetails", lobbyDetails);

                    var playerList = gameSession
                        .Players.Values.Select(p => new { p.UserId, p.Username })
                        .ToList();
                    await Clients.Group(roomCode).SendAsync("UpdatePlayerList", playerList);
                    _logger.LogInformation(
                        "Player {Username} successfully joined room {RoomCode}. Sent updated player list to group.",
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

        // ----------------------------------------------------------------------------------
        // --- The methods below are part of the full game logic and can be left out for now ---
        // ----------------------------------------------------------------------------------

        // public async Task SubmitAnswer(string roomCode, string answer)
        // {
        //     // This will be implemented later with the real GameSession.
        // }

        // public async Task NextState(string roomCode)
        // {
        //      // This will be implemented later with the real GameSession.
        // }
    }
}
