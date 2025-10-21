using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using Pangolivia.API.Hubs;
using Pangolivia.API.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Pangolivia.API.Services
{
    public class PlayerInfo
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string ConnectionId { get; set; } = string.Empty;
    }

    public class GameSession
    {
        public int QuizId { get; }
        public int HostUserId { get; }
        public string QuizName { get; }
        public string CreatorUsername { get; }
        public int QuestionCount { get; }
        public ConcurrentDictionary<int, PlayerInfo> Players { get; } = new();

        public GameSession(int quizId, int hostUserId, string quizName, string creatorUsername, int questionCount)
        {
            QuizId = quizId;
            HostUserId = hostUserId;
            QuizName = quizName;
            CreatorUsername = creatorUsername;
            QuestionCount = questionCount;
        }
    }

    public class GameManagerService
    {
        private readonly IHubContext<GameHub> _hubContext;
        private readonly IServiceProvider _serviceProvider;
        private readonly ConcurrentDictionary<string, GameSession> _games = new();
        private readonly ConcurrentDictionary<string, (string RoomCode, int UserId)> _connectionToRoomMap = new();

        public GameManagerService(IHubContext<GameHub> hubContext, IServiceProvider serviceProvider)
        {
            _hubContext = hubContext;
            _serviceProvider = serviceProvider;
        }

        public async Task<string> CreateGame(int quizId, int hostUserId)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var quizRepository = scope.ServiceProvider.GetRequiredService<IQuizRepository>();
                var quiz = await quizRepository.GetByIdWithDetailsAsync(quizId);

                if (quiz == null)
                {
                    throw new Exception("Quiz not found.");
                }
                
                var roomCode = GenerateUniqueRoomCode();
                var newGameSession = new GameSession(
                    quizId,
                    hostUserId,
                    quiz.QuizName,
                    quiz.CreatedByUser?.Username ?? "Unknown",
                    quiz.Questions.Count
                );
                
                _games.TryAdd(roomCode, newGameSession);
                Console.WriteLine($"[GameManager] Game created for QuizId {quizId} with Room Code: {roomCode}");
                return roomCode;
            }
        }

        public GameSession? GetGameSession(string roomCode)
        {
            _games.TryGetValue(roomCode.ToUpper(), out var session);
            return session;
        }

        public bool TryAddPlayerToGame(string roomCode, int userId, string username, string connectionId)
        {
            if (!_games.TryGetValue(roomCode.ToUpper(), out var gameSession))
            {
                Console.WriteLine($"[GameManager] Failed to join. Room {roomCode} not found.");
                return false;
            }

            var player = new PlayerInfo { UserId = userId, Username = username, ConnectionId = connectionId };

            gameSession.Players.AddOrUpdate(userId, player, (key, existingPlayer) => {
                existingPlayer.ConnectionId = connectionId; // Update connection ID on reconnect
                return existingPlayer;
            });
            
            _connectionToRoomMap[connectionId] = (roomCode.ToUpper(), userId);
            
            Console.WriteLine($"[GameManager] Player {username} ({userId}) joined or reconnected to room {roomCode}.");
            return true;
        }

        public async Task RemovePlayer(string connectionId)
        {
            if (_connectionToRoomMap.TryRemove(connectionId, out var roomInfo))
            {
                var (roomCode, userId) = roomInfo;
                if (_games.TryGetValue(roomCode, out var gameSession))
                {
                    
                    if (gameSession.Players.TryGetValue(userId, out var playerInfo) && playerInfo.ConnectionId == connectionId)
                    {
                        if (gameSession.Players.TryRemove(userId, out _))
                        {
                            Console.WriteLine($"[GameManager] Player {playerInfo.Username} removed from room {roomCode}.");

                            // Broadcast the new player list to everyone remaining.
                            var playerList = gameSession.Players.Values.Select(p => new { p.UserId, p.Username }).ToList();
                            await _hubContext.Clients.Group(roomCode).SendAsync("UpdatePlayerList", playerList);

                            // If the room is now empty, close it.
                            if (gameSession.Players.IsEmpty)
                            {
                                _games.TryRemove(roomCode, out _);
                                Console.WriteLine($"[GameManager] Room {roomCode} is empty and has been closed.");
                            }
                        }
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
                roomCode = new string(Enumerable.Repeat(chars, 6)
                    .Select(s => s[random.Next(s.Length)]).ToArray());
            } while (_games.ContainsKey(roomCode));
            return roomCode;
        }
    }
}