/*C:\Users\Husan\Projects\Revature\Pangolins\Pangolivia.API\Api\Services\Implementations\GameManagerService.cs*/
using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;
using Pangolivia.API.DTOs;
using Pangolivia.API.GameEngine;
using Pangolivia.API.Hubs;
using Pangolivia.API.Repositories;

namespace Pangolivia.API.Services
{
    public class GameManagerService
    {
        private readonly IHubContext<GameHub> _hubContext;
        private readonly IServiceProvider _serviceProvider;
        private readonly ConcurrentDictionary<string, GameSession> _games = new();
        private readonly ConcurrentDictionary<
            string,
            (string RoomCode, int UserId)
        > _connectionToRoomMap = new();

        public GameManagerService(IHubContext<GameHub> hubContext, IServiceProvider serviceProvider)
        {
            _hubContext = hubContext;
            _serviceProvider = serviceProvider;
        }

        public async Task<string> CreateGame(int quizId, int hostUserId, string hostUsername)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var quizRepository = scope.ServiceProvider.GetRequiredService<IQuizRepository>();
                // The user repository is no longer needed here

                var quiz = await quizRepository.GetByIdWithDetailsAsync(quizId);
                if (quiz == null)
                {
                    throw new Exception("Quiz not found.");
                }

                // The host user lookup is no longer necessary. We trust the claims from the token.
                // var host = await userRepository.getUserModelById(hostUserId);
                // if (host == null)
                // {
                //     throw new Exception("Host user not found.");
                // }

                var roomCode = GenerateUniqueRoomCode();
                var newGameSession = new GameSession(
                    quiz.Id,
                    quiz.QuizName,
                    hostUserId,
                    hostUsername, // Use the username passed directly from the controller
                    quiz
                );

                _games.TryAdd(roomCode, newGameSession);
                Console.WriteLine(
                    $"[GameManager] Game created for QuizId {quizId} with Room Code: {roomCode} by host {hostUsername}"
                );
                return roomCode;
            }
        }

        public GameSession? GetGameSession(string roomCode)
        {
            _games.TryGetValue(roomCode.ToUpper(), out var session);
            return session;
        }

        public bool TryAddPlayerToGame(
            string roomCode,
            int userId,
            string username,
            string connectionId
        )
        {
            if (!_games.TryGetValue(roomCode.ToUpper(), out var gameSession))
            {
                Console.WriteLine($"[GameManager] Failed to join. Room {roomCode} not found.");
                return false;
            }

            if (gameSession.HasGameStarted())
            {
                Console.WriteLine(
                    $"[GameManager] Failed to join. Game in room {roomCode} has already started."
                );
                return false;
            }

            _connectionToRoomMap[connectionId] = (roomCode.ToUpper(), userId);

            // Handle host connection
            if (userId == gameSession.HostUserId)
            {
                gameSession.HostConnectionId = connectionId;
                Console.WriteLine(
                    $"[GameManager] Host {username} ({userId}) connected to room {roomCode}."
                );
                return true;
            }

            // Handle player connection/reconnection
            if (gameSession.Players.TryGetValue(userId, out var existingPlayer))
            {
                existingPlayer.ConnectionId = connectionId;
                Console.WriteLine(
                    $"[GameManager] Player {username} ({userId}) reconnected to room {roomCode}."
                );
            }
            else
            {
                var userDto = new UserDto { Id = userId, Username = username };
                gameSession.RegisterPlayer(userDto, connectionId);
                Console.WriteLine(
                    $"[GameManager] Player {username} ({userId}) joined room {roomCode}."
                );
            }

            return true;
        }

        public async Task RemovePlayer(string connectionId)
        {
            if (_connectionToRoomMap.TryRemove(connectionId, out var roomInfo))
            {
                var (roomCode, userId) = roomInfo;
                if (_games.TryGetValue(roomCode, out var gameSession))
                {
                    // Handle host disconnection
                    if (
                        userId == gameSession.HostUserId
                        && gameSession.HostConnectionId == connectionId
                    )
                    {
                        gameSession.HostConnectionId = null;
                        Console.WriteLine($"[GameManager] Host disconnected from room {roomCode}.");
                    }
                    // Handle player disconnection
                    else if (gameSession.Players.TryRemove(userId, out var removedPlayer))
                    {
                        Console.WriteLine(
                            $"[GameManager] Player {removedPlayer.Username} removed from room {roomCode}."
                        );
                    }

                    // Broadcast the new player list to everyone remaining.
                    var playerList = gameSession
                        .Players.Values.Select(p => new { p.UserId, p.Username })
                        .ToList();
                    await _hubContext
                        .Clients.Group(roomCode)
                        .SendAsync("UpdatePlayerList", playerList);

                    // If the host is disconnected and no players are left, close the room.
                    if (gameSession.HostConnectionId == null && gameSession.Players.IsEmpty)
                    {
                        _games.TryRemove(roomCode, out _);
                        Console.WriteLine(
                            $"[GameManager] Room {roomCode} is empty and has been closed."
                        );
                    }
                }
            }
        }

        private string GenerateUniqueRoomCode()
        {
            const string chars = "ABCDEFGHIJKLMNPQRSTUVWXYZ123456789";
            var random = new Random();
            string roomCode;
            do
            {
                roomCode = new string(
                    Enumerable.Repeat(chars, 6).Select(s => s[random.Next(s.Length)]).ToArray()
                );
            } while (_games.ContainsKey(roomCode));
            return roomCode;
        }
    }
}