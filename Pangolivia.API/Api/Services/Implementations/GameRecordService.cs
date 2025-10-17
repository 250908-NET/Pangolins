using Pangolivia.API.DTOs;
using Pangolivia.API.Models;
using Pangolivia.API.Repositories;
using Pangolivia.Repositories.Interfaces;

namespace Pangolivia.API.Services
{
    public class GameRecordService
    {
        private readonly IGameRecordRepository _gameRecordRepository;
        private readonly IUserRepository _userRepository;
        private readonly IQuizRepository _quizRepository;

        public GameRecordService(
            IGameRecordRepository gameRecordRepository,
            IUserRepository userRepository,
            IQuizRepository quizRepository)
        {
            _gameRecordRepository = gameRecordRepository;
            _userRepository = userRepository;
            _quizRepository = quizRepository;
        }

        // Create a new game session (host starts a quiz)
        public async Task<GameRecordDto> CreateGameAsync(CreateGameRecordDto dto)
        {
            var host = await _userRepository.getUserModelById(dto.HostUserId);
            var quiz = await _quizRepository.GetByIdAsync(dto.QuizId);

            if (host == null)
                throw new Exception($"Host user with ID {dto.HostUserId} not found.");
            if (quiz == null)
                throw new Exception($"Quiz with ID {dto.QuizId} not found.");

            var gameRecord = new GameRecordModel
            {
                HostUserId = dto.HostUserId,
                QuizId = dto.QuizId,
                datetimeCompleted = DateTime.UtcNow
            };

            var createdGame = await _gameRecordRepository.CreateGameRecordAsync(gameRecord);

            return new GameRecordDto
            {
                Id = createdGame.Id,
                HostUserId = createdGame.HostUserId,
                QuizId = createdGame.QuizId,
                QuizName = quiz.QuizName,
                datetimeCompleted = createdGame.datetimeCompleted
            };
        }

        // Get all games 
        public async Task<IEnumerable<GameRecordDto>> GetAllGamesAsync()
        {
            var games = await _gameRecordRepository.GetAllGameRecordsAsync();
            return games.Select(g => new GameRecordDto
            {
                Id = g.Id,
                HostUserId = g.HostUserId,
                QuizId = g.QuizId,
                datetimeCompleted = g.datetimeCompleted
            });
        }

        // Get a specific game record by ID
        public async Task<GameRecordDto?> GetGameByIdAsync(int id)
        {
            var game = await _gameRecordRepository.GetGameRecordByIdAsync(id);
            if (game == null)
                return null;

            return new GameRecordDto
            {
                Id = game.Id,
                HostUserId = game.HostUserId,
                QuizId = game.QuizId,
                datetimeCompleted = game.datetimeCompleted
            };
        }

        // Mark a game as completed (update the datetime)
        public async Task<GameRecordDto?> CompleteGameAsync(int gameId)
        {
            var game = await _gameRecordRepository.GetGameRecordByIdAsync(gameId);
            if (game == null)
                throw new Exception($"Game with ID {gameId} not found.");

            game.datetimeCompleted = DateTime.UtcNow;
            await _gameRecordRepository.CreateGameRecordAsync(game); // reuse for save/update

            return new GameRecordDto
            {
                Id = game.Id,
                HostUserId = game.HostUserId,
                QuizId = game.QuizId,
                datetimeCompleted = game.datetimeCompleted
            };
        }

        // Delete a game record (if cancelled or invalid)
        public async Task DeleteGameAsync(int gameId)
        {
            var success = await _gameRecordRepository.DeleteGameRecordAsync(gameId);
            if (!success)
                throw new Exception($"Game with ID {gameId} not found or could not be deleted.");
        }

        // Get all games created by a specific host
        public async Task<IEnumerable<GameRecordDto>> GetGamesByHostAsync(int hostUserId)
        {
            var allGames = await _gameRecordRepository.GetAllGameRecordsAsync();
            var filtered = allGames
                .Where(g => g.HostUserId == hostUserId)
                .Select(g => new GameRecordDto
                {
                    Id = g.Id,
                    HostUserId = g.HostUserId,
                    QuizId = g.QuizId,
                    datetimeCompleted = g.datetimeCompleted
                });

            return filtered;
        }

        // Get all games for a specific quiz
        public async Task<IEnumerable<GameRecordDto>> GetGamesByQuizAsync(int quizId)
        {
            var allGames = await _gameRecordRepository.GetAllGameRecordsAsync();
            var filtered = allGames
                .Where(g => g.QuizId == quizId)
                .Select(g => new GameRecordDto
                {
                    Id = g.Id,
                    HostUserId = g.HostUserId,
                    QuizId = g.QuizId,
                    datetimeCompleted = g.datetimeCompleted
                });

            return filtered;
        }
    }
}
