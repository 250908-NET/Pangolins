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
        private readonly ILogger<GameManagerService> _logger; // Add logger
        private readonly ConcurrentDictionary<string, GameSession> _games = new();
        private readonly ConcurrentDictionary<
            string,
            (string RoomCode, int UserId)
        > _connectionToRoomMap = new();

        // Add some constants for game timing
        private const int SECONDS_PER_QUESTION = 10;
        private const int SECONDS_BETWEEN_ROUNDS = 5;

        public GameManagerService(
            IHubContext<GameHub> hubContext,
            IServiceProvider serviceProvider,
            ILogger<GameManagerService> logger // Add logger
        )
        {
            _hubContext = hubContext;
            _serviceProvider = serviceProvider;
            _logger = logger; // Add logger
        }

        public async Task<string> CreateGame(int quizId, int hostUserId, string hostUsername)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var quizRepository = scope.ServiceProvider.GetRequiredService<IQuizRepository>();
                var quiz = await quizRepository.GetByIdWithDetailsAsync(quizId);
                if (quiz == null || !quiz.Questions.Any())
                {
                    throw new Exception("Quiz not found or has no questions.");
                }

                var roomCode = GenerateUniqueRoomCode();
                var newGameSession = new GameSession(
                    quiz.Id,
                    quiz.QuizName,
                    hostUserId,
                    hostUsername,
                    quiz
                );

                if (_games.TryAdd(roomCode, newGameSession))
                {
                    _logger.LogInformation(
                        "[GameManager] Game created for QuizId {QuizId} with Room Code: {RoomCode} by host {HostUsername}",
                        quizId,
                        roomCode,
                        hostUsername
                    );
                    return roomCode;
                }

                throw new Exception("Failed to create and store the game session.");
            }
        }

        public async Task StartGame(string roomCode, int requestingUserId)
        {
            if (!_games.TryGetValue(roomCode, out var gameSession))
            {
                throw new KeyNotFoundException("Game session not found.");
            }

            if (gameSession.HostUserId != requestingUserId)
            {
                throw new UnauthorizedAccessException("Only the host can start the game.");
            }

            if (gameSession.HasGameStarted())
            {
                throw new InvalidOperationException("Game has already started.");
            }
            
            // Mark the game as started internally to prevent new players
            gameSession.Start();

            // Notify all clients that the game is starting and they should navigate
            await _hubContext.Clients.Group(roomCode).SendAsync("GameStarted", gameSession.Quiz.Id);
            
            // *** CHANGE: DO NOT START THE GAME LOOP HERE. ***
            // The host's client will trigger it after navigation.
        }

        // *** NEW METHOD: To be called by the host from the GameActive page ***
        public void TriggerGameLoop(string roomCode, int requestingUserId)
        {
            if (!_games.TryGetValue(roomCode, out var gameSession))
            {
                _logger.LogWarning("TriggerGameLoop failed: Game session {RoomCode} not found.", roomCode);
                return;
            }

            if (gameSession.HostUserId != requestingUserId)
            {
                _logger.LogWarning("TriggerGameLoop failed: User {UserId} is not the host of room {RoomCode}.", requestingUserId, roomCode);
                return;
            }
            
            // Fire and forget the game loop to run in the background
            _ = RunGameLoopAsync(roomCode);
        }

        private async Task RunGameLoopAsync(string roomCode)
        {
            if (!_games.TryGetValue(roomCode, out var gameSession))
            {
                _logger.LogWarning("RunGameLoopAsync failed: Game session {RoomCode} not found.", roomCode);
                return;
            }

            _logger.LogInformation("Starting game loop for room {RoomCode}", roomCode);

            // Loop through all questions
            while (gameSession.HasNextQuestion())
            {
                var question = gameSession.AdvanceQuestion();
                await _hubContext
                    .Clients.Group(roomCode)
                    .SendAsync("ReceiveQuestion", question, SECONDS_PER_QUESTION);

                // Wait for the question duration
                gameSession.QuestionCts = new CancellationTokenSource();
                try
                {
                    // Wait for the question duration, cancellable by SkipQuestion
                    await Task.Delay(SECONDS_PER_QUESTION * 1000, gameSession.QuestionCts.Token);
                }
                catch (TaskCanceledException)
                {
                    _logger.LogInformation("Question skipped by host in room {RoomCode}", roomCode);
                }
                finally
                {
                    gameSession.QuestionCts.Dispose();
                    gameSession.QuestionCts = null;
                }

                var roundResults = gameSession.EndQuestionRound();
                await _hubContext
                    .Clients.Group(roomCode)
                    .SendAsync("ReceiveRoundResults", roundResults);

                // Wait before the next round
                await Task.Delay(SECONDS_BETWEEN_ROUNDS * 1000);
            }

            // Game has ended
            _logger.LogInformation("Game loop finished for room {RoomCode}", roomCode);
            var finalRecordForDb = gameSession.EndGameAndGetFinalGameRecord();

            // Save the results to the database
            await SaveGameRecordAsync(finalRecordForDb);

            var finalRecordForClient = new
            {
                HostUserId = finalRecordForDb.HostUserId,
                QuizId = finalRecordForDb.QuizId,
                PlayerScores = finalRecordForDb.PlayerScores.Select(playerScore => {
                    gameSession.Players.TryGetValue(playerScore.UserId, out var player);
                    return new {
                        UserId = playerScore.UserId,
                        Username = player?.Username ?? "Unknown",
                        Score = playerScore.Score
                    };
                }).ToList()
            };

            await _hubContext.Clients.Group(roomCode).SendAsync("GameEnded", finalRecordForClient);

            _games.TryRemove(roomCode, out _);
            _logger.LogInformation("Game session {RoomCode} has been removed.", roomCode);
        }

        private async Task SaveGameRecordAsync(CreateGameRecordDto finalRecord)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var gameRecordService = scope.ServiceProvider.GetRequiredService<IGameRecordService>();
                var playerRecordService =
                    scope.ServiceProvider.GetRequiredService<IPlayerGameRecordService>();
                try
                {
                    var createdGame = await gameRecordService.CreateGameAsync(finalRecord);
                    if (createdGame.Id.HasValue)
                    {
                        foreach (var playerScore in finalRecord.PlayerScores)
                        {
                            await playerRecordService.RecordScoreAsync(
                                new CreatePlayerGameRecordDto
                                {
                                    GameRecordId = createdGame.Id.Value,
                                    UserId = playerScore.UserId,
                                    Score = playerScore.Score
                                }
                            );
                        }
                    }
                    _logger.LogInformation(
                        "Successfully saved game record for quiz {QuizId} hosted by user {HostId}",
                        finalRecord.QuizId,
                        finalRecord.HostUserId
                    );
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to save game record to the database.");
                }
            }
        }

        public void SubmitAnswer(string roomCode, int userId, string answer)
        {
            if (_games.TryGetValue(roomCode, out var gameSession))
            {
                try
                {
                    gameSession.AnswerQuestionForPlayer(userId, answer);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to submit answer for user {UserId} in room {RoomCode}", userId, roomCode);
                }
            }
        }

        public void SkipQuestion(string roomCode, int requestingUserId)
        {
            if (!_games.TryGetValue(roomCode, out var gameSession))
            {
                _logger.LogWarning("SkipQuestion failed: Game session {RoomCode} not found.", roomCode);
                return;
            }

            if (gameSession.HostUserId != requestingUserId)
            {
                _logger.LogWarning("SkipQuestion failed: User {UserId} is not the host of room {RoomCode}.", requestingUserId, roomCode);
                throw new UnauthorizedAccessException("Only the host can skip a question.");
            }

            // Cancel the current question's delay task
            gameSession.QuestionCts?.Cancel();
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
                _logger.LogWarning($"[GameManager] Failed to join. Room {roomCode} not found.");
                return false;
            }

            if (gameSession.HasGameStarted())
            {
                _logger.LogWarning(
                    $"[GameManager] Failed to join. Game in room {roomCode} has already started."
                );
                return false;
            }

            _connectionToRoomMap[connectionId] = (roomCode.ToUpper(), userId);

            if (userId == gameSession.HostUserId)
            {
                gameSession.HostConnectionId = connectionId;
                _logger.LogInformation(
                    $"[GameManager] Host {username} ({userId}) connected to room {roomCode}."
                );
                return true;
            }

            if (gameSession.Players.TryGetValue(userId, out var existingPlayer))
            {
                existingPlayer.ConnectionId = connectionId;
                _logger.LogInformation(
                    $"[GameManager] Player {username} ({userId}) reconnected to room {roomCode}."
                );
            }
            else
            {
                var userDto = new UserDto { Id = userId, Username = username };
                gameSession.RegisterPlayer(userDto, connectionId);
                _logger.LogInformation(
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
                    if (
                        userId == gameSession.HostUserId
                        && gameSession.HostConnectionId == connectionId
                    )
                    {
                        gameSession.HostConnectionId = null;
                        _logger.LogInformation($"[GameManager] Host disconnected from room {roomCode}.");
                    }
                    else if (gameSession.Players.TryRemove(userId, out var removedPlayer))
                    {
                        _logger.LogInformation(
                            $"[GameManager] Player {removedPlayer.Username} removed from room {roomCode}."
                        );
                    }

                     var playerList = gameSession
                        .Players.Values.Select(p => new
                        {
                            p.UserId,
                            p.Username,
                            IsHost = false
                        })
                        .ToList();

                    if(gameSession.HostConnectionId != null) {
                         playerList.Add(
                            new
                            {
                                UserId = gameSession.HostUserId,
                                Username = gameSession.HostUsername,
                                IsHost = true
                            }
                        );
                    }

                    await _hubContext
                        .Clients.Group(roomCode)
                        .SendAsync("UpdatePlayerList", playerList);


                    if (gameSession.HostConnectionId == null && gameSession.Players.IsEmpty)
                    {
                        _games.TryRemove(roomCode, out _);
                        _logger.LogInformation(
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